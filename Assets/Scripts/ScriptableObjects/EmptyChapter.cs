namespace SamsBackpack.Homework
{
    public class EmptyChapter : IOutlinerEntry
    {
        public string Title => title;
        public string title;

        public EmptyChapter(string title)
        {
            this.title = title;
        }
    }
}