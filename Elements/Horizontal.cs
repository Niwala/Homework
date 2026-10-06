using UnityEngine.UIElements;

namespace Heaj.Homework
{
    [UxmlElement]
    public partial class Horizontal : VisualElement
    {
        [UxmlAttribute]
        public bool Box
        {
            get => box;
            set
            {
                box = value;
                if (!value)
                    RemoveFromClassList("homework-box");
                else
                    AddToClassList("homework-box");
            }
        }

        private bool box;

        public Horizontal()
        {
            AddToClassList("homework-horizontal");
        }
    }
}
