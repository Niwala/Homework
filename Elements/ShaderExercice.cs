using UnityEditor;
using UnityEditor.Graphs;
using UnityEditor.UIElements;

using UnityEngine;
using UnityEngine.UIElements;

namespace Heaj.Homework
{

    [UxmlElement]
    public partial class ShaderExercice : VisualElement
    {
        [UxmlAttribute]
        public float widthWarpThreshold = 500;
        private int horizontal = 0;
        private const int shrinkMargin = 30;

        private Exercice exercice;
        public ShaderSolution shaderSolution;
        public ShaderUser shaderUser;
        private ShaderWatcher shaderWatcher;

        private VisualElement absRect;

        public ShaderExercice()
        {
            this.AddToClassList("homework-shader-exercice");
            shaderSolution = this.Add<ShaderSolution>();
            shaderUser = this.Add<ShaderUser>();
            shaderWatcher = this.Add<ShaderWatcher>();

            //absRect = this.Add("abs-rect");
            //absRect.style.position = Position.Absolute;

            this.RegisterCallback<AttachToPanelEvent>(OnAttachToParent);
            this.RegisterCallback<GeometryChangedEvent>(OnGeometryChanged);
        }

        public void Bind(Exercice exercice)
        {
            this.exercice = exercice;
            shaderUser.Bind(exercice);
            shaderSolution.Bind(exercice);
            shaderWatcher.Bind(exercice);
        }

        private void OnAttachToParent(AttachToPanelEvent e)
        {
            CheckSize();
            //AdaptAltContent();
        }

        private void OnGeometryChanged(GeometryChangedEvent e)
        {
            CheckSize();
            //AdaptAltContent();
        }

        //private void AdaptAltContent()
        //{
        //    //Adapt abs rect
        //    Rect solutionRect = this.WorldToLocal(shaderSolution[0].worldBound);
        //    Rect userRect = this.WorldToLocal(shaderUser[0].worldBound);
        //    float minX = Mathf.Min(solutionRect.xMin, userRect.xMin);
        //    float minY = Mathf.Min(solutionRect.yMin, userRect.yMin);
        //    float maxX = Mathf.Max(solutionRect.xMax, userRect.xMax);
        //    float maxY = Mathf.Max(solutionRect.yMax, userRect.yMax);
        //    Rect r = new Rect(minX, minY, maxX - minY, maxY - minY);

        //    absRect.style.left = r.x;
        //    absRect.style.top = r.y;
        //    absRect.style.width = r.width;
        //    absRect.style.height = r.height;
        //    absRect.style.backgroundColor = new Color(1, 0, 0, 0.1f);
        //}

        private void FindBestLayout()
        {
            Rect rect = contentRect;
            bool showProperties = (exercice == null) ? false : exercice.showProperties;

            float verticalFit = showProperties ? contentRect.height / 3.0f : contentRect.height / 2.0f;
            float horizontal = showProperties ? contentRect.width / 3.0f : contentRect.width / 2.0f;
            float gridFit = showProperties ? Mathf.Min(contentRect.width / 2.0f, contentRect.height / 2.0f) : (verticalFit + horizontal);

            if (verticalFit < horizontal && verticalFit < gridFit)
            {

            }
        }

        private void ApplyVerticalFit()
        {

        }

        private void ApplyHorizontalFit()
        {

        }

        private void ApplyGridFit()
        {

        }



        private void CheckSize()
        {
            if (parent == null)
                return;

            float width = parent.contentRect.width;
            if (float.IsNaN(width) || width == 0)
                return;
            float height = parent.contentRect.height;

            if (width < height && horizontal != 2)
            {
                horizontal = 2;
                style.flexGrow = 1;
                style.flexDirection = FlexDirection.Column;
                shaderSolution.style.flexDirection = FlexDirection.Column;
                shaderUser.style.flexDirection = FlexDirection.Column;
            }
            else if (height < width && horizontal != 1)
            {
                horizontal = 1;
                style.flexGrow = 0;
                style.flexDirection = FlexDirection.Row;
                shaderSolution.style.flexDirection = FlexDirection.Row;
                shaderUser.style.flexDirection = FlexDirection.Row;
            }

            if (horizontal == 2)
            {
                shaderSolution.style.maxHeight = width - shrinkMargin;
                shaderUser.style.maxHeight = width - shrinkMargin;
            }
            else
            {
                shaderSolution.style.maxHeight = new StyleLength(StyleKeyword.Auto);
                shaderUser.style.maxHeight = new StyleLength(StyleKeyword.Auto);
            }
        }

        public static void SetButtonEnable(Button button, bool enable)
        {
            button.SetEnabled(enable);
            button.SetCursor(enable ? MouseCursor.Link : MouseCursor.NotAllowed);
        }
    }

    public abstract class ShaderExercicePart : VisualElement
    {
        public ShaderElement shaderElement;
        protected Button[] buttons;

        public ShaderExercicePart()
        {
            this.AddToClassList("homework-shader-container");

            //Container
            VisualElement frame = this.Add("container", "homework-shader-frame");
            shaderElement = frame.Add<ShaderElement>();

            //Controls
            VisualElement controlsContainer = frame.Add("controls-container", "homework-shader-controls-container");
            VisualElement controls = controlsContainer.Add<VisualElement>("controls", "homework-shader-controls");

            //Buttons
            buttons = new Button[GetButtonCount()];
            for (int i = 0; i < buttons.Length; i++)
            {
                int j = i;
                buttons[i] = controls.Add<Button>("shader-btn", "homework-shader-btn");
                (Texture2D icon, string tooltip) = GetButtonContent(i);
                buttons[i].tooltip = tooltip;
                VisualElement iconElement = buttons[i].Add("icon", "homework-shader-btn-icon");
                iconElement.style.backgroundImage = icon;
                iconElement.pickingMode = PickingMode.Ignore;
                buttons[i].clicked += () => OnButtonClicked(j);
            }

            //Hooks
            this.RegisterCallback<DetachFromPanelEvent>(OnDetachFromPanel);
        }

        protected abstract int GetButtonCount();

        protected abstract (Texture2D icon, string tooltip) GetButtonContent(int id);

        protected abstract void OnButtonClicked(int id);

        protected virtual void OnDetachFromPanel(DetachFromPanelEvent e) { }
    }
}
