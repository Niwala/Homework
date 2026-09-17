using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UIElements;

namespace SamsBackpack.Homework
{
    public partial class HomeworkResources
    {
        public StyleSheet styles;
        public OutlinerData outlinerData;
        public ComputeShader warmupChecker;
        public List<Texture2D> icons;

        public Texture2D showIcon;
        public Texture2D editIcon;
        public Texture2D resetIcon;
        public Texture2D timeIcon;
        public Texture2D loaderIcon;
        public Texture2D validIcon;
        public Texture2D warningIcon;
        public Texture2D commentIcon;
        public Texture2D lockIcon;
        public TextAsset transientShaderTemplate;
    }
}