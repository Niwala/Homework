using System;

using UnityEngine;
using UnityEngine.UIElements;

namespace Heaj.Homework
{
    [CreateAssetMenu(menuName = "Exercice")]
    public class Exercice : ScriptableObject, IOutlinerEntry
    {
        public string Title => name;
        public VisualTreeAsset Article => article;
        public float Priority => entryPriority;

        public VisualTreeAsset article;

        [SerializeField]
        public Status status;
        [SerializeField]
        public float entryPriority = 0;


        [NonSerialized]
        public Shader userShader;
        public Shader solutionShader;
        public Shader startShader;

        [HideInInspector]
        public string startGuid;

        public enum Status
        {
            None,
            Invalid,
            Valid
        }
    }
}