using UnityEngine.UIElements;

namespace Heaj.Homework
{
    [UxmlElement]
    public partial class Text : TextElement, ICommentable
    {
        public Text()
        {
            this.AddToClassList("homework-text");
        }
    }
}
