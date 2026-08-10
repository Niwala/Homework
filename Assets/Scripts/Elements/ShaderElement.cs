using System.Threading.Tasks;

using UnityEditor;

using UnityEngine;
using UnityEngine.UIElements;

namespace SamsBackpack.Homework
{
    [UxmlElement]
    public partial class ShaderElement : VisualElement, ICommentable
    {

        [UxmlAttribute]
        public Shader shader { get; set; }

        public Material material { get; private set; }
        private bool enable;

        public ShaderElement()
        {
            AddToClassList("homework-shader-element");

            RegisterCallback<AttachToPanelEvent>(OnAttachToPanel);
            RegisterCallback<DetachFromPanelEvent>(OnDetachFromPanel);

            this.generateVisualContent += GenerateVisualContent;
            this.usageHints = UsageHints.LargePixelCoverage | UsageHints.DynamicColor | UsageHints.DynamicPostProcessing | UsageHints.MaskContainer;
        }

        private void OnAttachToPanel(AttachToPanelEvent e)
        {
            Refresh();
        }

        private void OnDetachFromPanel(DetachFromPanelEvent e)
        {
            style.unityMaterial = null;
            material = null;
            enable = false;
        }

        private void GenerateVisualContent(MeshGenerationContext ctx)
        {
            if (material != null)
            {
                float time = (float)(EditorApplication.timeSinceStartup % 1000);
                material.SetVector("_TimeParameters", new Vector4(time, 0, 0, 0));
            }
        }

        public void Refresh()
        {
            if (shader == null)
            {
                enable = false;
                return;
            }

            if (!enable)
            {
                enable = true;
                Loop();
                AnimLoop();
            }

            material = new Material(shader);
            style.unityMaterial = material;
        }

        private async void Loop()
        {
            if (!enable)
                return;

            await Task.Delay(4);

            MarkDirtyRepaint();
            Loop();
        }

        private async void AnimLoop()
        {
            if (!enable)
                return;

            await Task.Delay(2000);

            SetCheckedPseudoState(!hasCheckedPseudoState);
            AnimLoop();
        }
    }
}
