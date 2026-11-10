using System;
using System.Text;
using System.Threading.Tasks;

using UnityEditor;

using UnityEngine;
using UnityEngine.Networking;

using UnityEditor.PackageManager.Requests;
using UnityEditor.PackageManager;

namespace Heaj.Homework
{
    public static class Updater
    {
        private const string fileWriteUrl = "https://api.github.com/repos/Niwala/homework-sync/contents/homework.json";
        private const string fileReadUrl = "https://niwala.github.io/homework-sync/homework.json";
        private const string branch = "main";
        private const int maxAttempts = 4;

        private const string tokenStart = "github_pat_11AFSSANY0Ubr6Fft6Zwzp_";
        private const string tokenEnd = "9N8u9JJDIhjWdjrnkLeJvYZnaypY5qy6ecbsvEkVkyCBNL5XQFBP2tDt";
        private static string Token => tokenStart + tokenEnd;

        private static async Task<MetaDataPack> ReadGithub()
        {
            MetaDataPack metadata = null;
            using (UnityWebRequest request = UnityWebRequest.Get(fileReadUrl))
            {
                await request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.Success)
                {
                    Debug.Log(request.downloadHandler.text);
                    MetaDataPack response = JsonUtility.FromJson<MetaDataPack>(request.downloadHandler.text);
                    Debug.Log(response);
                }
                else
                {
                    Debug.LogError(request.error);
                }
            }

            return metadata;
        }

        public static async Task<MetaDataPack> Read()
        {
            //Get metadata pack from github
            MetaDataPack metadata = await ReadGithub();
            Debug.Log(metadata);
            Debug.Log(metadata.packageVersion);

            //Read current package version
            string packageVersion = await Package.GetPackageVersion();

            //Compare package version
            if (!VersionIsGreaterOrEqual(packageVersion, metadata.packageVersion))
            {
                //Package should be updated
                Debug.Log($"package version is outdated ({packageVersion}). Starting update...");
                await Package.UpdatePackageFromGit();
            }

            //Return
            return metadata;
        }

        public static bool VersionIsGreaterOrEqual(string versionA, string versionB)
        {
            //Drop the build metadata, then split the core version from the pre-release tag
            string[] a = versionA.Split('+')[0].Split('-', 2);
            string[] b = versionB.Split('+')[0].Split('-', 2);

            int coreComparison = new Version(a[0]).CompareTo(new Version(b[0]));
            if (coreComparison != 0)
                return coreComparison > 0;

            //A release without a pre-release tag is greater than the same one with a tag
            if (a.Length != b.Length)
                return a.Length < b.Length;

            return a.Length == 1 || string.CompareOrdinal(a[1], b[1]) >= 0;
        }

        [Serializable]
        private class ContentInfo
        {
            public string sha;
        }

        [Serializable]
        private class UpdatePayload
        {
            public string message;
            public string content;
            public string sha;
            public string branch;
        }

        public static async Task<(bool error, string errorMsg)> UpdateFlagsAsync(string code, MetaDataPack flags)
        {
            string json = JsonUtility.ToJson(flags);
            string base64Content = Convert.ToBase64String(Encoding.UTF8.GetBytes(json));
            string lastError = null;

            for (int attempt = 0; attempt < maxAttempts; attempt++)
            {
                //The sha is fetched again on every attempt, since a concurrent commit invalidates the previous one
                (string sha, string shaError) = await GetShaAsync(code);
                if (shaError != null)
                    return (true, shaError);

                (long responseCode, string putError) = await PutFileAsync(code, base64Content, sha);
                if (putError == null)
                    return (false, null);

                //409 and 422 indicate a stale sha, any other error is not worth retrying
                if (responseCode != 409 && responseCode != 422)
                    return (true, putError);

                lastError = putError;

                //Random growing delay so that simultaneous clients do not collide again
                await Task.Delay(UnityEngine.Random.Range(200, 800) * (attempt + 1));
            }

            return (true, lastError);
        }

        private static async Task<(string sha, string error)> GetShaAsync(string code)
        {
            using (UnityWebRequest request = UnityWebRequest.Get(fileWriteUrl + "?ref=" + branch))
            {
                SetGitHubHeaders(code, request);
                await request.SendWebRequest();

                if (request.result != UnityWebRequest.Result.Success)
                    return (null, request.error);

                ContentInfo info = JsonUtility.FromJson<ContentInfo>(request.downloadHandler.text);
                return (info.sha, null);
            }
        }

        private static async Task<(long responseCode, string error)> PutFileAsync(string code, string base64Content, string sha)
        {
            UpdatePayload payload = new UpdatePayload
            {
                message = "Update homework.json",
                content = base64Content,
                sha = sha,
                branch = branch
            };
            byte[] bodyRaw = Encoding.UTF8.GetBytes(JsonUtility.ToJson(payload));

            using (UnityWebRequest request = new UnityWebRequest(fileWriteUrl, "PUT"))
            {
                request.uploadHandler = new UploadHandlerRaw(bodyRaw);
                request.downloadHandler = new DownloadHandlerBuffer();
                request.SetRequestHeader("Content-Type", "application/json");
                SetGitHubHeaders(code, request);

                await request.SendWebRequest();

                bool success = request.result == UnityWebRequest.Result.Success;
                return (request.responseCode, success ? null : request.error);
            }
        }

        private static void SetGitHubHeaders(string code, UnityWebRequest request)
        {
            request.SetRequestHeader("Authorization", "Bearer " + Token + code);
            request.SetRequestHeader("Accept", "application/vnd.github+json");
            request.SetRequestHeader("X-GitHub-Api-Version", "2022-11-28");
        }
    }
}