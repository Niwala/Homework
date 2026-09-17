using UnityEngine.UIElements;

namespace SamsBackpack.Homework
{

    [UxmlElement]
    public partial class Title : TextElement, ICommentable
    {
        [UxmlAttribute]
        public int Type 
        {
            get => type; 
            set
            {
                for (int i = 0; i < 4; i++)
                {
                    if (i == value)
                        AddToClassList("homework-title-" + i);
                    else
                        RemoveFromClassList("homework-title-" + i);
                }
                type = value;
            }
        }
        public int type;

        public Title()
        {
            RegisterCallbackOnce<AttachToPanelEvent>(OnAttachToPanel);
        }
        private void OnAttachToPanel(AttachToPanelEvent e)
        {
            this.AddToClassList("homework-title-" + type);
        }
    }
}
