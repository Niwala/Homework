using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;

using UnityEditor;
using UnityEngine;

namespace Heaj.Homework
{
    public static class Exporter
    {
        public static string Export(ExportData exportData, string directory, string fileName)
        {
            string path = EditorUtility.SaveFilePanel("Homework", directory, fileName, "hwe");
            if (string.IsNullOrEmpty(path))
                return null;

            string plainText = new ExportContainer(exportData).ToPlainText();
            File.WriteAllBytes(path, Encrypt(plainText));
            return path;
        }

        public static byte[] Encrypt(string plainText)
        {
            using Aes aes = Aes.Create();
            aes.Key = Hash();
            aes.GenerateIV();

            using ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
            byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);

            using MemoryStream memoryStream = new MemoryStream();
            memoryStream.Write(aes.IV, 0, aes.IV.Length);

            using (CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
            {
                cryptoStream.Write(plainBytes, 0, plainBytes.Length);
            }

            return memoryStream.ToArray();
        }

        public static byte[] Hash()
        {
            string rawContent = File.ReadAllText(FP(), Encoding.UTF8);
            string normalizedContent = NormalizeContent(rawContent);
            byte[] contentBytes = Encoding.UTF8.GetBytes(normalizedContent);
            using SHA256 sha256 = SHA256.Create();
            return sha256.ComputeHash(contentBytes);
        }

        private static string NormalizeContent(string content)
        {
            string unifiedLineEndings = content.Replace("\r\n", "\n").Replace("\r", "\n");
            string[] lines = unifiedLineEndings.Split('\n');

            StringBuilder builder = new StringBuilder();
            foreach (string line in lines)
            {
                string trimmedLine = line.Trim();
                if (trimmedLine.Length == 0)
                    continue;

                builder.Append(trimmedLine);
                builder.Append('\n');
            }
            return builder.ToString();
        }

        private static string FP([CallerFilePath] string fp = default) => fp;

        [Serializable]
        private struct ExportContainer
        {
            public ExportContainer(ExportData exportData)
            {
                unityUsername = CloudProjectSettings.userName;
                unityUserId = CloudProjectSettings.userId;
                deviceIdentifier = SystemInfo.deviceUniqueIdentifier;
                deviceName = SystemInfo.deviceName;
                this.exportData = exportData;
            }

            public string ToPlainText()
            {
                return JsonUtility.ToJson(this, false);
            }

            public string unityUsername;
            public string unityUserId;
            public string deviceIdentifier;
            public string deviceName;
            public ExportData exportData;
        }
    }
}