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

            //Push button
            ToolbarButton pushBtn = toolbar.Add<ToolbarButton>();
            pushBtn.AddManipulator(new EditModeManipulator());
            pushBtn.text = "Push content";
            pushBtn.clicked += PushContent;

            //Export button
            ToolbarButton exportBtn = toolbar.Add<ToolbarButton>();
            exportBtn.AddManipulator(new EditModeManipulator());
            exportBtn.text = "Export metadata";
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

        private void PushContent()
        {
            PushPopup.Open(OnReceiveMessage);

            async void OnReceiveMessage(string gitMessage)
            {
                (bool success, string newPackageVersion) = await Package.PushPackageToGit(gitMessage);

                if (success)
                    Debug.Log($"Content pushed (version : {newPackageVersion})");
                else
                    Debug.LogError($"Push failed (version : {newPackageVersion})");
            }
        }

        private async void Export()
        {
            PasswordPopup.Open(OnReceivePassword);

            async void OnReceivePassword(string password)
            {
                if (string.IsNullOrEmpty(password))
                    return;

                UserData.Metadata.timeStamp = DateTime.Now.ToString();
                UserData.Metadata.packageVersion = await Package.GetPackageVersion();
                (bool error, string errorMsg) = await Updater.UpdateFlagsAsync(password, UserData.Metadata);

                if (error)
                    Debug.LogError(errorMsg);
                else
                    Debug.Log($"Meta data exported (version : {UserData.Metadata.packageVersion})");
            }
        }
    }
}
