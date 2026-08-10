using UnityEditor;
using UnityEngine;

using System.Reflection;

namespace SamsBackpack.Homework
{
    public class ShaderChecker : AssetPostprocessor
    {
        public static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            for (int i = 0; i < importedAssets.Length; i++)
            {
                if (importedAssets[i].EndsWith(".shadergraph"))
                    OnImportAsset(importedAssets[i]);
            }
        }

        private static void OnImportAsset(string file)
        {
            if (WarmupCounter.currentCounter == null)
                return;

            Shader shader = AssetDatabase.LoadAssetAtPath<Shader>(file);
            if (shader == null)
                return;

            if (WarmupCounter.currentCounter.currentUser == shader)
            {
                EditorWindow[] windows = Resources.FindObjectsOfTypeAll<EditorWindow>();
                foreach (var win in windows)
                {
                    if (win.GetType().Name == "MaterialGraphEditWindow")
                    {
                        PropertyInfo selectedGuidProp = win.GetType().GetProperty("selectedGuid", BindingFlags.Public | BindingFlags.Instance);
                        
                        if (selectedGuidProp.GetValue(win).ToString() == AssetDatabase.AssetPathToGUID(file))
                        {
                            if (WarmupCounter.currentCounter.GetErrorFactor() < 100)
                            {
                                EditorWindow toClose = win;
                                EditorApplication.delayCall += () => toClose.Close();
                                WarmupCounter.currentCounter.NextShader();
                            }
                            break;
                        }
                    }
                }
            }          
        }
    }
}
