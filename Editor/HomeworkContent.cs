using System;
using System.Security.Cryptography;

using UnityEditor;
using UnityEditor.Toolbars;
using UnityEditor.UIElements;

using UnityEngine;
using UnityEngine.UIElements;

namespace Heaj.Homework
{
    public class HomeworkContent : VisualElement
    {
        public IOutlinerEntry Entry { get; private set; }

        private VisualElement commentOverlay;
        private VisualElement currentOverlayedElement;
        private ToolbarToggle addArticleComment;
        private ToolbarMenu statusMenu;
        public CommentPopup commentPopup;
        private bool addCommentMode;
        private string widthPref;

        public void Show(IOutlinerEntry entry)
        {
            Entry = entry;
            Clear();
            this.AddToClassList("homework-content");

            //Tooblar
            CreateToolbar();

            //Limited banner
            LockMessage lockMessage = this.Add<LockMessage>();
            lockMessage.Bind(entry);

            //Main view
            if (entry is Warmup warmup)
            {
                ShowWarmupView(warmup);
            }
            else
            {
                ShowDefaultView(entry);
            }
        }

        private void CreateToolbar()
        {
            //Toolbars
            Toolbar articleToolbar = this.Add<Toolbar>();
            addArticleComment = articleToolbar.Add<ToolbarToggle>();
            addArticleComment.text = "Add comment";
            addArticleComment.RegisterValueChangedCallback((ChangeEvent<bool> e) => { addCommentMode = e.newValue; });

            //Status
            statusMenu = articleToolbar.Add<ToolbarMenu>();
            statusMenu.AddManipulator(new EditModeManipulator());
            SetupStatusMenu(Entry);

            //Space
            articleToolbar.Add("space").style.flexGrow = 1;

            //Edit mode toggle
            CreateEditModeToggle(articleToolbar);
        }

        private void ShowDefaultView(IOutlinerEntry entry)
        {
            bool isExercice = entry is Exercice;

            //Split
            string prefName = isExercice ? "article" : "exercice";
            widthPref = UserData.prefPrefix + prefName + ".width";
            float defaultArticleWidth = EditorPrefs.GetFloat(widthPref, 500);
            TwoPaneSplitView split = new TwoPaneSplitView(0, defaultArticleWidth, TwoPaneSplitViewOrientation.Horizontal);
            this.Add(split);

            ScrollView articleArea = split.Add<ScrollView>("article-area", "homework-article-area");
            articleArea.horizontalScrollerVisibility = ScrollerVisibility.Hidden;
            articleArea.RegisterCallback<GeometryChangedEvent>(OnSplitViewValueChanged);
            VisualElement shaderArea = split.Add<VisualElement>("shader-area", "homework-shader-area");



            //Exercices
            if (entry is Exercice exercice)
            {
                ShaderExercice shaderExercice = shaderArea.Add<ShaderExercice>();
                shaderExercice.Bind(exercice);

                //CommentElement comment = shaderArea.Add<CommentElement>();
            }

            //Article
            if (entry.Article != null)
            {
                VisualElement article = entry.Article.CloneTree();
                articleArea.Add(article);

                article.Query<VisualElement>().Where(x => x is ICommentable).ForEach(AddCommentMenu);
            }

            //Chapter
            if (entry is Chapter chapter)
            {
                OutlinerData outlinerData = Database.Resources.outlinerData;

                VisualElement chapterControls = outlinerData.chapterControls.CloneTree();
                shaderArea.Add(chapterControls);
            }

            //Comment overlay
            commentOverlay = this.Add("comment-overlay", "homework-comment-overlay");
            commentOverlay.pickingMode = PickingMode.Ignore;

            //Comment popup
            commentPopup = this.Add<CommentPopup>("comment-popup", "homework-comment-popup");
        }

        private void ShowWarmupView(Warmup warmup)
        {
            //Scroll view
            ScrollView articleArea = this.Add<ScrollView>("article-area", "homework-article-area");
            articleArea.horizontalScrollerVisibility = ScrollerVisibility.Hidden;

            //Create article
            VisualElement article = warmup.Article.CloneTree();
            articleArea.Add(article);

            //Bind counter
            WarmupCounter counter = articleArea.Q<WarmupCounter>();
            if (counter == null)
                Debug.LogError("No counter element found on this warmup : " + warmup.name);
            else
                counter.Bind(warmup);
        }

        private void AddCommentMenu(VisualElement ve)
        {
            ve.RegisterCallback<PointerEnterEvent>(OnPointerEnter);
            ve.RegisterCallback<PointerLeaveEvent>(OnPointerLeave);
            ve.RegisterCallback<PointerDownEvent>(OnPointerDown);
        }

        private void CreateEditModeToggle(VisualElement toolbar)
        {
#if HOMEWORK_EDIT
            ToolbarToggle toggle = toolbar.Add<ToolbarToggle>();
            toggle.text = "Edit mode";
            toggle.SetValueWithoutNotify(EditorPrefs.GetBool(State.editModePref, false));
            toggle.RegisterValueChangedCallback((ChangeEvent<bool> e) =>
            {
                EditorPrefs.SetBool(State.editModePref, e.newValue);
                State.onEditModeChanged?.Invoke();
                State.onMetaDataChanged?.Invoke();
            });
#endif
        }

        private void OnSplitViewValueChanged(GeometryChangedEvent e)
        {
            float width = ((VisualElement)e.target).contentRect.width;
            if (float.IsNaN(width) || width == 0)
                return;

            EditorPrefs.SetFloat(widthPref, width);
        }

        private void OnPointerEnter(PointerEnterEvent e)
        {
            VisualElement ve = e.target as VisualElement;
            if (ve == null)
                return;

            currentOverlayedElement = ve;

            Rect rect = ve.worldBound;
            rect = commentOverlay.parent.WorldToLocal(rect);
            const float margin = 4;

            commentOverlay.SetActivePseudoState(addCommentMode);
            commentOverlay.style.left = rect.x - margin;
            commentOverlay.style.top = rect.y - margin;
            commentOverlay.style.width = rect.width + margin * 2;
            commentOverlay.style.height = rect.height + margin * 2;
        }

        private void OnPointerLeave(PointerLeaveEvent e)
        {
            VisualElement ve = e.target as VisualElement;
            if (ve == null)
                return;

            if (currentOverlayedElement == ve)
                currentOverlayedElement = null;

            commentOverlay.SetActivePseudoState(false);
        }

        private void OnPointerDown(PointerDownEvent e)
        {
            if (!addCommentMode)
                return;

            VisualElement ve = e.target as VisualElement;
            if (ve == null)
                return;

            if (currentOverlayedElement != ve)
                return;

            CommentAnchor commentAnchor = CommentAnchor.AddOn(ve);
            commentAnchor.Open();

            addArticleComment.value = false;
            commentOverlay.SetActivePseudoState(false);
        }

        public void SetupStatusMenu(IOutlinerEntry entry)
        {
            MetaData metadata = UserData.GetMetaData(entry);
            string[] names = Enum.GetNames(typeof(Status));
            statusMenu.text = metadata.status.ToString();

            statusMenu.menu.ClearItems();
            for (int i = 0; i < names.Length; i++)
            {
                Status s = (Status)i;
                bool enable = s == metadata.status;
                statusMenu.menu.AppendAction(names[i], ChangeMenuStatus, GetStatus, new MenuInfo(enable, s, entry));
            }
        }

        private void ChangeMenuStatus(DropdownMenuAction a)
        {
            MenuInfo info = (MenuInfo) a.userData;
            MetaData metadata = UserData.GetMetaData(info.entry);

            metadata.status = info.status;
            statusMenu.text = info.status.ToString();

            if (info.status == Status.Limited)
            {
                DatePopup.Open((string time) =>
                {
                    metadata.limitedTime = time;
                    State.UpdateAutoLocks();
                    SetupStatusMenu(Entry);
                });
            }
            else
            {
                State.onMetaDataChanged?.Invoke();
                SetupStatusMenu(Entry);
            }
        }

        private DropdownMenuAction.Status GetStatus(DropdownMenuAction a)
        {
            MenuInfo info = (MenuInfo)a.userData;
            return info.enable ? DropdownMenuAction.Status.Checked : DropdownMenuAction.Status.Normal;
        }

        struct MenuInfo
        {
            public bool enable;
            public Status status;
            public IOutlinerEntry entry;

            public MenuInfo(bool enable, Status status, IOutlinerEntry entry)
            {
                this.enable = enable;
                this.status = status;
                this.entry = entry;
            }
        }
    }
}