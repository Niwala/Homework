using System.Diagnostics;
using System.IO;
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

        public static async Task<string> GetLastGitCommit()
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
                    Debug.Log(package.packageId);
                    Debug.Log(package.assetPath);

                    if (package.git == null)
                        throw new System.Exception("Homework package is not a git package.");

                    Debug.Log(package.git.revision);
                    Debug.Log(package.datePublished);

                    return package.git.revision;
                }
            }

            //Not found error
            throw new System.Exception("Homework package not found on the project.");
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

        public static async Task<bool> PushGitContent(string message)
        {
            string directory = await GetPackageDirectory();
            return Push(directory, message);
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