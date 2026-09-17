using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using UnityEditor;

using UnityEngine;
using UnityEngine.UIElements;

namespace SamsBackpack.Homework
{

    public class OutlinerData : ScriptableObject
    {
        public VisualTreeAsset chapterControls;

        public List<TreeViewItemData<IOutlinerEntry>> BuildEntries()
        {
            string localPath = GetLocalPath();
            int id = 0;
            List<TreeViewItemData<IOutlinerEntry>> entries = new List<TreeViewItemData<IOutlinerEntry>>();
            foreach (var dir in Directory.GetDirectories(localPath))
            {
                if (GetEntries(dir, ref id, out TreeViewItemData<IOutlinerEntry> subEntries))
                {
                    entries.Add(subEntries);
                    id++;
                }
            }

            //Keep metadata up-to-date
            foreach (var entry in entries)
            {
                UserData.Metadata.CreateIfMissing(entry.data.Title);
            }

            return entries;
        }

        private bool GetEntries(string directory, ref int id, out TreeViewItemData<IOutlinerEntry> entries)
        {
            IOutlinerEntry firstEntry = null;
            int firstPagePriority = 0;
            List<TreeViewItemData<IOutlinerEntry>> content = new List<TreeViewItemData<IOutlinerEntry>>();

            //Get files
            foreach (var file in Directory.GetFiles(directory, "*.asset"))
            {
                ScriptableObject obj = AssetDatabase.LoadAssetAtPath<ScriptableObject>(file);

                if (!(obj is IOutlinerEntry entry))
                    continue;


                if (obj is Exercice ex)
                {
                    MarkReadOnly(ex.solutionShader, true);
                    MarkReadOnly(ex.startShader, true);
                    MarkReadOnly(ex.userShader, false);
                }
                else if (!State.InEditMode)
                {
                    MetaData metadata = UserData.GetMetaData(entry);
                    if (metadata.status == Status.Hidden || metadata.status == Status.Unchecked)
                    {
                        entries = default;
                        return false;
                    }
                }

                if (firstEntry == null || entry.FirstPagePriority > firstPagePriority)
                {
                    //Replace previous > Add previous as a subpage
                    if (firstEntry != null)
                    {
                        content.Add(new TreeViewItemData<IOutlinerEntry>(id, firstEntry));
                        id++;
                    }

                    //Set as new first page (folder displayed page)
                    firstEntry = entry;
                    firstPagePriority = entry.FirstPagePriority;
                }

                //Add as subpage
                else
                {
                    content.Add(new TreeViewItemData<IOutlinerEntry>(id, entry));
                    id++;
                }
            }

            //Get sub directories
            if (!(firstEntry is Warmup))
            {
                foreach (var dir in Directory.GetDirectories(directory))
                {
                    if (GetEntries(dir, ref id, out TreeViewItemData<IOutlinerEntry> subEntries))
                    {
                        content.Add(subEntries);
                        id++;
                    }
                }
            }

            //Chapter decorator
            if (firstEntry == null)
            {
                firstEntry = new EmptyChapter(Path.GetFileName(directory));
            }

            entries = new TreeViewItemData<IOutlinerEntry>(id, firstEntry, content);
            return true;
        }

        public void ExportMetaData()
        {
            PasswordPopup.Open(OnReceivePassword);

            async void OnReceivePassword(string password)
            {
                if (string.IsNullOrEmpty(password))
                    return;

                UserData.Metadata.timeStamp = DateTime.Now.ToString();
                (bool error, string errorMsg) = await Updater.UpdateFlagsAsync(password, UserData.Metadata);

                if (error)
                    Debug.LogError(errorMsg);
                else
                    Debug.Log("Meta data exported");
            }
        }

        private void MarkReadOnly(UnityEngine.Object obj, bool readOnly)
        {
            if (obj == null)
                return;

            string path = AssetDatabase.GetAssetPath(obj);
            if (string.IsNullOrEmpty(path))
                return;

            FileInfo fileInfo = new FileInfo(path);
            if (fileInfo == null)
                return;

            fileInfo.IsReadOnly = readOnly;
        }

        public string GetLocalPath()
        {
            return Path.GetDirectoryName(AssetDatabase.GetAssetPath(this));
        }

        public void Export(Chapter chapter)
        {
            AssetDatabase.SaveAssets();
            UserData userData = UserData.Current;
            UserData.SerializedObject.ApplyModifiedProperties();

            ExportData exportData = new ExportData()
            {
                surname = userData.userSurname,
                name = userData.userName,
                content = chapter.Title,
            };

            string exportName = $"{chapter.Title}_{CleanName(userData.userSurname).ToUpper()}_{CleanName(userData.userName)}";
            Exporter.Export(exportData, "", exportName);
        }

        private static string CleanName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return "";

            char[] invalidChars = Path.GetInvalidFileNameChars();
            StringBuilder builder = new StringBuilder(name.Length);

            foreach (char c in name)
            {
                if (Array.IndexOf(invalidChars, c) < 0)
                {
                    builder.Append(c);
                }
            }

            string cleanedName = string.Join(" ", builder.ToString().Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries));
            return cleanedName.Length > 0 ? cleanedName : "Unknown";
        }
    }
}