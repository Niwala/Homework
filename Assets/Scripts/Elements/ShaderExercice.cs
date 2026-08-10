using UnityEditor;

using UnityEngine;
using UnityEngine.UIElements;

namespace SamsBackpack.Homework
{

    [UxmlElement]
    public partial class ShaderExercice : VisualElement
    {

        private Exercice exercice;
        private ShaderElement solution;
        private ShaderElement user;

        public ShaderExercice()
        {
            this.AddToClassList("homework-shader-exercice");
            VisualElement h = this.Add<VisualElement>();
            h.style.flexDirection = FlexDirection.Row;

            //Solution
            VisualElement solutionColumn = h.Add("solution");
            solutionColumn.style.alignItems = Align.Center;
            solutionColumn.style.width = new Length(50, LengthUnit.Percent);
            solution = solutionColumn.Add<ShaderElement>();

            Button showSolutionBtn = solutionColumn.Add<Button>("show-btn", "homework-shader-exercice-btn");
            showSolutionBtn.text = "Show solution";
            showSolutionBtn.clicked += OpenSolution;


            //User
            VisualElement userColumn = h.Add("user");
            userColumn.style.alignItems = Align.Center;
            userColumn.style.width = new Length(50, LengthUnit.Percent);
            user = userColumn.Add<ShaderElement>();

            Button editShaderBtn = userColumn.Add<Button>("edit-btn", "homework-shader-exercice-btn");
            editShaderBtn.text = "Edit shader";
            editShaderBtn.clicked += OpenUserShader;
        }

        public void Bind(Exercice exercice)
        {
            this.exercice = exercice;

            solution.shader = exercice.solution;
            solution.Refresh();

            user.shader = exercice.userShader;
            user.Refresh();
        }

        private void OpenSolution()
        {
            AssetDatabase.OpenAsset(exercice.solution);
        }

        private void OpenUserShader()
        {
            AssetDatabase.OpenAsset(exercice.userShader);
        }

    }
}
