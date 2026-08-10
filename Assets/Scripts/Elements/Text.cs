using UnityEngine.UIElements;

namespace SamsBackpack.Homework
{
    [UxmlElement]
    public partial class Text : TextElement
    {
        public Text()
        {
            this.AddToClassList("homework-text");
        }
    }
}
