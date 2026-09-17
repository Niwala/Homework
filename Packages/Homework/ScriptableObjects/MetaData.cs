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

        public Status GetLockableStatus()
        {
            if (status != Status.Limited)
                return status;

            if (DateTime.TryParse(limitedTime, out DateTime lockTime))
            {
                TimeSpan remaining = lockTime - DateTime.Now;
                if (remaining.TotalSeconds > 0)
                    return Status.Limited;
                else 
                    return Status.Locked;
            }
            return Status.Locked;
        }
    }
}