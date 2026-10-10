using System;
using System.Collections.Generic;

using UnityEditor.PackageManager;

namespace Heaj.Homework
{
    [Serializable]
    public class MetaDataPack
    {
        public string timeStamp;
        public string packageVersion;
        public List<MetaData> entries = new List<MetaData>();

        public void Init()
        {
            entries.Clear();
            timeStamp = DateTime.Now.ToString();
        }

        public void Add(MetaData entry, bool overrideExisting)
        {
            for (int i = 0; i < entries.Count; i++)
            {
                if (entries[i].name == entry.name)
                {
                    if (overrideExisting)
                        entries[i] = entry;
                    return;
                }
            }

            entries.Add(entry);
        }

        public void CreateIfMissing(string name)
        {
            for (int i = 0; i < entries.Count; i++)
            {
                if (entries[i].name == name)
                {
                    return;
                }
            }

            entries.Add(new MetaData(name));
        }
    }
}