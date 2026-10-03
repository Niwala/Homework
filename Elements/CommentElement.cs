using UnityEngine.UIElements;

namespace Heaj.Homework
{

    public partial class CommentElement : VisualElement
    {
        public VisualElement icon;
        public TextField textField;

        public CommentElement()
        {
            AddToClassList("homework-comment");
            icon = this.Add("icon", "homework-comment-icon");
            icon.style.backgroundImage = Background.FromTexture2D(Database.Resources.commentIcon);
            textField = this.Add<TextField>("field", "homework-comment");
        }

        public void Bind(IOutlinerEntry entry)
        {

        }
    }
}
