using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        public VisualTreeAsset chapterExport;
        public string userName;
        public string userSurname;
        public MetaDataPack metaDataPack;

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

        private MetaDataPack BuildMetaDataFromAssets()
        {
            MetaDataPack metadata = new MetaDataPack();
            metadata.Init();
            foreach (var entry in BuildEntries())
            {
                metadata.Add(entry.data.Title);
            }
            return metadata;
        }

        public async void ImportMetaData()
        {
            //Internet read
            Task<MetaDataPack> metaDataRead = MetaCheck.Read();
            MetaDataPack metaDataPack = await metaDataRead;
            if (metaDataPack == null)
            {
                EditorUtility.DisplayDialog("Homework", "Unable to load the latest data. The previous version will be used instead.\n\nPlease check your internet connection.", "Ok");

                //Local read
                metaDataPack = UserData.Current.ReadMetaData();
            }
            else
            {
                //Update local
                UserData.Current.WriteMetaData(metaDataPack);
            }

            this.metaDataPack = metaDataPack;
        }

        public static MetaData GetMetaData(IOutlinerEntry entry)
        {
            MetaData data = Database.Resources.outlinerData.metaDataPack.metadata.FirstOrDefault(x => x.name == entry.Title);
            if (data == null)
                data = new MetaData(entry.Title);
            return data;
        }

        public async void ExportMetaData()
        {
            PasswordWindow.Open(OnReceivePassword);

            async void OnReceivePassword(string password)
            {
                if (string.IsNullOrEmpty(password))
                    return;

                if (string.IsNullOrEmpty(metaDataPack.timeStamp))
                    metaDataPack = BuildMetaDataFromAssets();

                (bool error, string errorMsg) = await MetaCheck.UpdateFlagsAsync(password, metaDataPack);

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

    [Serializable]
    public class MetaDataPack
    {
        public string timeStamp;
        public List<MetaData> metadata = new List<MetaData>();

        public void Init()
        {
            metadata.Clear();
            timeStamp = DateTime.Now.ToString();
        }

        public void Add(string name)
        {
            metadata.Add(new MetaData(name));
        }
    }

    [Serializable]
    public class MetaData
    {
        public string name;
        public string comment;
        public Status status;
        public string limitedTime;

        public MetaData(string name)
        {
            this.name = name;
            status = Status.Unckecked;
        }
    }

    public enum Status
    {
        Unckecked,
        Hidden,
        Available,
        Limited,
        Outdated
    }
}