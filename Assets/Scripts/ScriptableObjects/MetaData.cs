using System;

namespace SamsBackpack.Homework
{
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
            status = Status.Unchecked;
        }
    }
}