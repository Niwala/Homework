using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;

using UnityEngine;

using Debug = UnityEngine.Debug;

namespace Heaj.Homework
{
    public static class Package
    {
        public const string packageName = "com.heaj.samshomework";

        public static async Task UpdatePackageFromGit()
        {
            //List packages
            ListRequest listRequest = Client.List(true, false);
            while (!listRequest.IsCompleted)
                await Task.Delay(16);

            //Error check
            if (listRequest.Status != StatusCode.Success)
                throw new System.Exception(listRequest.Error.message);

            //Search package
            foreach (var package in listRequest.Result)
            {
                if (package.name == packageName)
                {
                    if (package.git == null)
                        throw new System.Exception("Homework package is not a git package.");

                    AddRequest addRequest = Client.Add(packageName);
                    while (!addRequest.IsCompleted)
                    {
                        await Task.Delay(16);
                    }
                    
                    if (addRequest.Status == StatusCode.Success)
                    {
                        string packageVersion = await Package.GetPackageVersion();
                        Debug.Log($"Package updated (version : {packageVersion})");
                    }
                    else
                    {
                        Debug.LogError("Updated failed : " + addRequest.Error);
                    }

                    return;
                }
            }

            throw new System.Exception("Homework package not found.");
        }

        public static async Task<string> GetPackageVersion()
        {
            string directory = await GetPackageDirectory();
            return GetPackageVersion(directory);
        }

        public static string GetPackageVersion(string packagePath)
        {
            Regex versionRegex = new Regex(
                "(?<prefix>\"version\"\\s*:\\s*\")(?<major>\\d+)\\.(?<minor>\\d+)\\.(?<patch>\\d+)(?<suffix>[^\"]*)(?<end>\")");

            string manifestPath = Path.Combine(packagePath, "package.json");
            if (!File.Exists(manifestPath))
            {
                Debug.LogError($"No package.json found at '{manifestPath}'.");
                return null;
            }

            string content = File.ReadAllText(manifestPath);
            Match match = versionRegex.Match(content);
            if (!match.Success)
            {
                Debug.LogError($"No valid version field found in '{manifestPath}'.");
                return null;
            }

            int patch = int.Parse(match.Groups["patch"].Value);
            return $"{match.Groups["major"].Value}.{match.Groups["minor"].Value}.{patch}{match.Groups["suffix"].Value}";
        }

        public static async Task<(bool, string)> PushPackageToGit(string message)
        {
            string directory = await GetPackageDirectory();

            //Increment package version
            IncrementPackageVersion(directory, out string newPackageVersion);

            //Git push
            return (Push(directory, message), newPackageVersion);
        }

        public static async Task<string> GetPackageDirectory()
        {
            //List packages
            ListRequest listRequest = Client.List(true, false);
            while (!listRequest.IsCompleted)
                await Task.Delay(16);

            //Error check
            if (listRequest.Status != StatusCode.Success)
                throw new System.Exception(listRequest.Error.message);

            //Search package
            foreach (var package in listRequest.Result)
            {
                if (package.name == packageName)
                {
                    return package.assetPath;
                }
            }

            //Not found error
            throw new System.Exception("Homework package not found on the project.");
        }

        private static bool IncrementPackageVersion(string packagePath, out string newVersion)
        {
            Regex versionRegex = new Regex(
                "(?<prefix>\"version\"\\s*:\\s*\")(?<major>\\d+)\\.(?<minor>\\d+)\\.(?<patch>\\d+)(?<suffix>[^\"]*)(?<end>\")");

            newVersion = null;

            string manifestPath = Path.Combine(packagePath, "package.json");
            if (!File.Exists(manifestPath))
            {
                Debug.LogError($"No package.json found at '{manifestPath}'.");
                return false;
            }

            string content = File.ReadAllText(manifestPath);
            Match match = versionRegex.Match(content);
            if (!match.Success)
            {
                Debug.LogError($"No valid version field found in '{manifestPath}'.");
                return false;
            }

            int patch = int.Parse(match.Groups["patch"].Value) + 1;
            string version = $"{match.Groups["major"].Value}.{match.Groups["minor"].Value}.{patch}{match.Groups["suffix"].Value}";

            //Replace only the first occurrence so that dependencies are left untouched
            string updatedContent = versionRegex.Replace(
                content,
                match.Groups["prefix"].Value + version + match.Groups["end"].Value,
                1);

            File.WriteAllText(manifestPath, updatedContent, new UTF8Encoding(false));
            //AssetDatabase.Refresh();

            newVersion = version;
            return true;
        }

        private static bool Push(string directory, string message)
        {
            if (!Directory.Exists(Path.Combine(directory, ".git")))
                return false;

            return Run(directory, "add -A")
                && Run(directory, $"commit -m \"{message}\"")
                && Run(directory, "push");
        }

        private static bool Run(string directory, string arguments)
        {
            ProcessStartInfo info = new ProcessStartInfo("git", arguments)
            {
                WorkingDirectory = directory,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            //Wait for git to finish and report success through the exit code
            using (Process process = Process.Start(info))
            {
                process.WaitForExit();
                return process.ExitCode == 0;
            }
        }
    }
}