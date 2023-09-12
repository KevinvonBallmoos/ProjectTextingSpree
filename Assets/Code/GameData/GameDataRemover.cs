using System.IO;
using UnityEngine;

using Code.Controller.GameController;

namespace Code.GameData
{
    /// <summary>
    /// Deletes the selected Saved Data File
    /// and sorts the rest of the files
    /// </summary>
    /// <para name="author">Kevin von Ballmoos</para>
    /// <para name="date">05.05.23</para>
    public static class GameDataRemover
    {
        // Path to the Save files
        private static readonly string SaveDataPath = Application.persistentDataPath + "/SaveData";
        
        /// <summary>
        /// Searches the selected Data and deletes the according File
        /// </summary>
        public static void RemoveData_Click()
        {
            var placeholder = GameDataController.Gdc.GetPlaceholderNum();
            var files = Directory.GetFiles(SaveDataPath);
            
            if (files.Length <= 0) return;
            
            if (placeholder >= files.Length)
                placeholder = files.Length - 1;
            
            // Deletes the file
            File.Delete(files[placeholder]);
            // Updates the placeholder view
            GameDataController.Gdc.LoadDataIntoPlaceholders();
            // Sorts the other save files
            SortFiles();
        }
        
        /// <summary>
        /// Sorts and renames the Files after one was deleted
        /// </summary>
        private static void SortFiles()
        {
            var files = Directory.GetFiles(SaveDataPath);
            for (int i = 0; i < files.Length; i++)
            {
                var file = Path.GetFileName(files[i]);
                var newFile = Path.GetDirectoryName(files[i]) + "\\" + file.Replace(file.Substring(0,10), "SaveGame_" +  (i + 1));
                File.Move(files[i], newFile);
            }
        }
    }
}

