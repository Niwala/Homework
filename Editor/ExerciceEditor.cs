using System;
using System.IO;

using UnityEditor;
using UnityEditor.UIElements;

using UnityEngine;
using UnityEngine.UIElements;

namespace Heaj.Homework
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

                ReplaceIfEndWith(localFile, "_Solution.shadergraph", ref exercice.solutionShader);
                ReplaceIfEndWith(localFile, "_Start.shadergraph", ref exercice.startShader);
                ReplaceIfEndWith(localFile, ".uxml", ref exercice.article);
            }

            if (exercice.startShader != null)
            {
                string startPath = AssetDatabase.GetAssetPath(exercice.startShader);
                exercice.startGuid = AssetDatabase.AssetPathToGUID(startPath);
            }

            EditorUtility.SetDirty(exercice);

            Rename(exercice.solutionShader, name + "_Solution");
            Rename(exercice.startShader, name + "_Start");
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