using UnityEngine;
using UnityEngine.UIElements;


namespace Heaj.Homework
{
    public interface IOutlinerEntry
    {
        public string Title { get; }

        public float Priority { get; }

        public virtual VisualTreeAsset Article => null;

        /// <summary>
        /// A higher value causes the object to be used as the display for the parent folder.
        /// </summary>
        public virtual int FirstPagePriority => 0;
    }
}