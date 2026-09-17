using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using UnityEditor;

using UnityEngine;

namespace SamsBackpack.Homework
{
    public static class State
    {
        public static bool HasMetaData => hasMetaData;
        private static bool hasMetaData = false;

        public static bool UseLocalMetaData => useLocalMetaData;
        private static bool useLocalMetaData = true;

        public static string MetaDataDownloadMsg => metaDataDownloadMsg;
        private static string metaDataDownloadMsg;

        public static Action onMetaDataChanged;
        public static Action onLockChanged;
        public static Action onEditModeChanged;

        public static bool InEditMode
        {
            get
            {
#if HOMEWORK_EDIT
                return inEditMode;
#else
                return false;
#endif
            }
        }
        private static bool inEditMode;
        public const string editModePref = "Homework.EditMode";

        private static List<(DateTime, string)> autoLocks = new List<(DateTime, string)>();

        public static void Refresh()
        {
            hasMetaData = false;
            useLocalMetaData = true;
            metaDataDownloadMsg = "";
            onMetaDataChanged?.Invoke();
            AutoLoad();
        }

        [InitializeOnLoadMethod]
        private async static void AutoLoad()
        {
            //Edit mode
            CheckEditMode();
            onEditModeChanged -= CheckEditMode;
            onEditModeChanged += CheckEditMode;

            //Internet read
            Task<MetaDataPack> metaDataRead = Updater.Read();
            MetaDataPack metaDataPack = await metaDataRead;

            if (metaDataPack == null)
            {
                hasMetaData = true;
                useLocalMetaData = true;
                metaDataDownloadMsg = "Unable to load the latest data. The previous version will be used instead.\n\nPlease check your internet connection.";
                EditorUtility.DisplayDialog("Homework", "Unable to load the latest data. The previous version will be used instead.\n\nPlease check your internet connection.", "Ok");
            }
            else
            {
                //Update local
                UserData.WriteMetaData(metaDataPack);

                hasMetaData = true;
                useLocalMetaData = false;
                metaDataDownloadMsg = "";
            }


            UpdateAutoLocks();

        }

        public static void UpdateAutoLocks()
        {
            //Clear previous autolocks
            autoLocks.Clear();

            //Trigger auto-locks
            bool hasLimiteds = false;
            foreach (var meta in UserData.Metadata.entries)
            {
                if (meta.status == Status.Limited)
                {
                    if (DateTime.TryParse(meta.limitedTime, out DateTime lockTime))
                    {
                        TimeSpan remaining = lockTime - DateTime.Now;

                        if (remaining.TotalSeconds < 0)
                        {
                            meta.status = Status.Locked;
                        }
                        else
                        {
                            meta.status = Status.Limited;
                            hasLimiteds = true;
                            autoLocks.Add((lockTime, meta.name));
                        }
                    }
                    else
                    {
                        meta.status = Status.Locked;
                    }
                }
            }

            onMetaDataChanged?.Invoke();

            //Start check loop
            EditorApplication.update -= CheckLoop;
            if (hasLimiteds)
                EditorApplication.update += CheckLoop;
        }

        private static void CheckEditMode()
        {
            inEditMode = EditorPrefs.GetBool(editModePref, false);
        }

        private static void CheckLoop()
        {
            DateTime now = DateTime.Now;

            int count = autoLocks.Count;
            for (int i = 0; i < count; i++)
            {
                (DateTime lockTime, string name) = autoLocks[i];
                TimeSpan remaining = lockTime - DateTime.Now;

                if (remaining.TotalSeconds < 0)
                {
                    Lock(name);
                    autoLocks.RemoveAt(i);
                    count--;
                    i--;
                }
            }
        }

        private static void Lock(string name)
        {
            onMetaDataChanged?.Invoke();
            onLockChanged?.Invoke();
        }
    }
}
