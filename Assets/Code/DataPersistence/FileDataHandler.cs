using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

namespace Code.DataPersistence.Data
{
    public class FileDataHandler
    {
        // Class variables.
        private string _dataDirPath = "";
        private string _dataFileName = "";
        private GameData _loadedData = null;

        /// <summary>
        /// Constructor used for this class.
        /// </summary>
        /// <param name="dataDirPath"></param>
        /// <param name="dataFileName"></param>
        public FileDataHandler(string dataDirPath, string dataFileName)
        {
            Debug.Log("yaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaw");
            _dataDirPath = dataDirPath;
            _dataFileName = dataFileName;
        }

        /// <summary>
        /// Returns a game data object containing the game data.
        /// </summary>
        /// <returns></returns>
        public GameData Load()
        {
            // I use Path.Combine here to try and accout for different OS the game might be saved.
            string fullSaveFilePath = Path.Combine(_dataDirPath, _dataFileName);

            // Check if save file exists
            if (File.Exists(fullSaveFilePath))
            {
                try
                {
                    // Load serialized data from file.
                    string dataToLoad = "";

                    using (FileStream stream = new FileStream(fullSaveFilePath, FileMode.Open))
                    {
                        using (StreamReader reader = new StreamReader(stream))
                        {
                            dataToLoad = reader.ReadToEnd();
                        }
                    }
                    
                    // Deserialize the data into a game object. Just to make sure.
                    _loadedData = JsonUtility.FromJson<GameData>(dataToLoad);
                }
                catch (Exception e)
                {
                    Debug.LogError("Something not working with this save, mate: " + fullSaveFilePath + "\n" + e);
                }
            }

            return _loadedData;
        }

        /// <summary>
        /// Takes in game data object to store information in the file to
        /// </summary>
        /// <param name="data"></param>
        public void Save(GameData data)
        {
            // I use Path.Combine here to try and accout for different OS the game might be saved.
            string fullSaveFilePath = Path.Combine(_dataDirPath, _dataFileName);

            try
            {
                // Create directory path just in case it does not exist on the computer now.
                Directory.CreateDirectory(Path.GetDirectoryName(fullSaveFilePath));
                
                // Below we serialize the data into a JSON string using JsonUtility. The true parameter is passed to
                // format the data into JSON format.
                string dataToStore = JsonUtility.ToJson(data, true);
                
                // Write the file into the file system. Hurray!
                // We use "using" here, because this function closes the connection to the file as soon as its done
                // reading or writing.
                using (FileStream stream = new FileStream(fullSaveFilePath, FileMode.Create))
                {
                    using (StreamWriter write = new StreamWriter(stream))
                    {
                        write.Write(dataToStore);
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError("Something went wrong in the save: " + fullSaveFilePath + "\n" + e);
            }
        }
    }
}
