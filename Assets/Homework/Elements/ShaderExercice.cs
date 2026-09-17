using UnityEditor;
using UnityEditor.Graphs;
using UnityEditor.UIElements;

using UnityEngine;
using UnityEngine.UIElements;

namespace SamsBackpack.Homework
{

    [UxmlElement]
    public partial class ShaderExercice : VisualElement
    {
        [UxmlAttribute]
        public float widthWarpThreshold = 500;
        private int horizontal = 0;
        private const int shrinkMargin = 30;

        private Exercice exercice;
        private ShaderSolution shaderSolution;
        private ShaderUser shaderUser;
        private ShaderWatcher shaderWatcher;

        public ShaderExercice()
        {
            this.AddToClassList("homework-shader-exercice");
            shaderSolution = this.Add<ShaderSolution>();
            shaderUser = this.Add<ShaderUser>();
            shaderWatcher = this.Add<ShaderWatcher>();

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
        }

        private void OnGeometryChanged(GeometryChangedEvent e)
        {
            CheckSize();
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
                this.style.flexDirection = FlexDirection.Column;
                shaderSolution.style.flexDirection = FlexDirection.Column;
                shaderUser.style.flexDirection = FlexDirection.Column;
            }
            else if (height < width && horizontal != 1)
            {
                horizontal = 1;
                this.style.flexDirection = FlexDirection.Row;
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
        protected ShaderElement shaderElement;
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
