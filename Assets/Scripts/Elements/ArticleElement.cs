using UnityEngine;
using UnityEngine.UIElements;

namespace SamsBackpack.Homework
{
    [UxmlElement]
    public partial class ArticleElement : VisualElement
    {
        public ArticleElement()
        {
            this.AddToClassList("homework-article");
            RegisterCallbackOnce<AttachToPanelEvent>(OnAttachToPanel);
        }

        private void OnAttachToPanel(AttachToPanelEvent e)
        {
            styleSheets.Add(Database.Resources.styles);
        }
    }
}
