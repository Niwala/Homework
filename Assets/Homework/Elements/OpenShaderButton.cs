using UnityEditor;

using UnityEngine;
using UnityEngine.UIElements;

namespace SamsBackpack.Homework
{
    [UxmlElement]
    [Icon("UIToolkit/Icons/VisualElement.png")]
    public partial class OpenShaderButton : Button
    {
        [UxmlAttribute]
        public Shader shader;

        public OpenShaderButton()
        {
            this.AddToClassList("homework-shader-btn");
            this.tooltip = "Edit shader";
            VisualElement iconElement = this.Add("icon", "homework-shader-btn-icon");
            iconElement.style.backgroundImage = Database.Resources.editIcon;
            iconElement.pickingMode = PickingMode.Ignore;
            this.clicked += () => AssetDatabase.OpenAsset(shader);
        }
    }
}
