using UnityEngine.UIElements;

namespace SamsBackpack.Homework
{
    [UxmlElement]
    public partial class Callout : VisualElement
    {
        [UxmlAttribute]
        public int Type
        {
            get => type;
            set
            { 
                type = value;
            }
        }
        public int type;

        public Callout()
        {
            this.AddToClassList("homework-callout");
        }
    }
}
