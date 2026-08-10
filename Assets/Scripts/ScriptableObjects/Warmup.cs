using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UIElements;

namespace SamsBackpack.Homework
{
    [CreateAssetMenu(menuName = "Warmup")]
    public class Warmup : ScriptableObject, IOutlinerEntry
    {
        public string Title => name;
        public VisualTreeAsset Article => article;
        public VisualTreeAsset article;
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