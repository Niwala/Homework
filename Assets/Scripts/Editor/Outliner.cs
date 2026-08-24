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

            //Toolbar
            Toolbar toolbar = this.Add<Toolbar>();

            //Rebuild button
            ToolbarButton rebuildBtn = toolbar.Add<ToolbarButton>();
            rebuildBtn.text = "Rebuild";
            rebuildBtn.clicked += Rebuild;

            //Import button
            ToolbarButton importBtn = toolbar.Add<ToolbarButton>();
            importBtn.text = "Import";
            importBtn.clicked += Import;

            //Export button
            ToolbarButton exportBtn = toolbar.Add<ToolbarButton>();
            exportBtn.text = "Export";
            exportBtn.clicked += Export;

            entries = Database.Resources.outlinerData.BuildEntries();

            treeView = this.Add<TreeView>();
            treeView.SetRootItems(entries);
            treeView.makeItem += () => { return new OutlinerItem(); };
            treeView.bindItem += (VisualElement e, int index) => { ((OutlinerItem)e).BindProperty(treeView.GetItemDataForIndex<IOutlinerEntry>(index)); };
            treeView.selectionChanged += OnSelectionChanged;

            treeView.Rebuild();
            treeView.AddToClassList("homework-outliner-tree-view");
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

        private void Import()
        {
            Database.Resources.outlinerData.ImportMetaData();
        }

        private void Export()
        {
            Database.Resources.outlinerData.ExportMetaData();
        }
    }

    public class OutlinerItem : VisualElement
    {
        private Label label;
        private VisualElement icon;

        public OutlinerItem()
        {
            this.AddToClassList("homework-outliner-item");
            this.label = this.Add<Label>("label", "homework-outliner-item-label");
            this.icon = this.Add("icon", "homework-outliner-item-icon");
        }

        public void BindProperty(IOutlinerEntry entry)
        {
            //Get status
            Status status = Status.Available;
            if (!(entry is Exercice))
            {
                MetaData metadata = OutlinerData.GetMetaData(entry);
                status = metadata.status;
            }

            bool hidden = status == Status.Hidden || status == Status.Unchecked || status == Status.Outdated;

            label.text = entry.Title;
            label.style.opacity = hidden ? 0.5f : 1.0f;
            label.SetCheckedPseudoState(entry is Page);

            switch (status)
            {
                case Status.Limited:
                    icon.SetDisplay(true);
                    icon.style.backgroundImage = Background.FromTexture2D(Database.Resources.timeIcon);
                    icon.tooltip = "L'accès à ce chapitre est limité dans le temps.";
                    break;

                case Status.Outdated:
                    icon.SetDisplay(true);
                    icon.style.backgroundImage = Background.FromTexture2D(Database.Resources.warningIcon);
                    icon.tooltip = "Ce chapitre n'est plus à jour.";
                    break;

                default:
                    icon.SetDisplay(false);
                    break;
            }
        }
    }
}
