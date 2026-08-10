using System.Security.Cryptography;

using UnityEditor;
using UnityEditor.UIElements;

using UnityEngine;
using UnityEngine.UIElements;

namespace SamsBackpack.Homework
{
    public class HomeworkContent : VisualElement
    {
        public IOutlinerEntry Entry { get; private set; }

        private VisualElement commentOverlay;
        private VisualElement currentOverlayedElement;
        private ToolbarToggle addArticleComment;
        public CommentPopup commentPopup;
        private bool addCommentMode;

        public void Show(IOutlinerEntry entry)
        {
            Entry = entry;
            Clear();
            this.AddToClassList("homework-content");

            //Toolbars
            Toolbar articleToolbar = this.Add<Toolbar>();
            addArticleComment = articleToolbar.Add<ToolbarToggle>();
            addArticleComment.text = "Add comment";
            addArticleComment.RegisterValueChangedCallback((ChangeEvent<bool> e) => { addCommentMode = e.newValue; });


            if (entry is Warmup warmup)
            {
                ShowWarmupView(warmup);
            }
            else
            {
                ShowDefaultView(entry);
            }
        }

        private void ShowDefaultView(IOutlinerEntry entry)
        {
            //Split
            TwoPaneSplitView split = new TwoPaneSplitView(0, 600, TwoPaneSplitViewOrientation.Horizontal);
            this.Add(split);

            ScrollView articleArea = split.Add<ScrollView>("article-area", "homework-article-area");
            articleArea.horizontalScrollerVisibility = ScrollerVisibility.Hidden;
            ScrollView shaderArea = split.Add<ScrollView>("shader-area", "homework-shader-area");



            //Exercices
            if (entry is Exercice exercice)
            {
                ShaderExercice shaderExercice = shaderArea.Add<ShaderExercice>();
                shaderExercice.Bind(exercice);

                CommentElement comment = shaderArea.Add<CommentElement>();
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
                VisualElement chapterExportSection = outlinerData.chapterExport.CloneTree();
                ChapterControls controls = chapterExportSection.Q<ChapterControls>();
                controls.Bind(outlinerData, chapter);
                shaderArea.Add(chapterExportSection);
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

    }
}
