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
        private const string baseUrl = "https://api.jsonbin.io/v3/b/";
        private static readonly string publicKey =
            "$2a$10$gkzWrs" +
            "h6zq6BVvg0TN7" +
            "5L.S0O5kJH/WH" +
            "fjXNn2EP0m26u" +
            "e936LYBa";

        private static readonly string partialMasterKey =
            "$2a$10$bM1fV" +
            "YmCkStZCFlTJ" +
            "AhVRuq/X34E6" +
            "Mp8txUp8qxcL" +
            "tr5/G5KTo";

        private static readonly string binId =
            "6a87077c" +
            "da38895d" +
            "fefb9d9b";

        [Serializable]
        private class ReadResponse
        {
            public MetaDataPack record;
        }

        public static async Task<MetaDataPack> Read()
        {
            //Get metadata pack
            MetaDataPack metadata = null;
            using (UnityWebRequest request = UnityWebRequest.Get(baseUrl + binId))
            {
                request.SetRequestHeader("X-Access-Key", publicKey);

                await request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.Success)
                {
                    ReadResponse response = JsonUtility.FromJson<ReadResponse>(request.downloadHandler.text);
                    metadata = response.record;
                }
            }

            //Read current package version
            string packageVersion = await Package.GetPackageVersion();

            //Compare package version
            if (!VersionIsGreaterOrEqual(packageVersion, metadata.packageVersion))
            {
                //Package should be updated
                Debug.Log($"package version is outdated. Update from {packageVersion} to {metadata.packageVersion}.");
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

        public static async Task<(bool error, string errorMsg)> UpdateFlagsAsync(string code, MetaDataPack flags)
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(JsonUtility.ToJson(flags));

            using (UnityWebRequest request = new UnityWebRequest(baseUrl + binId, "PUT"))
            {
                request.uploadHandler = new UploadHandlerRaw(bodyRaw);
                request.downloadHandler = new DownloadHandlerBuffer();
                request.SetRequestHeader("Content-Type", "application/json");
                request.SetRequestHeader("X-Master-Key", partialMasterKey + code);

                await request.SendWebRequest();
                return (request.result != UnityWebRequest.Result.Success, request.error);
            }
        }
    }
}