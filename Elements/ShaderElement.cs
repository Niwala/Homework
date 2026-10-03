using UnityEditor;

using UnityEngine;
using UnityEngine.UIElements;

namespace Heaj.Homework
{
    [UxmlElement]
    public partial class ShaderElement : VisualElement, ICommentable
    {

        [UxmlAttribute]
        public Shader Shader
        {
            get
            {
                return shader;
            }
            set
            {
                if (shader == value)
                    return;

                shader = value;

                if (shader == null)
                    material = null;
                else
                    material = new Material(shader);

                style.unityMaterial = material;
            }
        }

        [UxmlAttribute]
        public bool constantRepaint = true;

        private Shader shader;
        private Material material;
        private double lastRepaint;

        public ShaderElement()
        {
            AddToClassList("homework-shader-element");
            this.generateVisualContent += OnGenerateVisualContent;

            this.RegisterCallback<AttachToPanelEvent>(OnAttachToPanel);
            this.RegisterCallback<DetachFromPanelEvent>(OnDetachFromPanel);
        }

        private void OnGenerateVisualContent(MeshGenerationContext ctx)
        {
            if (shader == null || material == null)
                return;

            float time = (float)(EditorApplication.timeSinceStartup % 1000);
            material.SetVector("_TimeParameters", new Vector4(time, 0, 0, 0));
        }

        private void OnAttachToPanel(AttachToPanelEvent e)
        {
            if (constantRepaint)
                EditorApplication.update += Loop;
        }

        private void OnDetachFromPanel(DetachFromPanelEvent e)
        {
            EditorApplication.update -= Loop;
        }

        private void Loop()
        {
            if ((EditorApplication.timeSinceStartup - lastRepaint) < 0.016f || !constantRepaint)
                return;
            lastRepaint = EditorApplication.timeSinceStartup;

            MarkDirtyRepaint();
        }

        public void SetFloat(string propertyName, float value)
        {
            material?.SetFloat(propertyName, value);
        }

        public void SetColor(string propertyName, Color value)
        {
            material?.SetColor(propertyName, value);
        }
    }
}
