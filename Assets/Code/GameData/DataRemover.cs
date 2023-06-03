using System.IO;
using UnityEngine;

namespace Code.GameData
{
    /// <summary>
    /// Removes Saved Data
    /// </summary>
    /// <para name="author">Kevin von Ballmoos</para>
    /// <para name="date">05.05.23</para>
    public static class DataRemover
    {
        /// <summary>
        /// Deletes the selected Data
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
            
            GameDataController.Gdc.UpdateEmptySlot(slot + 1); // TODO Update Slots completely
            
            SortFiles();
        }

        /// <summary>
        /// Sorts the Files after one was deleted
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

