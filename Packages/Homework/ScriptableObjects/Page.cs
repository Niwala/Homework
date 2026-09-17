using UnityEngine;
using UnityEngine.UIElements;

namespace SamsBackpack.Homework
{
    [CreateAssetMenu]
    public class Page : ScriptableObject, IOutlinerEntry
    {
        public string Title => name;
        public VisualTreeAsset Article => article;
        public float Priority => entryPriority;

        public float entryPriority = 0;
        public int FirstPagePriority => 2;

        public VisualTreeAsset article;

    }
}
