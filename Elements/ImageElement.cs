using UnityEngine;
using UnityEngine.UIElements;

namespace Heaj.Homework
{

    [UxmlElement]
    public partial class ImageElement : Image, ICommentable
    {
        [UxmlAttribute]
        public float Size
        {
            get => size;
            set
            {
                size = value;
                OnAttachToPanel(null);
            }
        }

        private float size = 0.7f;

        public ImageElement()
        {
            this.AddToClassList("homework-image");
            this.RegisterCallbackOnce<AttachToPanelEvent>(OnAttachToPanel);
        }

        private void OnAttachToPanel(AttachToPanelEvent e)
        {
            Texture2D tex = this.image as Texture2D;
            if (tex == null)
                return;

            this.style.aspectRatio = tex.width / (float)tex.height;
            this.style.maxWidth = tex.width * size;
            this.style.maxHeight = tex.height * size;
        }
    }
}
