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

        private IMGUIContainer imguiContainer;

        public Material material { get; private set; }
        private bool enable;

        public ShaderElement()
        {
            AddToClassList("homework-shader-element");

            imguiContainer = this.Add<IMGUIContainer>();
            imguiContainer.StretchToParentSize();
            imguiContainer.onGUIHandler += OnDrawGUI;
            imguiContainer.usageHints = UsageHints.DynamicColor;

            //this.usageHints = UsageHints.DynamicColor;
        }

        private void OnDrawGUI()
        {
            if (shader == null)
                return;

            if (material == null)
                material = new Material(shader);

            float time = (float)(EditorApplication.timeSinceStartup % 1000);
            material.SetVector("_TimeParameters", new Vector4(time, 0, 0, 0));
            style.unityMaterial = material;
            imguiContainer.MarkDirtyRepaint();
        }
    }
}
