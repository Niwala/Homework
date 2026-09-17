using System.Linq;

using UnityEditor;

using UnityEngine;

namespace SamsBackpack.Homework
{
    public class UserData : ScriptableObject
    {
        public string userName;
        public string userSurname;

        public static MetaDataPack Metadata => Current.metadata;

#if !HOMEWORK_EDIT
        [HideInInspector]
#endif
        [SerializeField]
        public MetaDataPack metadata  = new MetaDataPack();


        private const string path = "Assets/UserData.asset";
        public const string prefPrefix = "samsbackpack.homework.";

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

        public static SerializedObject SerializedObject
        {
            get
            {
                if (serializedObject == null)
                    serializedObject = new SerializedObject(Current);
                return serializedObject;
            }
        }
        private static SerializedObject serializedObject;

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

        public static void WriteMetaData(MetaDataPack metadata)
        {
            Current.metadata.timeStamp = metadata.timeStamp;
            foreach (var entry in metadata.entries)
            {
                Current.metadata.Add(entry, true);
            }

            EditorUtility.SetDirty(Current);
            AssetDatabase.SaveAssets();
        }

        public static MetaData GetMetaData(IOutlinerEntry entry)
        {
            MetaData data = Metadata.entries.FirstOrDefault(x => x.name == entry.Title);
            if (data == null)
                data = new MetaData(entry.Title);
            return data;
        }
    }
}