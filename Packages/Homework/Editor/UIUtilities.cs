using System.Reflection;

using UnityEngine;
using UnityEngine.UIElements;

using UnityEditor;
using UnityEditor.UIElements;

using Cursor = UnityEngine.UIElements.Cursor;

namespace SamsBackpack.Homework
{
    public static class ToolkitUIExtensions
    {
        public static VisualElement Add(this VisualElement parent, string name)
        {
            return parent.Add<VisualElement>(name);
        }

        public static VisualElement Add(this VisualElement.Hierarchy parent, string name)
        {
            return parent.Add<VisualElement>(name);
        }

        public static VisualElement Add(this VisualElement parent, string name, params string[] classList)
        {
            return parent.Add<VisualElement>(name, classList);
        }

        public static VisualElement Add(this VisualElement.Hierarchy parent, string name, params string[] classList)
        {
            return parent.Add<VisualElement>(name, classList);
        }

        public static T Add<T>(this VisualElement parent) where T : VisualElement, new()
        {
            T element = new T();
            parent.Add(element);
            return element;
        }

        public static T Add<T>(this VisualElement.Hierarchy parent) where T : VisualElement, new()
        {
            T element = new T();
            parent.Add(element);
            return element;
        }

        public static T Add<T>(this VisualElement parent, string name, params string[] classList) where T : VisualElement, new()
        {
            T element = new T { name = name };
            for (int i = 0; i < classList.Length; i++)
                element.AddToClassList(classList[i]);
            parent.Add(element);
            return element;
        }

        public static T Add<T>(this VisualElement.Hierarchy parent, string name, params string[] classList) where T : VisualElement, new()
        {
            T element = new T { name = name };
            for (int i = 0; i < classList.Length; i++)
                element.AddToClassList(classList[i]);
            parent.Add(element);
            return element;
        }

        public static T Insert<T>(this VisualElement parent, int index) where T : VisualElement, new()
        {
            T element = new T();
            parent.Insert(index, element);
            return element;
        }

        public static T Insert<T>(this VisualElement parent, int index, string name) where T : VisualElement, new()
        {
            T element = new T { name = name };
            parent.Insert(index, element);
            return element;
        }

        public static T Insert<T>(this VisualElement parent, int index, string name, params string[] classList) where T : VisualElement, new()
        {
            T element = new T { name = name };
            for (int i = 0; i < classList.Length; i++)
                element.AddToClassList(classList[i]);
            parent.Insert(index, element);
            return element;
        }

        public static T Insert<T>(this VisualElement.Hierarchy parent, int index) where T : VisualElement, new()
        {
            T element = new T();
            parent.Insert(index, element);
            return element;
        }

        public static T Insert<T>(this VisualElement.Hierarchy parent, int index, string name) where T : VisualElement, new()
        {
            T element = new T { name = name };
            parent.Insert(index, element);
            return element;
        }

        public static T Insert<T>(this VisualElement.Hierarchy parent, int index, string name, params string[] classList) where T : VisualElement, new()
        {
            T element = new T { name = name };
            for (int i = 0; i < classList.Length; i++)
                element.AddToClassList(classList[i]);
            parent.Insert(index, element);
            return element;
        }

        public static PropertyField AddProperty(this VisualElement parent, SerializedProperty parentProperty, string name, params string[] classList)
        {
            SerializedProperty prop = parentProperty.FindPropertyRelative(name);
            PropertyField field = new PropertyField(prop);
            for (int i = 0; i < classList.Length; i++)
                field.AddToClassList(classList[i]);
            parent.Add(field);
            return field;
        }

        public static PropertyField AddProperty(this VisualElement.Hierarchy parent, SerializedProperty parentProperty, string name, params string[] classList)
        {
            SerializedProperty prop = parentProperty.FindPropertyRelative(name);
            PropertyField field = new PropertyField(prop);
            for (int i = 0; i < classList.Length; i++)
                field.AddToClassList(classList[i]);
            parent.Add(field);
            return field;
        }

        public static PropertyField AddProperty(this VisualElement parent, SerializedObject parentObject, string name, params string[] classList)
        {
            SerializedProperty prop = parentObject.FindProperty(name);
            PropertyField field = new PropertyField(prop);
            for (int i = 0; i < classList.Length; i++)
                field.AddToClassList(classList[i]);
            parent.Add(field);
            return field;
        }

        public static PropertyField AddProperty(this VisualElement.Hierarchy parent, SerializedObject parentObject, string name, params string[] classList)
        {
            SerializedProperty prop = parentObject.FindProperty(name);
            PropertyField field = new PropertyField(prop);
            for (int i = 0; i < classList.Length; i++)
                field.AddToClassList(classList[i]);
            parent.Add(field);
            return field;
        }

        public static VisualElement GetRoot(this VisualElement element)
        {
            while (element.parent != null)
                element = element.parent;
            return element;
        }

        public static T GetParent<T>(this VisualElement element) where T : VisualElement
        {
            if (element is T t)
                return t;
            if (element.parent == null)
                return null;
            return GetParent<T>(element.parent);
        }

        public static T GetParent<T>(this VisualElement element, string name) where T : VisualElement
        {
            if (element is T t && element.name == name)
                return t;
            if (element.parent == null)
                return null;
            return GetParent<T>(element.parent, name);
        }

        public static VisualElement GetParent(this VisualElement element, string name)
        {
            if (element.name == name)
                return element;
            if (element.parent == null)
                return null;
            return GetParent(element, name);
        }

        public static void SetCursor(this VisualElement element, MouseCursor cursor)
        {
            object objCursor = new Cursor();
            PropertyInfo fields = typeof(Cursor).GetProperty("defaultCursorId", BindingFlags.NonPublic | BindingFlags.Instance);
            fields.SetValue(objCursor, (int)cursor);
            element.style.cursor = new StyleCursor((Cursor)objCursor);
        }

        public static void SetDisplay(this VisualElement element, bool flex)
        {
            element.style.display = flex ? DisplayStyle.Flex : DisplayStyle.None;
        }
    }
}