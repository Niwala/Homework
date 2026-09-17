using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UIElements;

namespace SamsBackpack.Homework
{
    [UxmlElement]
    public partial class GraphFrame : VisualElement, IGraphDrawer
    {
        public HashSet<IGraphDrawer> childs = new HashSet<IGraphDrawer>();

        public GraphFrame()
        {
            this.AddToClassList("homework-graph-frame");
            this.RegisterCallback<AttachToPanelEvent>(OnAttachToPanelEvent);
        }

        public void Draw(MeshGenerationContext ctx, IGraphDrawer.Remap remap, IGraphProperties properties)
        {
            foreach (var drawer in childs)
            {
                if (drawer != null)
                    drawer.Draw(ctx, remap, properties);
            }
        }

        private void OnAttachToPanelEvent(AttachToPanelEvent e)
        {
            GraphElement element = this.GetFirstAncestorOfType<GraphElement>();
            if (element != null && element.currentFrame == null)
                element.currentFrame = this;
        }
    }
}
