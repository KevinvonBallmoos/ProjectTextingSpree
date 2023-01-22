using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.PackageManager;
using UnityEngine;
using Code.DataPersistence.Data;

namespace Code.DataPersistence
{
    public class DataPersistanceManager : MonoBehaviour
    {
        [Header("Save file storage config")]
        
        [SerializeField] private string fileName;
        
        // Game data of the game we play atm.
        private GameData _gameData;
        private List<IDataPersistance> _dataPersistancesObjects;
        private FileDataHandler _fileDataHandler;

        // Create instance of the DataPersistanceManager with getter and setter
        public static DataPersistanceManager instance { get; private set; }

        private void Awake()
        {
            // Check if there is an instance of the DataPersistanceManager
            if (instance != null)
            {
                // Throw Error of things get out of hand and there is an instance already.
                Debug.Log(instance.name);
                Debug.LogError("ARGH! An instance of DataPersistanceManager already exists in this scene");
            }
            instance = this;
        }

        private void Start()
        {
            // Set the dataHandler to the standard directory to save the new data.
            _fileDataHandler = new FileDataHandler(Application.persistentDataPath, fileName);
            _dataPersistancesObjects = FindAllDataPersistenceObjects();
            LoadGame();
        }

        /// <summary>
        /// Speaks for itself. It is called when a new game is started.
        /// </summary>
        public void NewGame()
        {
            _gameData = new GameData();
        }

        /// <summary>
        /// Load the game data when you load a game.
        /// </summary>
        public void LoadGame()
        {
            // Load data from file data handler
            _gameData = _fileDataHandler.Load();

            // If no data can be loaded, start a new game.
            if (_gameData == null)
            {
                Debug.Log("No game data was found. Start a new game and initialize the new game");
                NewGame();
            }

            // TODO: Push the data to all the scripts that need this.
            foreach (IDataPersistance dataPersistance in _dataPersistancesObjects)
            {
                dataPersistance.LoadData(_gameData);
            }

            Debug.Log("Game was loaded in theory.");
        }

        /// <summary>
        /// Saves the game data as a file in the later stages of the game.
        /// </summary>
        public void SaveGame()
        {
            // pass the data to all the scripts that needs it so they can update the data
            foreach (IDataPersistance dataPersistance in _dataPersistancesObjects)
            {
                dataPersistance.SaveData(_gameData);
            }

            // Save the data to a file using the data file handler
            _fileDataHandler.Save(_gameData);
        }

        /// <summary>
        /// Saves the game when we quit the game. Or at least it should.
        /// </summary>
        private void OnApplicationQuit()
        {
            SaveGame();
        }

        /// <summary>
        /// This function looks for all the game data to be able to load it later on.
        /// </summary>
        /// <returns></returns>
        private List<IDataPersistance> FindAllDataPersistenceObjects()
        {
            IEnumerable<IDataPersistance> dataPersistanceObjects =
                FindObjectsOfType<MonoBehaviour>().OfType<IDataPersistance>();

            return new List<IDataPersistance>(dataPersistanceObjects);
        }
    }
}
