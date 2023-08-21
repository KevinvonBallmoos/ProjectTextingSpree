using System.IO;
using UnityEngine;

namespace Code.Controller.FileControllers
{
    /// <summary>
    /// Controls the CRUD of Files and Folders
    /// </summary>
    /// <para name="author">Kevin von Ballmoos</para>
    /// <para name="date">11.01.2023</para>
    public static class FileController
    {
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
    }
}