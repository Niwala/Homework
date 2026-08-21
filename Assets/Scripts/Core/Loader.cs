using System.Collections;
using System.Collections.Generic;
using System.IO;

using UnityEditor;

using UnityEngine;

namespace SamsBackpack.Homework
{
    public static class Loader
    {
        private const string userShadersPath = "Assets/UserShaders/";

        public static bool GetUserShaderPath(Exercice exercice, out string startShaderPath, out string userShaderPath)
        {
            if (exercice.startShader == null)
            {
                Debug.LogError($"Start shader is missing on {exercice.name}");
                startShaderPath = "";
                userShaderPath = "";
                return false;
            }

            startShaderPath = AssetDatabase.GetAssetPath(exercice.startShader);
            string guid = AssetDatabase.AssetPathToGUID(startShaderPath);
            userShaderPath = $"{userShadersPath}{exercice.startShader.name}.shadergraph";
            return true;
        }

        public static IEnumerable<IOutlinerEntry> GetChapterEntries(Chapter chapter)
        {
            string directory = Path.GetDirectoryName(AssetDatabase.GetAssetPath(chapter));
            foreach (var file in Directory.GetFiles(directory, "*.asset", SearchOption.AllDirectories))
            {
                ScriptableObject obj = AssetDatabase.LoadAssetAtPath<ScriptableObject>(file);

                if (!(obj is IOutlinerEntry entry))
                    continue;

                yield return entry;
            }
        }

        private static IEnumerable<(string, Shader)> LoadShaders(string directory)
        {
            foreach (var file in Directory.GetFiles(directory, "*.shadergraph", SearchOption.AllDirectories))
            {
                Shader shader = AssetDatabase.LoadAssetAtPath<Shader>(file);
                if (shader == null)
                    continue;

                string[] labels = AssetDatabase.GetLabels(shader);
                if (labels.Length == 0)
                    continue;

                yield return (labels[0], shader);
            }
        }

        private static void Rename(Shader userShader, string userShaderPath, string targetName)
        {
            string dir = Path.GetDirectoryName(userShaderPath);
            string name = targetName;
            int i = 0;
            while (File.Exists(dir + name))
                name = targetName + $" r{i++}";
            AssetDatabase.RenameAsset(userShaderPath, name);
        }

        public static void Load(Chapter chapter)
        {
            //Ensure directory
            string chapterDir = userShadersPath + chapter.Title + "/";
            if (!Directory.Exists(chapterDir))
                Directory.CreateDirectory(chapterDir);

            //Load existing
            Dictionary<string, Shader> existings = new Dictionary<string, Shader>();
            foreach ((string startGuid, Shader shader) in LoadShaders(chapterDir))
            {
                if (!existings.ContainsKey(startGuid))
                    existings.Add(startGuid, shader);
            }

            //Match existing
            foreach (var entry in GetChapterEntries(chapter))
            {
                if (entry is Exercice exercice)
                {
                    if (exercice.startShader == null)
                        continue;


                    //Get guid
                    string startPath = AssetDatabase.GetAssetPath(exercice.startShader);
                    string startName = Path.GetFileNameWithoutExtension(startPath);
                    string targetName = startName.Replace("_Start", "_User");
                    string guid = exercice.startGuid;
                    if (string.IsNullOrEmpty(guid))
                        guid = AssetDatabase.AssetPathToGUID(startPath);


                    //Match guid
                    if (existings.ContainsKey(guid))
                    {
                        exercice.userShader = existings[guid];
                        string userPath = AssetDatabase.GetAssetPath(exercice.userShader);
                        string userName = Path.GetFileNameWithoutExtension(userPath);
                        if (targetName != userName)
                            Rename(exercice.userShader, userPath, targetName);
                    }


                    //Create missings
                    else
                    {
                        string userPath = chapterDir + targetName + ".shadergraph";
                        File.Copy(startPath, userPath);
                        AssetDatabase.ImportAsset(userPath, ImportAssetOptions.ForceUpdate);
                        exercice.userShader = AssetDatabase.LoadAssetAtPath<Shader>(userPath);
                        AssetDatabase.SetLabels(exercice.userShader, new string[] { guid });
                    }
                }
            }
        }

        public static void Reset(Chapter chapter)
        {
            string chapterDir = userShadersPath + chapter.Title + "/";
            if (!Directory.Exists(chapterDir))
                return;

            List<string> paths = new List<string>();
            List<string> failedDeletePath = new List<string>();
            foreach ((string _, Shader shader) in LoadShaders(chapterDir))
            {
                paths.Add(AssetDatabase.GetAssetPath(shader));
            }
            AssetDatabase.DeleteAssets(paths.ToArray(), failedDeletePath);

            foreach (var errorPath in failedDeletePath)
            {
                Debug.LogError("Unable to delete shader : " + errorPath);
            }

            AssetDatabase.Refresh();
            Load(chapter);
        }
    }
}