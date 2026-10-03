using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

using UnityEditor;

using UnityEngine;

namespace Heaj.Homework
{
    public class TransientComputeShaders : ScriptableObject
    {
        public List<TransientComputeShaderElement> elements = new List<TransientComputeShaderElement>();


        private const string path = "Assets/TransientComputeShaders.tcs";

        public static TransientComputeShaders Current
        {
            get
            {
                if (current == null)
                    current = LoadExistingAsset();
                if (current == null)
                    current = CreateAsset();
                return current;
            }
        }
        private static TransientComputeShaders current;

        public static TransientComputeShaders LoadExistingAsset()
        {
            return AssetDatabase.LoadAssetAtPath<TransientComputeShaders>(path);
        }

        public static TransientComputeShaders CreateAsset()
        {
            TransientComputeShaderList list = new TransientComputeShaderList();
            File.WriteAllText(path, JsonUtility.ToJson(list, true));
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
            return AssetDatabase.LoadAssetAtPath<TransientComputeShaders>(path);
        }


        public static ComputeShader Get(string uniforms, string content)
        {
            if (string.IsNullOrEmpty(content))
                return null;

            TransientComputeShaders tcsAsset = Current;


            //Try to return existing corresponding shader
            uint key = HashString(uniforms + content);
            for (int i = 0; i < tcsAsset.elements.Count; i++)
            {
                if (tcsAsset.elements[i].key == key)
                {
                    //Still importing or uncompilable shader
                    if (tcsAsset.elements[i].shader == null)
                        return null;

                    return tcsAsset.elements[i].shader;
                }
            }


            //Create new shader for the content
            string path = AssetDatabase.GetAssetPath(tcsAsset);
            string file = File.ReadAllText(path);
            TransientComputeShaderList list = new TransientComputeShaderList();
            JsonUtility.FromJsonOverwrite(file, list);
            TransientComputeShaderElement tcs = new TransientComputeShaderElement()
            {
                key = key,
                uniforms = uniforms,
                content = content,
                shader = null,
            };
            list.elements.Add(tcs);
            File.WriteAllText(path, JsonUtility.ToJson(list, true));


            //Import the new shader
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);

            return null;
        }

        //Fnv1aHash
        public static uint HashString(string value)
        {
            uint hash = 2166136261u;
            for (int i = 0; i < value.Length; i++)
            {
                hash ^= value[i];
                hash *= 16777619u;
            }
            return hash;
        }

        private static void Execute(ComputeShader shader, ref float[] values)
        {

        }
    }

    [Serializable]
    public class TransientComputeShaderList
    {
        public List<TransientComputeShaderElement> elements = new List<TransientComputeShaderElement>();
    }

    [Serializable]
    public class TransientComputeShaderElement
    {
        public uint key;
        public string uniforms;
        public string content;
        public ComputeShader shader;
    }
}
