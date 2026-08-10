using System;
using System.IO;

using UnityEditor;
using UnityEditor.UIElements;

using UnityEngine;
using UnityEngine.UIElements;

namespace SamsBackpack.Homework
{
    [CustomEditor(typeof(Exercice))]
    public class ExerciceEditor : Editor
    {

        public override VisualElement CreateInspectorGUI()
        {
            VisualElement root = new VisualElement();

            InspectorElement.FillDefaultInspector(root, serializedObject, this);

            Button formatFromDirBtn = root.Add<Button>();
            formatFromDirBtn.text = "Format from directory";
            formatFromDirBtn.clicked += FormatFromDir;

            return root;
        }

        public void FormatFromDir()
        {
            Exercice exercice = target as Exercice;
            string path = AssetDatabase.GetAssetPath(exercice);
            string dir = Path.GetDirectoryName(path);
            string name = Path.GetFileName(dir);

            //Find shaders
            foreach (var file in Directory.GetFiles(dir))
            {
                string localFile = AbsoluteToRelativePath(file);

                ReplaceIfEndWith(localFile, "_Solution.shadergraph", ref exercice.solution);
                ReplaceIfEndWith(localFile, "_User.shadergraph", ref exercice.userShader);
                ReplaceIfEndWith(localFile, ".uxml", ref exercice.article);
            }
            EditorUtility.SetDirty(exercice);

            Rename(exercice.solution, name + "_Solution");
            Rename(exercice.userShader, name + "_User");
            Rename(exercice.article, name);
            Rename(exercice, name);
        }

        private bool ReplaceIfEndWith<T>(string localFile, string ending, ref T value) where T : UnityEngine.Object
        {
            if (localFile.EndsWith(ending))
            {
                value = AssetDatabase.LoadAssetAtPath<T>(localFile);
                return true;
            }
            return false;
        }

        private void Rename(UnityEngine.Object obj, string newName)
        {
            if (obj == null)
                return;

            string path = AssetDatabase.GetAssetPath(obj);
            string ext = Path.GetExtension(path);
            string newPath = Path.GetDirectoryName(path).Replace("\\", "/") + "/" + newName + ext;
            string error = AssetDatabase.RenameAsset(path, newName);

            if (!string.IsNullOrEmpty(error))
                Debug.Log(error);
        }

        private static string AbsoluteToRelativePath(string absolutePath)
        {
            if (string.IsNullOrEmpty(absolutePath))
                return absolutePath;

            string normalizedPath = absolutePath.Replace('\\', '/');
            string[] anchors = { "/Assets/", "/Packages/" };

            foreach (string anchor in anchors)
            {
                int index = normalizedPath.IndexOf(anchor, StringComparison.OrdinalIgnoreCase);
                if (index >= 0)
                {
                    return normalizedPath.Substring(index + 1);
                }
            }
            return absolutePath;
        }
    }
}