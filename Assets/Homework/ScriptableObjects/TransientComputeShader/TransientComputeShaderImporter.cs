using System.IO;

using UnityEditor;
using UnityEditor.AssetImporters;

using UnityEngine;

namespace SamsBackpack.Homework
{
    [ScriptedImporter(0, "tcs")]
    public class TransientComputeShaderImporter : ScriptedImporter
    {
        public override void OnImportAsset(AssetImportContext ctx)
        {
            //Read file
            string file = File.ReadAllText(ctx.assetPath);
            TransientComputeShaderList list = new TransientComputeShaderList();
            JsonUtility.FromJsonOverwrite(file, list);

            //Create instance
            TransientComputeShaders tcs = ScriptableObject.CreateInstance<TransientComputeShaders>();
            tcs.elements = list.elements;
            ctx.AddObjectToAsset("main", tcs);
            ctx.SetMainObject(tcs);

            //Create shaders
            for (int i = 0; i < tcs.elements.Count; i++)
            {
                string name = "Shader " + tcs.elements[i].key;
                tcs.elements[i].shader = ShaderUtil.CreateComputeShaderAsset(ctx, GenerateShader(tcs.elements[i]));
                tcs.elements[i].shader.name = name;
                ctx.AddObjectToAsset(name, tcs.elements[i].shader);
            }
        }

        private static string GenerateShader(TransientComputeShaderElement element)
        {
            string shader = Database.Resources.transientShaderTemplate.text;
            shader = shader.Replace("#pragma uniforms", element.uniforms);
            shader = shader.Replace("#pragma content", element.content);
            return shader;
        }
    }
}
