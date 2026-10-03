using UnityEngine.UIElements;

namespace Heaj.Homework
{
    [UxmlElement]
    public partial class ShaderSlider : Slider
    {
        [UxmlAttribute]
        public string propertyName;

        public GraphElement graphTarget;
        public ShaderElement shaderTarget;


        public ShaderSlider()
        {
            this.RegisterCallback<AttachToPanelEvent>(OnAttachToPanel);
            this.RegisterValueChangedCallback(OnValueChanged);
        }

        private void OnAttachToPanel(AttachToPanelEvent e)
        {
            graphTarget = this.GetFirstAncestorOfType<GraphElement>();
            shaderTarget = this.GetFirstAncestorOfType<ShaderElement>();
            Send(value);
        }

        private void OnValueChanged(ChangeEvent<float> e)
        {
            if (string.IsNullOrEmpty(propertyName))
                return;

            Send(e.newValue);
        }

        private void Send(float value)
        {
            if (graphTarget != null)
                graphTarget.SetFloat(propertyName, value);

            if (shaderTarget != null)
                shaderTarget.SetFloat(propertyName, value);
        }
    }
}