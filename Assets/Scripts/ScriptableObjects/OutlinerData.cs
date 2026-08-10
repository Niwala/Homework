using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

using UnityEditor;

using UnityEngine;
using UnityEngine.UIElements;

namespace SamsBackpack.Homework
{
    public class OutlinerData : ScriptableObject
    {
        public VisualTreeAsset chapterExport;
        public string userName;
        public string userSurname;

        public List<TreeViewItemData<IOutlinerEntry>> BuildEntries()
        {
            string localPath = GetLocalPath();
            int id = 0;
            List<TreeViewItemData<IOutlinerEntry>> entries = new List<TreeViewItemData<IOutlinerEntry>>();
            foreach (var dir in Directory.GetDirectories(localPath))
            {
                entries.Add(GetEntries(dir, ref id));
                id++;
            }
            return entries;
        }

        private TreeViewItemData<IOutlinerEntry> GetEntries(string directory, ref int id)
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
                    MarkReadOnly(ex.solution, true);
                    MarkReadOnly(ex.defaultState, true);
                    MarkReadOnly(ex.userShader, false);
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
                    content.Add(GetEntries(dir, ref id));
                    id++;
                }
            }

            //Chapter decorator
            if (firstEntry == null)
            {
                firstEntry = new EmptyChapter(Path.GetFileName(directory));
            }

            return new TreeViewItemData<IOutlinerEntry>(id, firstEntry, content);
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

            ExportData exportData = new ExportData()
            {
                surname = userSurname,
                name = userName,
                content = chapter.Title,
            };

            string exportName = $"{chapter.Title}_{CleanName(userSurname).ToUpper()}_{CleanName(userName)}";
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