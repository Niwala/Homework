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

        public Texture2D commentIcon;
        public TextAsset transientShaderTemplate;
    }
}