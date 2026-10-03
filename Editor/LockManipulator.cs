using System;

using UnityEngine.UIElements;

namespace Heaj.Homework
{
    public class LockManipulator : Manipulator
    {
        private Action onLockChanged;

        public LockManipulator(Action onLockChanged)
        {
            this.onLockChanged = onLockChanged;
        }

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
            State.onLockChanged += onLockChanged;
            onLockChanged.Invoke();
        }

        private void OnDetachFromPanel(DetachFromPanelEvent e)
        {
            State.onLockChanged -= onLockChanged;
        }
    }
}