using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

namespace Code.DataPersistence.Data
{
    public class FileDataHandler
    {
        // Path to save file
        private string _dataDirPath = "";

        // Save file name
        private string _dataFileName = "";

        public FileDataHandler(string dataDirPath, string dataFileName)
        {
            // this.dataDirPath = _dataDirPath;
            // this.dataFileName = _dataFileName;
        }

        // public GameData Load()
        // {
        //     
        // }

        public void Save(GameData data)
        {

        }
    }
}
