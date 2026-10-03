using System;

using UnityEngine;
using UnityEngine.UIElements;

using UnityEditor;

namespace Heaj.Homework
{
    public class DatePopup : EditorWindow
    {
        public IntegerField year;
        public IntegerField month;
        public IntegerField day;
        public IntegerField hour;
        public IntegerField minute;

        private Button button;
        private Action<string> onGetDate;

        public static void Open(Action<string> onGetDate)
        {
            DatePopup window = EditorWindow.CreateInstance<DatePopup>();
            window.onGetDate = onGetDate;
            window.titleContent = new GUIContent("Select end date");
            window.minSize = window.maxSize = new Vector2(250, 90);
            window.ShowModalUtility();
        }

        private void OnEnable()
        {
            rootVisualElement.styleSheets.Add(Database.Resources.styles);
            rootVisualElement.style.paddingBottom = rootVisualElement.style.paddingRight =
                rootVisualElement.style.paddingLeft = rootVisualElement.style.paddingTop = 10;

            //Current date
            DateTime date = DateTime.Now;

            //
            VisualElement h = rootVisualElement.Add("horizontal", "homework-date");
            h.Add<Label>("date", "homework-date-label").text = "Date";

            day = h.Add<IntegerField>("day", "homework-date-field");
            day.value = date.Day;
            h.Add<Label>("", "homework-date-symbol").text = "/";
            month = h.Add<IntegerField>("month", "homework-date-field");
            month.value = date.Month;
            h.Add<Label>("", "homework-date-symbol").text = "/";
            year = h.Add<IntegerField>("year", "homework-date-field");
            year.value = date.Year;

            //
            h = rootVisualElement.Add("horizontal", "homework-date");
            h.Add<Label>("hour", "homework-date-label").text = "Hour";
            hour = h.Add<IntegerField>("hour", "homework-date-field");
            hour.value = date.Hour;
            h.Add<Label>("", "homework-date-symbol").text = ":";
            minute = h.Add<IntegerField>("minute", "homework-date-field");
            minute.value = date.Minute;

            //
            button = this.rootVisualElement.Add<Button>();
            button.text = "Apply";
            button.clicked += Apply;
        }

        private void Apply()
        {
            DateTime date = new DateTime(year.value, month.value, day.value, hour.value, minute.value, 0);
            onGetDate(date.ToString());
            Close();
        }
    }
}
