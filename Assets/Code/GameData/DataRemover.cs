using System.IO;
using UnityEngine;

namespace Code.Controller.GameController
{
    /// <summary>
    /// Deletes the selected Saved Data File
    /// and sorts the rest of the files
    /// </summary>
    /// <para name="author">Kevin von Ballmoos</para>
    /// <para name="date">05.05.23</para>
    public static class DataRemover
    {
        /// <summary>
        /// Searches the selected Data and deletes the according File
        /// </summary>
        public static void RemoveData_Click()
        {
            var slot = GameDataController.Gdc.GetSlotNum();
            var files = Directory.GetFiles(Application.persistentDataPath);
            if (files.Length == 0) return;
            if (files.Length <= slot)
                slot -= 1;
            var file = files[slot];
           
            File.Delete(file);
            
            GameDataController.Gdc.UpdateEmptySlot(slot + 1); // TODO: Update Slots completely
            
            SortFiles();
        }

        /// <summary>
        /// Sorts and renames the Files after one was deleted
        /// </summary>
        private static void SortFiles()
        {
            var files = Directory.GetFiles(Application.persistentDataPath);
            for (int i = 0; i < files.Length; i++)
            {
                var file = Path.GetFileName(files[i]);
                var newFile = Path.GetDirectoryName(files[i]) + "\\" + file.Replace(file.Substring(0,10), "SaveGame_" +  (i + 1));
                File.Move(files[i], newFile);
            }
        }
    }
}

