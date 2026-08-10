using UnityEditor;

using UnityEngine.UIElements;

namespace SamsBackpack.Homework
{
    public class HomeworkContent : VisualElement
    {
        public IOutlinerEntry Entry { get; private set; }

        public void Show(IOutlinerEntry entry)
        {
            Entry = entry;

            Clear();

            //Split
            TwoPaneSplitView split = new TwoPaneSplitView(0, 600, TwoPaneSplitViewOrientation.Horizontal);
            this.Add(split);

            ScrollView articleArea = split.Add<ScrollView>("article-area", "homework-article-area");
            ScrollView shaderArea = split.Add<ScrollView>("shader-area", "homework-shader-area");

            //Exercices
            if (entry is Exercice exercice)
            {
                ShaderExercice shaderExercice = shaderArea.Add<ShaderExercice>();
                shaderExercice.Bind(exercice);
            }

            //Article
            if (entry.Article != null)
            {
                articleArea.Add(entry.Article.CloneTree());
            }

            //Chapter
            if (entry is Chapter chapter)
            {
                OutlinerData outlinerData = Database.Resources.outlinerData;
                VisualElement chapterExportSection = outlinerData.chapterExport.CloneTree();
                ChapterControls controls = chapterExportSection.Q<ChapterControls>();
                controls.Bind(outlinerData, chapter);
                shaderArea.Add(chapterExportSection);
            }

        }



    }

}
