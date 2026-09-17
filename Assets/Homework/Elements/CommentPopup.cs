using UnityEngine;
using UnityEngine.UIElements;

namespace SamsBackpack.Homework
{
    public partial class CommentPopup : VisualElement
    {
        private Label label;
        private TextField textField;
        private Button saveBtn;
        private Button removeBtn;

        private VisualElement target;

        public CommentPopup()
        {
            this.AddToClassList("homework-comment-popup");

            label = this.Add<Label>("label", "homework-title-2");
            label.text = "Add comment";
            label.style.marginTop = 3;
            label.style.marginLeft = 2;
            label.style.marginBottom = 10;

            textField = this.Add<TextField>();
            textField.style.flexGrow = 1;
            textField.style.minHeight = 120;
            textField.style.fontSize = 14;
            textField.multiline = true;

            VisualElement h = this.Add("controls");
            h.style.flexDirection = FlexDirection.Row;
            h.style.justifyContent = Justify.FlexEnd;
            h.style.marginTop = 5;

            removeBtn = h.Add<Button>("removeBtn");
            removeBtn.text = "Remove";
            removeBtn.clicked += Remove;

            saveBtn = h.Add<Button>("saveBtn");
            saveBtn.text = "Save";
            saveBtn.clicked += Save;
        }

        public void Open(VisualElement target)
        {
            this.target = target;
            //TODO : Check if element has already a comment

            this.style.display = DisplayStyle.Flex;

        }

        public void Close()
        {
            this.style.display = DisplayStyle.None;
        }

        private void Save()
        {
            CommentAnchor commentAnchor = CommentAnchor.AddOn(target);
            Close();
        }

        private void Remove()
        {
            Close();
        }
    }
}