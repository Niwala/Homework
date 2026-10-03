using UnityEditor;
using UnityEditor.UIElements;

using UnityEngine;
using UnityEngine.UIElements;

namespace Heaj.Homework
{
    [UxmlElement]
    public partial class ChapterControls : VisualElement
    {
        //Data
        private HomeworkContent content;

        //UI
        private VisualElement exportSection;
        private VisualElement loadSection;
        private VisualElement resetSection;
        private TextField surnameField;
        private TextField nameField;
        private Button exportBtn;
        private Button loadBtn;
        private Button resetBtn;

        public ChapterControls()
        {
            this.RegisterCallback<AttachToPanelEvent>(OnAttachToParent);
        }

        private void OnAttachToParent(AttachToPanelEvent e)
        {
            content = this.GetFirstAncestorOfType<HomeworkContent>();
            Chapter chapter = null;
            if (content != null)
                chapter = content.Entry as Chapter;
            bool isLoaded = chapter != null && Loader.IsLoaded(chapter);


            UserData userData = UserData.Current;
            SerializedObject so = UserData.SerializedObject;


            //Export
            exportSection = parent.Q<VisualElement>("export-section");
            exportSection.SetDisplay(isLoaded);
            surnameField = exportSection.Q<TextField>("surname-field");
            surnameField?.BindProperty(so.FindProperty(nameof(userData.userSurname)));
            nameField = exportSection.Q<TextField>("name-field");
            nameField?.BindProperty(so.FindProperty(nameof(userData.userName)));
            exportBtn = exportSection.Q<Button>("export-btn");
            if (exportBtn != null)
                exportBtn.clicked += Export;

            //Load
            loadSection = parent.Q<VisualElement>("load-section");
            loadSection.SetDisplay(!isLoaded);
            loadBtn = loadSection.Q<Button>("load-btn");
            if (loadBtn != null)
                loadBtn.clicked += Load;

            //Reset
            resetSection = parent.Q<VisualElement>("reset-section");
            resetSection.SetDisplay(isLoaded);
            resetBtn = resetSection.Q<Button>("reset-btn");
            if (resetBtn != null)
                resetBtn.clicked += Reset;
        }

        public void Export()
        {
            Chapter chapter = content.Entry as Chapter;
            if (chapter == null)
                return;

            Database.Resources.outlinerData.Export(chapter);
        }

        public void Load()
        {
            Chapter chapter = content.Entry as Chapter;
            if (chapter != null)
                Loader.Load(chapter);
        }

        public void Reset()
        {
            Chapter chapter = content.Entry as Chapter;
            if (chapter != null)
                Loader.Reset(chapter);
        }
    }
}
