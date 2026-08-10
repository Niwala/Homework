using UnityEditor;
using UnityEditor.UIElements;

using UnityEngine;
using UnityEngine.UIElements;

namespace SamsBackpack.Homework
{
    [UxmlElement]
    public partial class ChapterControls : VisualElement
    {
        private TextField surnameField;
        private TextField nameField;

        private Button exportBtn;
        private Chapter chapter;

        public ChapterControls()
        {
            //Fields
            VisualElement fields = this.Add("fields", "homework-chapter-fields");
            surnameField = fields.Add<TextField>("surname-field");
            surnameField.label = "Nom";
            surnameField.style.marginBottom = 5;

            nameField = fields.Add<TextField>("name-field");
            nameField.label = "Prénom";
            nameField.style.marginBottom = 5;


            //Controls
            VisualElement controls = this.Add("controls", "homework-chapter-controls");

            //Export btn
            exportBtn = controls.Add<Button>("export-btn", "homework-chapter-controls-btn");
            exportBtn.text = "Export";
            exportBtn.clicked += Export;
        }

        public void Bind(OutlinerData outlinerData, Chapter chapter)
        {
            this.chapter = chapter;

            SerializedObject so = new SerializedObject(outlinerData);
            surnameField.BindProperty(so.FindProperty(nameof(outlinerData.userSurname)));
            nameField.BindProperty(so.FindProperty(nameof(outlinerData.userName)));
        }

        public void Export()
        {
            if (chapter == null)
                return;

            Database.Resources.outlinerData.Export(chapter);
        }
    }
}
