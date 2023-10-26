using System.IO;
using UnityEngine;

namespace Code.Model.FileModels
{
    /// <summary>
    /// Controls the CRUD of Files and Folders
    /// </summary>
    /// <para name="author">Kevin von Ballmoos</para>
    /// <para name="date">11.01.2023</para>
    public static class FileIOModel
    {
        // Path to the Save files
        private static readonly string SaveDataPath = Application.persistentDataPath + "/SaveData";
        
        /// <summary>
        /// Creates Folders
        /// SaveData: stores the Save Data from the Game
        /// StoryAssets: stores the node Information as Json files
        /// </summary>
        public static void CreateFolders()
        {
            var folders = new [] { "/SaveData", "/StoryAssets" };
            foreach (var f in folders)
            {
                if (Directory.Exists(Application.persistentDataPath + f)) return;
                Directory.CreateDirectory(Application.persistentDataPath + f);
            }
        }
        
        /// <summary>
        /// Sorts and renames the Files after one was deleted
        /// </summary>
        public static void SortSaveFiles()
        {
            var files = Directory.GetFiles(SaveDataPath);
            for (int i = 0; i < files.Length; i++)
            {
                var file = Path.GetFileName(files[i]);
                var newFile = Path.GetDirectoryName(files[i]) + "\\" + file.Replace(file.Substring(0,10), "SaveGame_" +  (i + 1));
                File.Move(files[i], newFile);
            }
        }

        /// <summary>
        /// Deletes a specific file
        /// </summary>
        /// <param name="path">path to the file, that needs to be deleted</param>
        public static void DeleteFile(string path)
        {
            File.Delete(path);
        }
    }
}