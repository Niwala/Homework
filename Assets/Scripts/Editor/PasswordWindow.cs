using System;

using UnityEngine;
using UnityEngine.UIElements;

using UnityEditor;

namespace SamsBackpack.Homework
{
    public class PasswordWindow : EditorWindow
    {
        private TextField textField;
        private Button button;
        private Action<string> onGetPassword;

        public static void Open(Action<string> onGetPassword)
        {
            PasswordWindow window = EditorWindow.CreateInstance<PasswordWindow>();
            window.onGetPassword = onGetPassword;
            window.titleContent = new GUIContent("Export password");
            window.minSize = window.maxSize = new Vector2(250, 60);
            window.ShowModalUtility();
        }

        private void OnEnable()
        {
            rootVisualElement.style.paddingBottom = rootVisualElement.style.paddingRight =
                rootVisualElement.style.paddingLeft = rootVisualElement.style.paddingTop = 10;

            textField = this.rootVisualElement.Add<TextField>();
            textField.isPasswordField = true;

            button = this.rootVisualElement.Add<Button>();
            button.text = "Send";
            button.clicked += Send;
        }

        private void Send()
        {
            onGetPassword(textField.text);
            Close();
        }
    }
}
