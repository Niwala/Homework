using System.Threading.Tasks;

using UnityEditor.UIElements;

using UnityEngine.UIElements;

namespace SamsBackpack.Homework
{
    public class HomeworkStatusElement : VisualElement
    {
        private Label label;
        private VisualElement icon;

        public HomeworkStatusElement()
        {
            this.AddToClassList("homework-status");
            label = this.Add<Label>("label", "homework-status-label");
            icon = this.Add("icon", "homework-status-icon");

            this.RegisterCallback<AttachToPanelEvent>(OnAttachToPanel);
            this.RegisterCallback<DetachFromPanelEvent>(OnDetachFromPanel);
        }

        private void OnAttachToPanel(AttachToPanelEvent e)
        {
            State.onMetaDataChanged += OnMetadataChanged;
            OnMetadataChanged();
        }

        private void OnDetachFromPanel(DetachFromPanelEvent e)
        {
            State.onMetaDataChanged -= OnMetadataChanged;
        }

        private void OnMetadataChanged()
        {
            icon.SetCheckedPseudoState(false);

            if (State.HasMetaData)
            {
                if (State.UseLocalMetaData)
                {
                    label.SetDisplay(true);
                    label.text = "Local";
                    tooltip = "<b>The last update failed</b>\nThe last exercise table will be used instead.\n\n" +
                        "<b>Error message :</b>\n" + State.MetaDataDownloadMsg;
                    icon.style.backgroundImage = Background.FromTexture2D(Database.Resources.warningIcon);
                }
                else
                {
                    icon.style.backgroundImage = Background.FromTexture2D(Database.Resources.validIcon);
                    label.SetDisplay(false);
                    tooltip = "<b>Up-to-date</b>\nThe latest update went smoothly.";
                }
            }
            else
            {
                label.SetDisplay(true);
                label.text = "Loading...";
                icon.style.backgroundImage = Background.FromTexture2D(Database.Resources.loaderIcon);
                icon.SetCheckedPseudoState(true);
                tooltip = "<b>Loading</b>\nLoading the latest exercise table from the Internet...";
            }
        }
    }
}
