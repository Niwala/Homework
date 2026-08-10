using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;

namespace SamsBackpack.Homework
{

    public class HomeworkWindow : EditorWindow
    {
        private Outliner outliner;
        private HomeworkContent content;

        [MenuItem("Window/Homework")]
        public static void Open()
        {
            HomeworkWindow window = EditorWindow.GetWindow<HomeworkWindow>();
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
            TwoPaneSplitView splitView = new TwoPaneSplitView(0, 200, TwoPaneSplitViewOrientation.Horizontal);
            rootVisualElement.Add(splitView);

            //Outliner
            outliner = splitView.Add<Outliner>();

            //Content
            content = splitView.Add<HomeworkContent>();



            //Events
            outliner.onSelectEntry += content.Show;
        }
    }
}
