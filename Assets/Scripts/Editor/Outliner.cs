using System;
using System.Collections.Generic;

using UnityEditor;
using UnityEditor.UIElements;

using UnityEngine;
using UnityEngine.UIElements;

namespace SamsBackpack.Homework
{

    public class Outliner : VisualElement
    {
        private TreeView treeView;
        private SerializedProperty property;
        private List<TreeViewItemData<IOutlinerEntry>> entries;

        public Action<IOutlinerEntry> onSelectEntry;

        public Outliner()
        {
            this.name = "outliner";
            this.AddToClassList("homework-outliner");

            Button rebuildBtn = this.Add<Button>();
            rebuildBtn.text = "Rebuild";
            rebuildBtn.clicked += Rebuild;

            entries = Database.Resources.outlinerData.BuildEntries();

            treeView = this.Add<TreeView>();
            treeView.SetRootItems(entries);
            treeView.makeItem += () => { return new OutlinerItem(); };
            treeView.bindItem += (VisualElement e, int index) => { ((OutlinerItem)e).BindProperty(treeView.GetItemDataForIndex<IOutlinerEntry>(index)); };
            treeView.selectionChanged += OnSelectionChanged;


            treeView.Rebuild();

            
        }

        private void OnSelectionChanged(IEnumerable<object> obj)
        {
            foreach (var item in obj)
            {
                if (item is IOutlinerEntry entry)
                {
                    onSelectEntry?.Invoke(entry);
                    return;
                }
                else
                {
                    Debug.Log(item.GetType());
                }
            }
        }

        private void Rebuild()
        {
            entries = Database.Resources.outlinerData.BuildEntries();
            treeView.SetRootItems(entries);
            treeView.Rebuild();
        }
    }

    public class OutlinerItem : VisualElement
    {
        private Label label;

        public OutlinerItem()
        {
            this.AddToClassList("homework-outliner-item");
            this.label = this.Add<Label>("label", "homework-outliner-item-label");
        }

        public void BindProperty(IOutlinerEntry entry)
        {
            //SerializedProperty prop = listProperty.GetArrayElementAtIndex(index);
            //SerializedObject obj = new SerializedObject(prop.objectReferenceValue);


            label.text = entry.Title;
        }

    }

}
