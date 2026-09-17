using System.IO;

using UnityEngine;
using UnityEngine.UIElements;

namespace SamsBackpack.Homework
{
    [UxmlElement]
    public partial class ShaderWatcher : VisualElement
    {
        //private FileSystemWatcher watcher;

        public void Bind(Exercice exercice)
        {
            //Loader.GetPathInfo(exercice, out Loader.LoadInfo info);
            //string path = Path.GetDirectoryName(Path.GetFullPath(info.userPath));
            //watcher = new FileSystemWatcher(path);
            //watcher.Created += OnFileCreated;
            //watcher.Changed += OnFileChanged;
            //watcher.Deleted += OnFileDeleted;

            //this.RegisterCallback<DetachFromPanelEvent>(OnDetachFromPanel);
        }

        private void OnFileCreated(object sender, FileSystemEventArgs e)
        {
            Debug.Log("Created");
        }

        private void OnFileChanged(object sender, FileSystemEventArgs e)
        {
            Debug.Log("Changed");
        }

        private void OnFileDeleted(object sender, FileSystemEventArgs e)
        {
            Debug.Log("Deleted");
        }

        private void OnDetachFromPanel(DetachFromPanelEvent e)
        {
            //if (watcher != null)
            //{
            //    watcher.Dispose();
            //}
        }
    }
}