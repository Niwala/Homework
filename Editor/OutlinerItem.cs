using System;

using UnityEngine;
using UnityEngine.UIElements;

namespace Heaj.Homework
{
    public class OutlinerItem : VisualElement
    {
        private Label label;
        private VisualElement icon;

        private IOutlinerEntry entry;
        private new IOutlinerEntry parent;

        public OutlinerItem()
        {
            this.AddToClassList("homework-outliner-item");
            this.label = this.Add<Label>("label", "homework-outliner-item-label");
            this.icon = this.Add("icon", "homework-outliner-item-icon");

            this.RegisterCallback<DetachFromPanelEvent>(OnDetachFromPanel);
        }

        public void BindProperty(IOutlinerEntry entry, IOutlinerEntry parent)
        {
            this.entry = entry;
            this.parent = parent;

            Refresh();
        }

        private void OnDetachFromPanel(DetachFromPanelEvent e)
        {
            State.onLockChanged -= Refresh;
        }

        private void Refresh()
        {
            State.onLockChanged -= Refresh;

            //Get status
            Status status = Status.Available;
            if (!(entry is Exercice))
            {
                MetaData metadata = UserData.GetMetaData(entry);
                status = metadata.GetLockableStatus();
            }
            bool hidden = status == Status.Hidden || status == Status.Unchecked;


            //Title
            string title = entry.Title;
            if (parent != null)
            {
                string prefix = parent.Title.ToLower() + "_";
                if (title.ToLower().StartsWith(prefix))
                    title = title.Substring(prefix.Length);
            }

            label.text = title;
            label.style.opacity = hidden ? 0.5f : 1.0f;
            label.SetCheckedPseudoState(entry is Page);

            switch (status)
            {
                case Status.Limited:
                    icon.SetDisplay(true);
                    icon.style.backgroundImage = Background.FromTexture2D(Database.Resources.timeIcon);
                    icon.tooltip = "L'accès à ce chapitre est limité dans le temps.";
                    State.onLockChanged += Refresh;
                    break;

                case Status.Locked:
                    icon.SetDisplay(true);
                    icon.style.backgroundImage = Background.FromTexture2D(Database.Resources.lockIcon);
                    icon.tooltip = "Ce chapitre ne peut plus être modifié.";
                    break;

                default:
                    icon.SetDisplay(false);
                    break;
            }
        }
    }
}
