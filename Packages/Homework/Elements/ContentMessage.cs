using UnityEngine;
using UnityEngine.UIElements;

namespace SamsBackpack.Homework
{
    public class ContentMessage : VisualElement
    {
        private TextElement textElement;

        public ContentMessage()
        {
            AddToClassList("homework-content-message");
            textElement = this.Add<TextElement>("text", "homework-content-message-text");
        }
    }

    public class LockMessage : VisualElement
    {
        private IOutlinerEntry entry;
        private TextElement textElement;

        public LockMessage()
        {
            AddToClassList("homework-content-message");
            textElement = this.Add<TextElement>("text", "homework-content-message-text");
            this.AddManipulator(new LockManipulator(Refresh));
        }

        public void Bind(IOutlinerEntry entry)
        {
            this.entry = entry;
            Refresh();
        }

        private void Refresh()
        {
            if (entry == null)
            {
                this.SetDisplay(false);
                return;
            }

            MetaData metadata = UserData.GetMetaData(entry);
            Status status = metadata.GetLockableStatus();

            switch (status)
            {
                case Status.Limited:
                    this.SetDisplay(true);
                    textElement.text = "L'édition de ce chapitre est limitée dans le temps.\n" +
                        "Le chapitre sera bloqué automatiquement à cette date : " + metadata.limitedTime;
                    break;

                case Status.Locked:
                    this.SetDisplay(true);
                    textElement.text = "L'édition de ce chapitre est bloquée.\n" +
                        "Vous pouvez toujours exporter vos solutions sur la page du chapitre.";
                    break;

                default: this.SetDisplay(false); break;
            }
        }
    }
}
