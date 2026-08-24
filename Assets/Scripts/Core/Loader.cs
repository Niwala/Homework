using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

using UnityEditor;

using UnityEngine;
using UnityEngine.UIElements;

namespace SamsBackpack.Homework
{
    public static class Loader
    {
        private const string userShadersPath = "Assets/UserShaders/";
        private const string startShadersPath = "/Data/Outliner/";
        private static Dictionary<string, Shader> loadedShaders;
        private const double autoLockTime = 60;

        public struct LoadInfo
        {
            public string startPath;
            public string startGuid;
            public string userPath;
            public string userTargetName;
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

        private static string PluginToUserPath(string path)
        {
            path = path.Replace('\\', '/');

            //Asset from Packages directory
            if (path.StartsWith("Packages"))
            {
                int index = path.IndexOf(startShadersPath);
                if (index == -1)
                {
                    Debug.LogError("Unable to resolve shader path : " + path);
                    return "";
                }

                path = path.Substring(index).Replace(startShadersPath, userShadersPath);
            }

            //Asset from Assets directory
            else
            {
                path = path.Replace("Assets" + startShadersPath, userShadersPath);
            }
            return path;
        }

        public static bool GetPathInfo(Exercice exercice, out LoadInfo loadInfo)
        {
            loadInfo = default;

            string path = AssetDatabase.GetAssetPath(exercice.startShader);
            loadInfo.startPath = path; 
            loadInfo.userTargetName = Path.GetFileNameWithoutExtension(path).Replace("_Start", "_User");
            loadInfo.startGuid = AssetDatabase.AssetPathToGUID(path);

            //Start to user
            path = PluginToUserPath(path);
            path = path.Replace("_Start.shadergraph", "_User.shadergraph");
            loadInfo.userPath = path;

            return true;
        }

        private static void EnsureLoaded()
        {
            if (loadedShaders != null)
                return;

            loadedShaders = new Dictionary<string, Shader>();

            //Create directory if missing
            if (!Directory.Exists(userShadersPath))
                Directory.CreateDirectory(userShadersPath);

            //Load existing
            foreach ((string startGuid, Shader shader) in LoadShaders(userShadersPath))
            {
                if (!loadedShaders.ContainsKey(startGuid))
                    loadedShaders.Add(startGuid, shader);
            }
        }

        private static Shader GetOrCreateUser(Exercice exercice)
        {
            if (exercice.startShader == null)
                return null;

            if (!GetPathInfo(exercice, out LoadInfo loadInfo))
                return null;


            //Match guid
            if (loadedShaders.ContainsKey(loadInfo.startGuid))
            {
                exercice.userShader = loadedShaders[loadInfo.startGuid];
                string currentName = Path.GetFileNameWithoutExtension(loadInfo.userTargetName);
                if (loadInfo.userTargetName != currentName)
                    Rename(exercice.userShader, loadInfo.userPath, loadInfo.userTargetName);
            }


            //Create missings
            else
            {
                string parentDir = Path.GetDirectoryName(loadInfo.userPath);
                if (!Directory.Exists(parentDir))
                    Directory.CreateDirectory(parentDir);

                File.Copy(loadInfo.startPath, loadInfo.userPath);
                FileInfo fileInfo = new FileInfo(loadInfo.userPath);
                fileInfo.IsReadOnly = false;
                fileInfo.CreationTimeUtc = DateTime.UtcNow;
                AssetDatabase.ImportAsset(loadInfo.userPath, ImportAssetOptions.ForceUpdate);
                exercice.userShader = AssetDatabase.LoadAssetAtPath<Shader>(loadInfo.userPath);
                AssetDatabase.SetLabels(exercice.userShader, new string[] { loadInfo.startGuid });
                loadedShaders.Add(loadInfo.startGuid, exercice.userShader);
            }

            return exercice.userShader;
        }

        public static void Load(Chapter chapter)
        {
            EnsureLoaded();

            //Match existing
            foreach (var entry in GetChapterEntries(chapter))
            {
                if (entry is Exercice exercice)
                {
                    if (exercice.startShader == null)
                        continue;

                    GetOrCreateUser(exercice);
                }
            }
        }

        public static void Load(Exercice exercice)
        {
            EnsureLoaded();
            GetOrCreateUser(exercice);
        }

        public static void Reset(Exercice exercice)
        {
            EnsureLoaded();
            string path = AssetDatabase.GetAssetPath(exercice);

            if (!GetPathInfo(exercice, out LoadInfo loadInfo))
                return;

            if (AssetDatabase.AssetPathExists(loadInfo.userPath))
                AssetDatabase.DeleteAsset(loadInfo.userPath);
            File.Copy(loadInfo.startPath, loadInfo.userPath, true);
            FileInfo fileInfo = new FileInfo(loadInfo.userPath);
            fileInfo.IsReadOnly = false;
            fileInfo.CreationTimeUtc = DateTime.UtcNow;
            AssetDatabase.ImportAsset(loadInfo.userPath, ImportAssetOptions.ForceUpdate);
            exercice.userShader = AssetDatabase.LoadAssetAtPath<Shader>(loadInfo.userPath);
            AssetDatabase.SetLabels(exercice.userShader, new string[] { loadInfo.startGuid });
            loadedShaders[loadInfo.startGuid] = exercice.userShader;
        }

        public static void Reset(Chapter chapter)
        {
            if (!EditorUtility.DisplayDialog("Homework", $"Êtes-vous sûr de vouloir réinitialiser le chaptre {chapter.name} ?\nCette opération est irréversible.", "Oui", "Non"))
                return;

            string path = Path.GetDirectoryName(AssetDatabase.GetAssetPath(chapter));
            string chapterUserPath = PluginToUserPath(path);

            AssetDatabase.DeleteAsset(chapterUserPath);
            AssetDatabase.Refresh();
            loadedShaders = null;

            Load(chapter);
        }

        public static bool IsLoaded(Chapter chapter)
        {
            string path = Path.GetDirectoryName(AssetDatabase.GetAssetPath(chapter));
            return Directory.Exists(path);
        }

        public static bool IsLoaded(Exercice exercice)
        {
            if (exercice.startShader == null)
                return false;

            EnsureLoaded();

            //Get guid
            string guid = exercice.startGuid;
            if (string.IsNullOrEmpty(guid))
                guid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(exercice.startShader));


            //Match guid
            if (loadedShaders.ContainsKey(guid))
            {
                exercice.userShader = loadedShaders[guid];
                return true;
            }
            return false;
        }

        public static async Task ApplyAutoLock(Exercice exercice, Button button)
        {
            string path = "";
            if (exercice.userShader == null)
            {
                GetPathInfo(exercice, out LoadInfo loadInfo);
                path = loadInfo.userPath;
            }
            else
            {
                path = AssetDatabase.GetAssetPath(exercice.userShader);
            }

            if (string.IsNullOrEmpty(path))
            {
                ShaderExercice.SetButtonEnable(button, false);
                return;
            }

            FileInfo fileInfo = new FileInfo(path);

            if (!fileInfo.Exists)
            {
                ShaderExercice.SetButtonEnable(button, false);
                return;
            }

            DateTime creationTime = fileInfo.CreationTimeUtc;
            TimeSpan delta = DateTime.UtcNow - creationTime;

            double remainingSeconds = autoLockTime - delta.TotalSeconds;

            if (remainingSeconds < 0)
            {
                ShaderExercice.SetButtonEnable(button, true);
            }
            else
            {
                ShaderExercice.SetButtonEnable(button, false);
                await Task.Delay((int)(remainingSeconds * 1000));
                delta = DateTime.UtcNow - creationTime;
                if (button != null)
                    ShaderExercice.SetButtonEnable(button, true);
            }
        }

    }
}