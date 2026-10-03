using System;

namespace Heaj.Homework
{
    public class EmptyChapter : IOutlinerEntry
    {
        public string Title => title;
        public float Priority => entryPriority;

        public string title;
        public float entryPriority = 0;

        public EmptyChapter(string title)
        {
            this.title = title;
        }
    }
}