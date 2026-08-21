using UnityEditor;
using UnityEditor.UIElements;

using UnityEngine;
using UnityEngine.UIElements;

namespace SamsBackpack.Homework
{
    [UxmlElement]
    public partial class ChapterControls : VisualElement
    {
        //Data
        private HomeworkContent content;

        //UI
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
            UserData userData = UserData.Current;
            SerializedObject so = UserData.SerializedObject;

            //Export
            surnameField = parent.Q<TextField>("surname-field");
            surnameField?.BindProperty(so.FindProperty(nameof(userData.userSurname)));
            nameField = parent.Q<TextField>("name-field");
            nameField?.BindProperty(so.FindProperty(nameof(userData.userName)));
            exportBtn = parent.Q<Button>("export-btn");
            if (exportBtn != null)
                exportBtn.clicked += Export;

            //Load
            loadBtn = parent.Q<Button>("load-btn");
            if (loadBtn != null)
                loadBtn.clicked += Load;

            //Reset
            resetBtn = parent.Q<Button>("reset-btn");
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
