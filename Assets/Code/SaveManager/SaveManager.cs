using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

using Code.Dialogue.Story;
using Newtonsoft.Json;
using TMPro;

namespace Code.SaveManager
{
    public class SaveData
    {
        public string Title { get; set; }
        public int ProgressPercentage { get; set; }
        public int TimeSpent { get; set; }
        public string TimeOfSave { get; set; }
        public string CurrentChapter { get; set; }
        public StoryNode CurrentNode { get; set; }
        public StoryNode ParentNode { get; set; }
        public bool IsStoryNode { get; set; }
    }
    
    public class SaveManager : MonoBehaviour
    {
        // Buttons
        [SerializeField] private Button NewGame;
        [SerializeField] private Button LoadSaveGame;
        
        // GameObjects
        [SerializeField] private GameObject SaveSlot1;
        [SerializeField] private GameObject SaveSlot2;
        [SerializeField] private GameObject SaveSlot3;
        
        // Screens
        [SerializeField] private GameObject MainMenuScreen;
        [SerializeField] private GameObject SaveGameScreen;
        
        public static SaveManager Dpm;
        // SaveData
        private static SaveData _saveData;
        private static readonly List<SaveData> LoadedData = new List<SaveData>();
        private static int _slotNum;
        private static bool _isNewGame = false;
        private static string _filename;
        
        private void Awake()
        {
            if (Dpm == null)
                Dpm = this;
            else
                Destroy(gameObject);
        }
        
        private void Start()
        {
            Debug.Log("Start Function");
            if (Directory.GetFiles(Application.persistentDataPath).Length > 0)
                LoadSaveGame.interactable = true;
        }

        /// <summary>
        /// Load the game data when you load a game.
        /// </summary>
        public void NewGame_Click()
        {
            Debug.Log(Directory.GetFiles(Application.persistentDataPath).Length.ToString());
            if (Directory.GetFiles(Application.persistentDataPath).Length == 3)
            {
                // Message Box - No more slots left- you have to override a save
                MainMenuScreen.SetActive(false);
                SaveGameScreen.SetActive(true);
                _isNewGame = true;
                return;
            }

            GameManager.LoadNewGame();
        }
        
        public void LoadDataIntoSlots_Click()
        { 
            MainMenuScreen.SetActive(false);
            SaveGameScreen.SetActive(true);
            _isNewGame = false;
            
            var files = Directory.GetFiles(Application.persistentDataPath);

            foreach (var file in files)
            {
                var json = File.ReadAllText(file, Encoding.UTF8);
                LoadedData.Add(JsonConvert.DeserializeObject<SaveData>(json));
            }
            for (var i = 1;i <= 3;i++)
                UpdateSlotView(i);
        }

        private void UpdateSlotView(int slotNum)
        {
            GameObject slotObject = slotNum switch
            {
                1 => SaveSlot1,
                2 => SaveSlot2,
                3 => SaveSlot3,
                _ => null
            };

            // for loop over each child, assign then set text
            // When load data = "" then write empty or ...
                var title = slotObject.transform.GetChild(0).gameObject;
                title.GetComponent<TextMeshProUGUI>().text += LoadedData[slotNum -1].Title;
                // append all info
            // LoadedData[slotNum]
        }

        private static void LoadGame(int slotNum)
        {
            _slotNum = slotNum;
            var files = Directory.GetFiles(Application.persistentDataPath);
            var json = File.ReadAllText(files[slotNum -1], Encoding.UTF8);
            
            _saveData = JsonConvert.DeserializeObject<SaveData>(json);
            GameManager.LoadSavedScene(_saveData.CurrentChapter);
        }

        public void LoadSlot1_Click()
        {
            if (!_isNewGame)
                LoadGame(1);
            else
            {
                // ask if really want to override status
                _filename = Directory.GetFiles(Application.persistentDataPath)[0];
                GameManager.LoadNewGame();
            }
        }

        public void LoadSlot2_Click()
        {
            if (!_isNewGame)
                LoadGame(2); 
            else
            {
                _filename = Directory.GetFiles(Application.persistentDataPath)[1];
                GameManager.LoadNewGame();
            }
        }

        public void LoadSlot3_Click()
        {
            if (!_isNewGame)
                LoadGame(3);
            else
            {
                _filename = Directory.GetFiles(Application.persistentDataPath)[2];
                GameManager.LoadNewGame();
            }
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
            // When its saving the first time, user has to choose the slot
            var time = 0;
            if (LoadedData.Count > 0)
                time = LoadedData[_slotNum].TimeSpent;

            var saveData = new SaveData
            {
                Title = "Test Title", // Title maybe missing in Dialog System
                ProgressPercentage = 10, // GetProgressPercentage(), // Write method 
                TimeSpent = time, // + GetTimeSpentInGame(), // Write Method
                TimeOfSave = DateTime.Now.ToString("yyyy-dd-M:HH-mm-ss"),
                CurrentChapter = GameObject.FindGameObjectWithTag("Story").GetComponent<StoryHolder>().selectedChapter
                    .name,
                ParentNode = save.ParentNode,
                IsStoryNode = save.IsStoryNode
            };

            var gameData = new GameData(saveData);
            var json = "";
            var saveTime = DateTime.Now.ToString("yyyy-dd-M--HH-mm-ss");
            json = JsonConvert.SerializeObject(gameData, Formatting.Indented);

            if (!_isNewGame){
                _filename = $"\\SaveGame_{Directory.GetFiles(Application.persistentDataPath).Count()}_{saveTime}.json"; // date
                File.WriteAllText( Application.persistentDataPath + _filename, json); 
            }
            else

            {
                if (File.Exists(_filename))
                    File.Delete(_filename);
                _filename = _filename[..^24] + saveTime + ".json";
                File.WriteAllText( _filename, json); 
            }
            // Application.persistentDataPath = C:\Users\Kevin\AppData\LocalLow\DefaultCompany
        }
    }
}
