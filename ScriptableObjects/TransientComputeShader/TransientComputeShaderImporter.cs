using System.IO;
using System.Runtime.CompilerServices;

using UnityEditor;
using UnityEditor.AssetImporters;

using UnityEngine;

namespace Heaj.Homework
{
    [ScriptedImporter(1, "tcs")]
    public class TransientComputeShaderImporter : ScriptedImporter
    {
        private const string templateFile = "TransientShaderTemplate.txt";

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
            string shader = ReadTemplate();
            shader = shader.Replace("#pragma uniforms", element.uniforms);
            shader = shader.Replace("#pragma content", element.content);
            return shader;
        }

        private static string ReadTemplate([CallerFilePath] string path = "")
        {
            string dir = Path.GetDirectoryName(path) + "\\" + templateFile;
            return File.ReadAllText(dir);
        }
    }
}
