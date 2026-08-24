using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UIElements;

namespace SamsBackpack.Homework
{
    [UxmlElement]
    public partial class GraphElement : VisualElement, IGraphDrawer
    {
        [UxmlAttribute]
        public int FrameID
        {
            get => frameID;
            set { frameID = value; UpdateFrame(); }
        }
        private int frameID = 0;

        [UxmlAttribute]
        public bool ShowXLabels
        {
            get => showXLabels;
            set { showXLabels = value; UpdateVisibilities(); }
        }
        private bool showXLabels = true;


        [UxmlAttribute]
        public bool ShowYLabels
        {
            get => showYLabels;
            set { showYLabels = value; UpdateVisibilities(); }
        }
        private bool showYLabels = true;

        [UxmlAttribute]
        public bool ShowShader
        {
            get => showShader;
            set { showShader = value; UpdateVisibilities(); }
        }
        private bool showShader = true;

        [UxmlAttribute]
        public Shader Shader
        {
            get => shader.Shader;
            set => shader.Shader = value;
        }

        [UxmlAttribute]
        public Vector2 min = Vector2.zero;

        [UxmlAttribute]
        public Vector2 max = Vector2.one;

        [UxmlAttribute]
        public Color lineColor = new Color(0.3f, 0.3f, 0.3f, 1);

        [UxmlAttribute]
        public Color backgroundColor = new Color(0.15f, 0.15f, 0.15f, 1);

        [UxmlAttribute]
        public float thickness = 1.0f;

        [UxmlAttribute]
        public bool drawZero = true;

        public GraphFrame currentFrame;

        private VisualElement description;
        private VisualElement view;
        private VisualElement canvas;
        private VisualElement top;
        private VisualElement right;
        private VisualElement corner;
        private ShaderElement shader;
        private VisualElement shaderView;
        public override VisualElement contentContainer => description;

        private IGraphProperties properties = new IGraphProperties();

        public GraphElement()
        {
            this.AddToClassList("homework-graph-element");
            this.generateVisualContent += GenerateVisualContent;

            //Vertical
            VisualElement vertical = hierarchy.Add("vertical");
            vertical.style.width = new Length(50, LengthUnit.Percent);
            vertical.style.aspectRatio = 0.5f;

            //View
            view = vertical.Add("view", "homework-graph-view");
            canvas = view.Add("canvas", "homework-graph-canvas");

            //Shader
            shaderView = vertical.Add<ShaderElement>("shader-view", "homework-graph-view");
            shaderView.style.marginTop = 20;
            shaderView.style.marginRight = 2;
            shaderView.style.flexGrow = 0;

            shader = shaderView.Add<ShaderElement>();
            shader.style.flexGrow = 0;

            //Description
            description = hierarchy.Add("description", "homework-graph-description");

            //Corner
            corner = view.Add("corner", "homework-graph-bar");
            corner.style.top = corner.style.right = 0;
            corner.style.width = corner.style.height = 20;

            //Top
            top = view.Add("top", "homework-graph-bar");
            top.style.borderBottomWidth = 1;

            //Right
            right = view.Add("right", "homework-graph-bar");
            right.style.borderLeftWidth = 1;

            UpdateVisibilities();

            this.RegisterCallback<AttachToPanelEvent>(OnAttachToPanel);
        }

        private void OnAttachToPanel(AttachToPanelEvent e)
        {
            AddBarLabels(top, true); 
            AddBarLabels(right, false);
        }

        private void UpdateVisibilities()
        {
            float topHeight = showXLabels ? 20 : 0;
            float rightWidth = showYLabels ? 20 : 0;

            canvas.style.marginTop = topHeight;
            canvas.style.marginRight = rightWidth;

            top.style.display = showXLabels ? DisplayStyle.Flex : DisplayStyle.None;
            top.style.top = top.style.left = 0;
            top.style.right = rightWidth;
            top.style.height = 20;

            right.style.display = showYLabels ? DisplayStyle.Flex : DisplayStyle.None;
            right.style.flexDirection = FlexDirection.ColumnReverse;
            right.style.right = right.style.bottom = 0;
            right.style.top = topHeight;
            right.style.width = 20;

            corner.style.display = (showXLabels && showYLabels) ? DisplayStyle.Flex : DisplayStyle.None;

            shaderView.style.marginRight = rightWidth;
            shader.style.display = showShader ? DisplayStyle.Flex : DisplayStyle.None;
        }

        private void UpdateFrame()
        {
            List<GraphFrame> frames = this.Query<GraphFrame>().ToList();
            for (int i = 0; i < frames.Count; i++)
            {
                if (i == frameID)
                {
                    frames[i].style.display = DisplayStyle.Flex;
                    currentFrame = frames[i];
                }
                else
                    frames[i].style.display = DisplayStyle.None;
            }
        }

        private void AddBarLabels(VisualElement element, bool horizontal)
        {
            float minValue = horizontal ? min.x : min.y;
            float maxValue = horizontal ? max.x : max.y;
            float midValue = (minValue + maxValue) * 0.5f;

            AddBarLabels(element, horizontal, 0.0f, minValue.ToString("0.0"));
            AddBarLabels(element, horizontal, 0.5f, midValue.ToString("0.0"));
            AddBarLabels(element, horizontal, 1.0f, maxValue.ToString("0.0"));
        }

        private void AddBarLabels(VisualElement element, bool horizontal, float t, string text)
        {
            Label label = element.Add<Label>("label", "homework-graph-bar-label");
            //label.style.position = Position.Absolute;
            label.text = text;
            //if (horizontal)
            //    label.style.left = new Length(t * 100, LengthUnit.Percent);
            //else
            //    label.style.bottom = new Length(t * 100, LengthUnit.Percent);
        }

        private void GenerateVisualContent(MeshGenerationContext ctx)
        {
            //Draw background & Zero line
            Draw(ctx, Remap, properties);

            //Draw frame content
            if (currentFrame != null)
                currentFrame.Draw(ctx, Remap, properties);
        }

        private Vector2 Remap(float x, float y)
        {
            x = Mathf.InverseLerp(min.x, max.x, x);
            y = Mathf.InverseLerp(min.y, max.y, y);

            Rect rect = canvas.contentRect;
            rect.y += showXLabels ? 20 : 0;
            return new Vector2(rect.x + rect.width * x, rect.yMax - rect.height * y);
        }

        public void Draw(MeshGenerationContext ctx, IGraphDrawer.Remap remap, IGraphProperties properties)
        {
            //Background
            ctx.painter2D.BeginPath();
            ctx.painter2D.fillColor = backgroundColor;
            ctx.painter2D.MoveTo(remap(min.x, min.y));
            ctx.painter2D.LineTo(remap(max.x, min.y));
            ctx.painter2D.LineTo(remap(max.x, max.y));
            ctx.painter2D.LineTo(remap(min.x, max.y));
            ctx.painter2D.Fill();

            //0.0 line
            if (drawZero)
            {
                ctx.painter2D.strokeColor = lineColor;
                ctx.painter2D.lineWidth = thickness;

                float midY = (min.y + max.y) * 0.5f;
                ctx.painter2D.BeginPath();
                ctx.painter2D.MoveTo(remap(min.x, midY));
                ctx.painter2D.LineTo(remap(max.x, midY));
                ctx.painter2D.Stroke();
            }
        }

        public void SetFloat(string propertyName, float value)
        {
            if (properties.floatValues.ContainsKey(propertyName))
                properties.floatValues[propertyName] = value;
            else
                properties.floatValues.Add(propertyName, value);

            shader.SetFloat(propertyName, value);
            this.MarkDirtyRepaint();
        }

        public void SetColor(string propertyName, Color value)
        {
            if (properties.colorValues.ContainsKey(propertyName))
                properties.colorValues[propertyName] = value;
            else
                properties.colorValues.Add(propertyName, value);

            shader.SetColor(propertyName, value);
            this.MarkDirtyRepaint();
        }
    }

    public interface IGraphDrawer
    {
        public delegate Vector2 Remap(float x, float y);

        public void Draw(MeshGenerationContext ctx, Remap remap, IGraphProperties properties);
    }

    public class IGraphProperties
    {
        public Dictionary<string, float> floatValues = new Dictionary<string, float>();
        public Dictionary<string, Color> colorValues = new Dictionary<string, Color>();

        public void Apply(Material material)
        {
            if (material == null)
                return;

            foreach (var item in floatValues)
                material.SetFloat(item.Key, item.Value);
            foreach (var item in colorValues)
                material.SetColor(item.Key, item.Value);
        }

        public void Apply(ComputeShader shader)
        {
            if (shader == null)
                return;

            foreach (var item in floatValues)
                shader.SetFloat(item.Key, item.Value);
            foreach (var item in colorValues)
                shader.SetVector(item.Key, item.Value);
        }
    }
}