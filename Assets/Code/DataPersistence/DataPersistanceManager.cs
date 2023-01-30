using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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
        public StoryNode CurrentNode { get; set; }
        public StoryNode ParentNode { get; set; }
        public bool IsStoryNode { get; set; }
        public bool IsNull { get; set; }
    }
    
    public class DataPersistanceManager : MonoBehaviour
    {
        [SerializeField] private Button NewGame;
        [SerializeField] private Button LoadGame;
        
        public static DataPersistanceManager Dpm;
        // SaveData
        private static SaveData _saveData;

        private void Awake()
        {
            if (Dpm == null)
                Dpm = this;
            else
                Destroy(gameObject);
        }
        
        private void Start()
        {
            Debug.Log("New Instance");
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
            var json = File.ReadAllText(Application.persistentDataPath + "\\SaveGame.json", Encoding.UTF8);
            _saveData = JsonConvert.DeserializeObject<SaveData>(json);

            Debug.Log("Load successful");
            GameManager.LoadSavedScene(_saveData.CurrentChapter.name);
        }
        
        public static bool LoadData()
        {
            return _saveData != null;
        }
        
        public static SaveData GetSaveData()
        {
            Debug.Log("Loading");
            return _saveData;
        }
        
        /// <summary>
        /// Saves the game data as a file in the later stages of the game.
        /// </summary>
        /// <param name="save"></param>
        public static void SaveGame(SaveData save)
        {
            var saveData = new SaveData
            {
                CurrentChapter = GameObject.FindGameObjectWithTag("Story").GetComponent<StoryHolder>().selectedChapter,
                ParentNode = save.ParentNode,
                IsStoryNode = save.IsStoryNode,
                IsNull = save.IsNull
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
