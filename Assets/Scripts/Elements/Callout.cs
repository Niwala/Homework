using System.Runtime.CompilerServices;

using UnityEngine;
using UnityEngine.UIElements;

namespace SamsBackpack.Homework
{

    [UxmlElement]
    public partial class Callout : VisualElement, ICommentable
    {
        [UxmlAttribute]
        public Texture2D Icon
        {
            get => icon;
            set
            {
                icon = value;
                SetIcon(Icon);
            }
        }
        public Texture2D icon;

        [UxmlAttribute, Range(0.0f, 1.0f)]
        public float Opacity
        {
            get => opacity;
            set
            {
                if (iconElement != null)
                    iconElement.style.opacity = value;
                opacity = value;
            }
        }
        public float opacity = 1.0f;

        [UxmlAttribute]
        public Color Color
        {
            get => color;
            set
            {
                if (iconElement != null)
                    iconElement.style.unityBackgroundImageTintColor = value;
                color = value;
            }
        }
        public Color color = Color.white;

        private VisualElement iconElement;
        private VisualElement content;

        public override VisualElement contentContainer => content;

        public Callout()
        {
            AddToClassList("homework-callout");

            iconElement = hierarchy.Add<VisualElement>("icon", "homework-callout-icon");
            SetIcon(Icon);
            iconElement.style.unityBackgroundImageTintColor = color;
            iconElement.style.opacity = opacity;
            content = hierarchy.Add("content");
            content.style.flexGrow = 1;
        }

        private void SetIcon(Texture2D icon)
        {
            if (iconElement == null)
                return;

            if (icon == null)
                iconElement.style.display = DisplayStyle.None;
            else
            {
                iconElement.style.backgroundImage = Background.FromTexture2D(icon);
                iconElement.style.display = DisplayStyle.Flex;
            }
        }
    }
}
