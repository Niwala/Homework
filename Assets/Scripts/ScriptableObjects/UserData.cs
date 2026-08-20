using UnityEditor;

using UnityEngine;

namespace SamsBackpack.Homework
{
    public class UserData : ScriptableObject
    {
        public MetaDataPack metadata = new MetaDataPack();

        private const string path = "Assets/UserData.asset";

        public static UserData Current
        {
            get
            {
                if (current == null)
                    current = LoadExistingUserData();
                if (current == null)
                    current = CreateUserData();
                return current;
            }
        }
        private static UserData current;




        public static UserData LoadExistingUserData()
        {
            return AssetDatabase.LoadAssetAtPath<UserData>(path);
        }

        public static UserData CreateUserData()
        {
            UserData userData = CreateInstance<UserData>();
            AssetDatabase.CreateAsset(userData, path);
            return AssetDatabase.LoadAssetAtPath<UserData>(path);
        }

        public void WriteMetaData(MetaDataPack metadata)
        {
            Current.metadata = metadata;
            EditorUtility.SetDirty(Current);
            AssetDatabase.SaveAssets();
        }

        public MetaDataPack ReadMetaData()
        {
            return Current.metadata;
        }
    }
}