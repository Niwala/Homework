using System;
using System.Collections.Generic;
using System.IO;

using UnityEngine;

namespace Heaj.Homework
{
    [Serializable]
    public class HomeworkFile
    {
        public string name;
        public string location;


        public HomeworkFile(string location)
        {
            this.location = location;
            this.name = Path.GetFileNameWithoutExtension(location);
        }

    }
}
