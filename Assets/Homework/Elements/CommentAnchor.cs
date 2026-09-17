using UnityEngine.UIElements;

namespace SamsBackpack.Homework
{
    public partial class CommentAnchor : Button
    {
        private VisualElement icon;
        private VisualElement target;

        public CommentAnchor()
        {
            this.AddToClassList("homework-comment-anchor");
            this.SetCursor(UnityEditor.MouseCursor.Link);

            icon = this.Add("icon");
            icon.pickingMode = PickingMode.Ignore;
            icon.style.flexGrow = 1;
            icon.style.backgroundImage = Background.FromTexture2D(Database.Resources.commentIcon);

            this.clicked += Open;
        }

        public static CommentAnchor AddOn(VisualElement ve)
        {
            VisualElement parent = ve.parent;
            int childIndex = 0;
            for (int i = 0; i < parent.childCount; i++)
            {
                if (parent[i] == ve)
                {
                    childIndex = i;
                    break;
                }
            }

            VisualElement container = parent.Insert<VisualElement>(childIndex, "comment-container");
            ve.RemoveFromHierarchy();
            container.Add(ve);

            CommentAnchor anchor = container.Add<CommentAnchor>();
            anchor.target = ve;
            return anchor;
        }

        public void Open()
        {
            CommentPopup popup = this.GetFirstAncestorOfType<HomeworkContent>()?.commentPopup;
            if (popup != null)
                popup.Open(target);
        }
    }
}