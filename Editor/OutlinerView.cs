using System;
using System.Collections.Generic;

using UnityEditor;
using UnityEditor.UIElements;

using UnityEngine;
using UnityEngine.UIElements;

namespace Heaj.Homework
{
    public class OutlinerView : VisualElement
    {
        private TreeView treeView;
        private SerializedProperty property;
        private List<TreeViewItemData<IOutlinerEntry>> entries;

        public Action<IOutlinerEntry> onSelectEntry;

        public OutlinerView()
        {
            this.name = "outliner";
            this.AddToClassList("homework-outliner");

            //Toolbar
            Toolbar toolbar = this.Add<Toolbar>();

            //Refresh button
            ToolbarButton refreshButton = toolbar.Add<ToolbarButton>();
            refreshButton.text = "Refresh";
            refreshButton.clicked += State.Refresh;

            //Export button
            ToolbarButton exportBtn = toolbar.Add<ToolbarButton>();
            exportBtn.AddManipulator(new EditModeManipulator());
            exportBtn.text = "Export";
            exportBtn.clicked += Export;

            //Space
            toolbar.Add("space").style.flexGrow = 1;

            //Status
            HomeworkStatusElement status = toolbar.Add<HomeworkStatusElement>();

            entries = Database.Resources.outlinerData.BuildEntries();

            treeView = this.Add<TreeView>();
            treeView.SetRootItems(entries);
            treeView.makeItem += () => { return new OutlinerItem(); };
            treeView.bindItem += (VisualElement e, int index) => 
            {
                OutlinerItem item = (OutlinerItem)e;
                IOutlinerEntry entry = treeView.GetItemDataForIndex<IOutlinerEntry>(index);
                IOutlinerEntry parent = treeView.GetItemDataForId<IOutlinerEntry>(treeView.GetParentIdForIndex(index));
                item.BindProperty(entry, parent); 

            };
            treeView.selectionChanged += OnSelectionChanged;

            treeView.Rebuild();
            treeView.AddToClassList("homework-outliner-tree-view");


            State.onMetaDataChanged += RebuildEntries;
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


        private void RebuildEntries()
        {
            entries = Database.Resources.outlinerData.BuildEntries();
            treeView.SetRootItems(entries);
            treeView.Rebuild();
        }

        private void Export()
        {
            Database.Resources.outlinerData.ExportMetaData();
        }
    }
}
