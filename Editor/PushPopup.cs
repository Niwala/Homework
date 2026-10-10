using System;

using UnityEngine;
using UnityEngine.UIElements;

using UnityEditor;

namespace Heaj.Homework
{
    public class PushPopup : EditorWindow
    {
        private TextField textField;
        private Button button;
        private Action<string> onGetPushDescription;

        public static void Open(Action<string> onGetPassword)
        {
            PushPopup window = EditorWindow.CreateInstance<PushPopup>();
            window.onGetPushDescription = onGetPassword;
            window.titleContent = new GUIContent("Push content");
            window.minSize = window.maxSize = new Vector2(250, 220);
            window.ShowModalUtility();
        }

        private void OnEnable()
        {
            rootVisualElement.style.paddingBottom = rootVisualElement.style.paddingRight =
                rootVisualElement.style.paddingLeft = rootVisualElement.style.paddingTop = 10;

            TextElement description = rootVisualElement.Add<TextElement>();
            description.text = "Push description";
            description.style.marginLeft = 5;
            description.style.marginBottom = 5;

            textField = this.rootVisualElement.Add<TextField>();
            textField.multiline = true;
            textField.style.height = 150;
            textField.style.marginBottom = 5;

            button = this.rootVisualElement.Add<Button>();
            button.text = "Push to git";
            button.clicked += Send;
        }

        private void Send()
        {
            onGetPushDescription(textField.text);
            Close();
        }
    }
}