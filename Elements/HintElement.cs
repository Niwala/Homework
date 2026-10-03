using UnityEngine.UIElements;

namespace Heaj.Homework
{
    [UxmlElement]
    public partial class HintElement : VisualElement
    {
        [UxmlAttribute]
        public string text
        {
            get => label.text;
            set => label.text = value;
        }

        private bool value;
        private VisualElement content;
        private VisualElement mask;
        private Label label;

        public override VisualElement contentContainer => content;

        public HintElement()
        {
            this.pickingMode = PickingMode.Ignore;

            content = hierarchy.Add("content", "homework-hint");
            mask = hierarchy.Add("mask", "homework-hint-mask");
            mask.SetCursor(UnityEditor.MouseCursor.Link);
            label = mask.Add<Label>("label", "homework-hint-label");
            label.pickingMode = PickingMode.Ignore;

            mask.RegisterCallback<PointerUpEvent>(OnPointerUp);
        }

        private void OnPointerUp(PointerUpEvent e)
        {
            value = !value;
            mask.SetCheckedPseudoState(value);
            mask.pickingMode = PickingMode.Ignore;
        }
    }
}
