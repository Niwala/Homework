using System;
using System.Collections.Generic;

namespace SamsBackpack.Homework
{
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
}