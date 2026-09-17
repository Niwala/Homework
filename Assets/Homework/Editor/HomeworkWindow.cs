using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;

namespace SamsBackpack.Homework
{
    public class HomeworkWindow : EditorWindow
    {
        private OutlinerView outliner;
        private HomeworkContent content;

        [MenuItem("Window/Homework")]
        public static void Open()
        {
            HomeworkWindow window = EditorWindow.GetWindow<HomeworkWindow>();
            window.titleContent = new GUIContent("Homework");
            window.Show();
        }


        private void OnEnable()
        {
            //Data
            OutlinerData data = Database.Resources.outlinerData;
            SerializedObject so = new SerializedObject(data);

            //Styles
            rootVisualElement.styleSheets.Add(Database.Resources.styles);

            //Split
            float defaultOultinerWidth = EditorPrefs.GetFloat(UserData.prefPrefix + "outliner.width", 200);
            TwoPaneSplitView splitView = new TwoPaneSplitView(0, defaultOultinerWidth, TwoPaneSplitViewOrientation.Horizontal);
            rootVisualElement.Add(splitView);

            //Outliner
            outliner = splitView.Add<OutlinerView>();
            outliner.RegisterCallback<GeometryChangedEvent>(OnSplitViewValueChanged);

            //Content
            content = splitView.Add<HomeworkContent>();

            //Events
            outliner.onSelectEntry += content.Show;
        }

        private void OnSplitViewValueChanged(GeometryChangedEvent e)
        {
            float width = outliner.contentRect.width;
            if (float.IsNaN(width) || width == 0)
                return;

            EditorPrefs.SetFloat(UserData.prefPrefix + "outliner.width", width);
        }
    }
}
