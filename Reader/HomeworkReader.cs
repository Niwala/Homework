using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

using UnityEditor;
using UnityEditor.UIElements;

using UnityEngine;
using UnityEngine.UIElements;

namespace Heaj.Homework
{
    public class HomeworkReader : EditorWindow
    {
        private VisualElement content;
        private ListView list;
        private SerializedObject so;
        private SerializedProperty listProperty;


        //Serialisable properties
        public List<HomeworkFile> files = new List<HomeworkFile>();


        [MenuItem("Window/Homework reader")]
        public static void Open()
        {
            HomeworkReader window = EditorWindow.GetWindow<HomeworkReader>();
            window.Show();
        }

        private void OnEnable()
        {
            //Properties
            so = new SerializedObject(this);
            listProperty = so.FindProperty(nameof(files));


            //Open btn
            Button openBtn = rootVisualElement.Add<Button>();
            openBtn.text = "Open directory";
            openBtn.clicked += OpenDirectory;


            //Content
            content = rootVisualElement.Add("content");


            //List
            list = content.Add<ListView>();
            list.makeItem += () => { return new HomeworkFileElement(); };
            list.bindItem += (VisualElement e, int index) => { ((HomeworkFileElement)e).BindProperty(listProperty, index); };
            list.showBoundCollectionSize = false;
            list.selectionChanged += OnSelectElement;
            list.BindProperty(listProperty);
        }

        private void OpenDirectory()
        {
            //Open folder popup
            string path = EditorUtility.OpenFolderPanel("Homework", "", "");
            if (string.IsNullOrEmpty(path))
                return;
            path = path.Replace("/", "\\") + "\\";

            //List files
            so.UpdateIfRequiredOrScript();
            listProperty.ClearArray();
            string[] fileList = Directory.GetFiles(path, "*.hwe", SearchOption.AllDirectories);
            for (int i = 0,j = 0 ; i < fileList.Length; i++)
            {
                if (!fileList[i].EndsWith(".hwe"))
                    continue;

                listProperty.InsertArrayElementAtIndex(j);
                SerializedProperty prop = listProperty.GetArrayElementAtIndex(j);
                j++;
                prop.boxedValue = new HomeworkFile(fileList[i]);
            }
            so.ApplyModifiedProperties();
        }

        private void OnSelectElement(IEnumerable<object> selection)
        {
            foreach (var item in selection)
            {
                HomeworkFile fileInfo = ((SerializedProperty)item).boxedValue as HomeworkFile;

                if (fileInfo == null)
                    continue;

                string file = Decrypt(File.ReadAllBytes(fileInfo.location));
                Debug.Log(file);
            }
        }

        public static string Decrypt(byte[] encryptedData)
        {
            using Aes aes = Aes.Create();
            aes.Key = Exporter.Hash();

            byte[] iv = new byte[aes.IV.Length];
            Array.Copy(encryptedData, 0, iv, 0, iv.Length);
            aes.IV = iv;

            int cipherTextOffset = iv.Length;
            int cipherTextLength = encryptedData.Length - cipherTextOffset;

            using ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
            using MemoryStream memoryStream = new MemoryStream(encryptedData, cipherTextOffset, cipherTextLength);
            using CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
            using StreamReader streamReader = new StreamReader(cryptoStream, Encoding.UTF8);

            return streamReader.ReadToEnd();
        }
    }

    public class HomeworkFileElement : VisualElement
    {
        public int index;

        private Label label;

        public HomeworkFileElement()
        {
            this.label = this.Add<Label>();
        }

        public void BindProperty(SerializedProperty list, int index)
        {
            this.index = index;
            SerializedProperty prop = list.GetArrayElementAtIndex(index);
            label.text = prop.FindPropertyRelative("name").stringValue;
        }


    }

}
