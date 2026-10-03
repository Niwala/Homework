using System.Threading;

using UnityEditor;

using UnityEngine;
using UnityEngine.UIElements;

namespace Heaj.Homework
{

    [UxmlElement]
    public partial class ShaderSolution : ShaderExercicePart
    {
        private Exercice exercice;

        protected override int GetButtonCount() => 1;

        protected override (Texture2D icon, string tooltip) GetButtonContent(int id)
        {
            return (Database.Resources.showIcon, "Show");
        }

        protected override void OnButtonClicked(int id)
        {
            AssetDatabase.OpenAsset(exercice.solutionShader);
        }

        public void Bind(Exercice exercice)
        {
            this.exercice = exercice;
            this.shaderElement.Shader = exercice.solutionShader;
            try
            {
                var _ = Loader.ApplyAutoLock(exercice, buttons[0]);
            }
            catch { }
        }
    }
}