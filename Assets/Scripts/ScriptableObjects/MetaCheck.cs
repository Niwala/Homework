using System;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using UnityEngine.Networking;

namespace SamsBackpack.Homework
{
    public static class MetaCheck
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
            using (UnityWebRequest request = UnityWebRequest.Get(baseUrl + binId))
            {
                request.SetRequestHeader("X-Access-Key", publicKey);

                await request.SendWebRequest();

                if (request.result != UnityWebRequest.Result.Success)
                    return null;

                ReadResponse response = JsonUtility.FromJson<ReadResponse>(request.downloadHandler.text);
                return response.record;
            }
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