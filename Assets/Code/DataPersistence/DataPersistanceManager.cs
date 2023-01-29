using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

using Code.DataPersistence.Data;
using Code.Dialogue.Story;
using Newtonsoft.Json;
using UnityEngine.UI;

namespace Code.DataPersistence
{
    public class SaveData
    {
        public Story CurrentChapter { get; set; }
        public string CurrentNode { get; set; }
    }
    
    public class DataPersistanceManager : MonoBehaviour
    {
        [SerializeField] private Button NewGame;
        [SerializeField] private Button LoadGame;
        
        public static DataPersistanceManager Dpm;

        private void Start()
        {
            Dpm = this;
            Debug.Log("New Instance");
            //NewGame.onClick.AddListener(NewGame_Click);
            //LoadGame.onClick.AddListener(LoadGame_Click);
            if (File.Exists(Application.persistentDataPath + "\\SaveGame.json"))
                LoadGame.interactable = true;
        }

        /// <summary>
        /// Load the game data when you load a game.
        /// </summary>
        public void NewGame_Click()
        {
            GameManager.LoadNewGame();
        }

        public void LoadGame_Click()
        {
            GameManager.LoadSaveGame();
        }
        
        /// <summary>
        /// Saves the game data as a file in the later stages of the game.
        /// </summary>
        public static void SaveGame(string currentNode)
        {
            var saveData = new SaveData
            {
                CurrentChapter = GameObject.FindGameObjectWithTag("Story").GetComponent<StoryHolder>().selectedChapter, 
                CurrentNode = currentNode, // id
            };
            var gameData = new GameData(saveData);
            
            var json = "";
            json = JsonConvert.SerializeObject(gameData, Formatting.Indented);
            File.WriteAllText( Application.persistentDataPath + "\\SaveGame.json", json);  // C:\Users\Kevin\AppData\LocalLow\DefaultCompany
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
