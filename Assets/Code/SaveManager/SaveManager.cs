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
using Unity.VisualScripting;
using UnityEngine.iOS;

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
        private static bool _isNewGame;
        private static bool _isOverride;
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
            if (Directory.GetFiles(Application.persistentDataPath).Length > 0)
                LoadSaveGame.interactable = true;
        }

        /// <summary>
        /// Load the game data when you load a game.
        /// </summary>
        public void NewGame_Click()
        {
            if (Directory.GetFiles(Application.persistentDataPath).Length == 3)
            {
                // Message Box - No more slots left- you have to override a save
                MainMenuScreen.SetActive(false);
                SaveGameScreen.SetActive(true);
                _isOverride = true;
                return;
            }

            _isNewGame = true;
            GameManager.LoadNewGame();
        }
        
        public void LoadDataIntoSlots_Click()
        { 
            MainMenuScreen.SetActive(false);
            SaveGameScreen.SetActive(true);
            _isNewGame = false;
            _isOverride = true;
            var files = Directory.GetFiles(Application.persistentDataPath);

            foreach (var file in files)
            {
                var json = File.ReadAllText(file, Encoding.UTF8);
                LoadedData.Add(JsonConvert.DeserializeObject<SaveData>(json));
            }
            for (var i = 1;i <= LoadedData.Count;i++)
                UpdateSlotView(i);
            
            if (LoadedData.Count >= 3) return;
            {
                var length = 3 - LoadedData.Count;
                for (var i = 1; i < 2; i++)
                {
                    if (length == 2)
                        UpdateEmptySlot(i + 1);
                    else if (length == 1)
                        UpdateEmptySlot(i + 2);
                }
            }
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

            GameObject obj = null;
            for (int i = 0; i < 4; i++)
            {
                obj = slotObject.transform.GetChild(i).gameObject;

                switch (i)
                {
                    case 0:
                        obj.GetComponent<TextMeshProUGUI>().text += LoadedData[slotNum -1].Title;
                        break;
                    case 1:
                        obj.GetComponent<TextMeshProUGUI>().text += LoadedData[slotNum -1].ProgressPercentage + " %";
                        break;
                    case 2:
                        obj.GetComponent<TextMeshProUGUI>().text += LoadedData[slotNum -1].TimeOfSave;
                        break;
                    case 3:
                        obj.GetComponent<TextMeshProUGUI>().text += LoadedData[slotNum -1].TimeSpent;
                        break;
                }
            }
            
            
            

                // append all info
            // LoadedData[slotNum]
        }

        private void UpdateEmptySlot(int slotNum)
        {
            
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
        {   _filename = Directory.GetFiles(Application.persistentDataPath)[0];
            if (_isOverride)
                LoadGame(1);
            else
                // ask if really want to override status
                GameManager.LoadNewGame();
            
        }

        public void LoadSlot2_Click()
        {
            _filename = Directory.GetFiles(Application.persistentDataPath)[1];
            if (_isOverride)
                LoadGame(2); 
            else
                GameManager.LoadNewGame();
        }

        public void LoadSlot3_Click()
        { 
            _filename = Directory.GetFiles(Application.persistentDataPath)[2];
            if (_isOverride)
                LoadGame(3);
            else
                GameManager.LoadNewGame();
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
            var time = 0;
            if (LoadedData.Count > 0)
                time = LoadedData[_slotNum].TimeSpent;

            var saveTime = DateTime.Now.ToString("yyyy-dd-M--HH-mm-ss");
            var saveData = new SaveData
            {
                Title = "Chapter Title", // Title maybe missing in Dialog System
                ProgressPercentage = 10, // GetProgressPercentage(), // Write method 
                TimeSpent = time, // + GetTimeSpentInGame(), // Write Method
                TimeOfSave = saveTime,
                CurrentChapter = GameObject.FindGameObjectWithTag("Story").GetComponent<StoryHolder>().selectedChapter
                    .name,
                ParentNode = save.ParentNode,
                IsStoryNode = save.IsStoryNode
            };

            var gameData = new GameData(saveData);
            var json = JsonConvert.SerializeObject(gameData, Formatting.Indented);

            if (_isNewGame && _isOverride == false)
            {
                _filename = Application.persistentDataPath + $"\\SaveGame_{Directory.GetFiles(Application.persistentDataPath).Count() + 1}_{saveTime}.json";
                File.WriteAllText(_filename, json);
                _isNewGame = false;
                _isOverride = true;
            }
            else if (_isOverride)
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
