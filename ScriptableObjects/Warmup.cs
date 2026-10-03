using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UIElements;

namespace Heaj.Homework
{
    [CreateAssetMenu(menuName = "Warmup")]
    public class Warmup : ScriptableObject, IOutlinerEntry
    {
        public string Title => name;
        public float Priority => entryPriority;
        public VisualTreeAsset Article => article;

        public VisualTreeAsset article;
        public float entryPriority = 0;
        public Shader placeHolder;

        public List<ShaderPair> shaders = new List<ShaderPair>();
    }

    [Serializable]
    public struct ShaderPair
    {
        public Shader solution;
        public Shader defaultState;
        public Shader userShader;
    }

}