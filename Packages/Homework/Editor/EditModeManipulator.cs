using UnityEngine.UIElements;

namespace SamsBackpack.Homework
{
    public class EditModeManipulator : Manipulator
    {
        protected override void RegisterCallbacksOnTarget()
        {
            target.RegisterCallback<AttachToPanelEvent>(OnAttachToPanel);
            target.RegisterCallback<DetachFromPanelEvent>(OnDetachFromPanel);
        }

        protected override void UnregisterCallbacksFromTarget()
        {
            target.UnregisterCallback<AttachToPanelEvent>(OnAttachToPanel);
            target.UnregisterCallback<DetachFromPanelEvent>(OnDetachFromPanel);
        }

        private void OnAttachToPanel(AttachToPanelEvent e)
        {
            State.onEditModeChanged += OnEditModeChanged;
            OnEditModeChanged();
        }

        private void OnDetachFromPanel(DetachFromPanelEvent e)
        {
            State.onEditModeChanged -= OnEditModeChanged;
        }

        private void OnEditModeChanged()
        {
            target.SetDisplay(State.InEditMode);
        }
    }
}