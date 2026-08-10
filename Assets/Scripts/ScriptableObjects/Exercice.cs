using UnityEngine;
using UnityEngine.UIElements;

namespace SamsBackpack.Homework
{
    [CreateAssetMenu(menuName = "Exercice")]
    public class Exercice : ScriptableObject, IOutlinerEntry
    {
        public string Title => name;
        public VisualTreeAsset Article => article;


        public VisualTreeAsset article;

        [SerializeField]
        public Status status;

        public Shader solution;
        public Shader defaultState;
        public Shader userShader;

        public enum Status
        {
            None,
            Invalid,
            Valid
        }
    }
}