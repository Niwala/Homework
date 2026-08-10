using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UIElements;

namespace SamsBackpack.Homework
{
    [CreateAssetMenu]
    public class Chapter : ScriptableObject, IOutlinerEntry
    {
        public string Title => name;
        public VisualTreeAsset Article => article;
        public int FirstPagePriority => 1;

        public VisualTreeAsset article;

    }
}
