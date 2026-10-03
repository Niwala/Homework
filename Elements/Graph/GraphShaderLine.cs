using UnityEditor;
using UnityEditor.AssetImporters;

using UnityEngine;
using UnityEngine.UIElements;

namespace Heaj.Homework
{
    [UxmlElement]
    public partial class GraphShaderLine : VisualElement, IGraphDrawer
    {
        [UxmlAttribute]
        public Color color = new Color(1, 0.235f, 0.235f, 1);

        [UxmlAttribute]
        public float thickness = 3.0f;

        //[UxmlAttribute]
        //public Vector2 start;

        //[UxmlAttribute]
        //public Vector2 end;

        [UxmlAttribute, Multiline(4)]
        public string uniforms;

        [UxmlAttribute, Multiline(8)]
        public string function;

        [UxmlAttribute, Min(2)]
        public int segmentCount = 2;

        private float[] values;
        private ComputeBuffer buffer;
        private GraphFrame frame;
        private ComputeShader shader;

        public GraphShaderLine()
        {
            this.RegisterCallback<AttachToPanelEvent>(OnAttachToPanel);
            this.RegisterCallback<DetachFromPanelEvent>(OnDetachFromPanel);
        }

        private void OnAttachToPanel(AttachToPanelEvent e)
        {
            segmentCount = Mathf.Max(2, segmentCount);
            buffer = new ComputeBuffer(segmentCount, sizeof(float));
            values = new float[segmentCount];

            frame = this.GetFirstAncestorOfType<GraphFrame>();
            if (frame != null)
                frame.childs.Add(this);
        }

        private void OnDetachFromPanel(DetachFromPanelEvent e)
        {
            buffer?.Release();
            buffer = null;
            values = null;

            frame?.childs.Remove(this);
        }

        public void Draw(MeshGenerationContext ctx, IGraphDrawer.Remap remap, IGraphProperties properties)
        {
            //Painter styles
            ctx.painter2D.strokeColor = color;
            ctx.painter2D.lineWidth = thickness;
            ctx.painter2D.lineCap = LineCap.Round;

            //Line from shader
            ctx.painter2D.BeginPath();
            LineFromShader(ctx.painter2D, remap, properties);
            ctx.painter2D.Stroke();

            //ctx.painter2D.BeginPath();
            //ctx.painter2D.MoveTo(start);
            //ctx.painter2D.LineTo(end);
            //ctx.painter2D.Stroke();
        }

        public void LineFromShader(Painter2D painter, IGraphDrawer.Remap remap, IGraphProperties properties)
        {
            //Resize
            segmentCount = Mathf.Max(2, segmentCount);
            if (values == null || values.Length != segmentCount)
            {
                values = new float[segmentCount];
                buffer?.Release();
                buffer = new ComputeBuffer(segmentCount, sizeof(float));
            }


            //Get shader
            if (shader == null)
                shader = TransientComputeShaders.Get(uniforms, function);
            if (shader == null)
                return;


            //Execute shader
            properties.Apply(shader);
            shader.SetBuffer(0, "_Values", buffer);
            shader.SetInt("_ValueCount", segmentCount);
            shader.Dispatch(0, Mathf.CeilToInt(segmentCount / 8.0f), 1, 1);
            buffer.GetData(values);


            //Draw line
            painter.MoveTo(remap(0, values[0]));
            for (int i = 0; i < values.Length; i++)
            {
                float x = i / (values.Length - 1.0f);
                painter.LineTo(remap(x, values[i]));
            }
        }

        public void SetFloat(string propertyName, float value)
        {
            shader?.SetFloat(propertyName, value);
        }

        public void SetColor(string propertyName, Color value)
        {
            shader?.SetVector(propertyName, value);
        }
    }
}
