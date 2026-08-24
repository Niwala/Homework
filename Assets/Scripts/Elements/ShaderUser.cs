using UnityEditor;

using UnityEngine;
using UnityEngine.UIElements;

namespace SamsBackpack.Homework
{
    [UxmlElement]
    public partial class ShaderUser : ShaderExercicePart
    {
        private Exercice exercice;
        private bool loaded;

        protected override int GetButtonCount() => 2;

        protected override (Texture2D icon, string tooltip) GetButtonContent(int id)
        {
            if (id == 0)
                return (Database.Resources.editIcon, "Edit");
            else
                return (Database.Resources.resetIcon, "Reset");
        }

        protected override void OnButtonClicked(int id)
        {
            if (id == 0)    //Edit
            {
                if (!loaded)
                {
                    Loader.Load(exercice);
                    shaderElement.Shader = exercice.userShader;
                    ShaderExercice.SetButtonEnable(buttons[1], true);
                    loaded = true;
                }

                AssetDatabase.OpenAsset(exercice.userShader);
            }
            else            //Reset
            {
                Loader.Reset(exercice);
            }
        }

        public void Bind(Exercice exercice)
        {
            this.exercice = exercice;
            this.loaded = Loader.IsLoaded(exercice);

            if (loaded)
            {
                shaderElement.Shader = exercice.userShader;
            }
            else
            {
                shaderElement.Shader = exercice.startShader;
                ShaderExercice.SetButtonEnable(buttons[1], false);
            }
        }
    }
}
