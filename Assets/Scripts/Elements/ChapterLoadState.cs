using UnityEngine;
using UnityEngine.UIElements;

namespace SamsBackpack.Homework
{
    [UxmlElement]
    public partial class ChapterLoadState : VisualElement
    {
        public TextElement text;


        public ChapterLoadState()
        {
            text = this.Add<TextElement>();

        }

    }
}
