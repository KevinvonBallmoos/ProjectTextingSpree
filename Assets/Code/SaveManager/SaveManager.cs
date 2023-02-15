using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

using Code.Dialogue.Story;
using UnityEditor;

namespace Code.SaveManager
{
    public class SaveData
    {
        public string Title { get; set; }
        public int ProgressPercentage { get; set; }
        public int TimeSpent { get; set; }
        public string TimeOfSave { get; set; }
        public string CurrentChapter { get; set; }
        public StoryNode ParentNode { get; set; }
        public string RootNode { get; set; }
        public bool IsStoryNode { get; set; }
    }
    
    public class SaveManager : MonoBehaviour
    {
        // Load save
        [SerializeField] private Button loadSaveGame;
        // GameObjects
        [SerializeField] private GameObject saveSlot1;
        [SerializeField] private GameObject saveSlot2;
        [SerializeField] private GameObject saveSlot3;
        // Screens
        [SerializeField] private GameObject mainMenuScreen;
        [SerializeField] private GameObject saveGameScreen;
        
        private static SaveManager _sm;
        // SaveData
        private static SaveData _saveData;
        private static readonly List<SaveData> LoadedData = new List<SaveData>();
        private static int _slotNum;
        private static bool _isNewGame;
        private static bool _isNewOverride;
        private static bool _isOverride;
        private static string _filename;

        private void Awake()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            
            if (_sm == null)
                _sm = this;
            else
                Destroy(gameObject);
        }
        
        private void Start()
        {
            if (Directory.GetFiles(Application.persistentDataPath).Length > 0)
                loadSaveGame.interactable = true;
        }

        /// <summary>
        /// Load the game data when you load a game.
        /// </summary>
        public void NewGame_Click()
        {
            if (Directory.GetFiles(Application.persistentDataPath).Length == 3)
            {
                // Message Box - No more slots left- you have to override a save
                LoadDataIntoSlots_Click();
                _isOverride = false;
                return;
            }

            _isNewGame = true;
            GameManager.LoadNewGame();
        }
        
        public void LoadDataIntoSlots_Click()
        { 
            mainMenuScreen.SetActive(false);
            saveGameScreen.SetActive(true);
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
                for (var i = 1; i <= 2; i++)
                {
                    if (length == 2)
                        UpdateEmptySlot(i + 1);
                    else if (length == 1)
                    {
                        UpdateEmptySlot(i + 2);
                        break;
                    }
                }
            }
        }

        private void UpdateSlotView(int slotNum)
        {
            var slotObject = slotNum switch
            {
                1 => saveSlot1,
                2 => saveSlot2,
                3 => saveSlot3,
                _ => null
            };
            
            for (var i = 0; i < 4; i++)
            {
                var obj = slotObject.transform.GetChild(i).gameObject;
                var time = DateTime.ParseExact(LoadedData[slotNum - 1].TimeOfSave, "yyyy-dd-M--HH-mm-ss",
                    CultureInfo.InvariantCulture);
                obj.GetComponent<TextMeshProUGUI>().text = i switch
                {
                    0 => $"Chapter: {LoadedData[slotNum -1].Title}",
                    1 => $"Completion: {LoadedData[slotNum -1].ProgressPercentage} %",
                    2 => $"Time of last Save: \n{time:dddd, dd MMMM yyyy. HH:mm:ss}",
                    3 =>  $"Time spent in Game: {LoadedData[slotNum -1].TimeSpent}",
                    _ => obj.GetComponent<TextMeshProUGUI>().text
                };
            }
        }

        private void UpdateEmptySlot(int slotNum)
        {
            var slotObject = slotNum switch
            {
                2 => saveSlot2,
                3 => saveSlot3,
                _ => null
            };

            for (var i = 0; i < 4; i++)
            {
                var obj = slotObject.transform.GetChild(i).gameObject;
                obj.GetComponent<TextMeshProUGUI>().text = i switch
                {
                    0 => "Chapter: No data saved",
                    1 => "Completion: ... %",
                    2 => "Time of last Save: No data",
                    3 => "Time spent in Game: 00d 00h 00m",
                    _ => obj.GetComponent<TextMeshProUGUI>().text
                };
            }
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
            {
                // ask if really want to override status
                GameManager.LoadNewGame();
                _isNewOverride = true;
            }
        }

        public void LoadSlot2_Click()
        {
            _filename = Directory.GetFiles(Application.persistentDataPath)[1];
            if (_isOverride)
                LoadGame(2);
            else
            {
                GameManager.LoadNewGame();
                _isNewOverride = true;
            }
        }

        public void LoadSlot3_Click()
        { 
            _filename = Directory.GetFiles(Application.persistentDataPath)[2];
            if (_isOverride)
                LoadGame(3);
            else{
                GameManager.LoadNewGame();
                _isNewOverride = true;
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
            var time = 0;
            if (LoadedData.Count > 0)
                time = LoadedData[_slotNum].TimeSpent;

            var saveTime = DateTime.Now.ToString("yyyy-dd-M--HH-mm-ss");
            var saveData = new SaveData
            {
                Title = save.RootNode, // Title maybe missing in Dialog System
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
            else if (_isOverride || _isNewOverride)
            {
                if (File.Exists(_filename))
                    File.Delete(_filename);
                
                _filename = _filename[..^24] + saveTime + ".json";
                File.WriteAllText( _filename, json); 
            }
            // Application.persistentDataPath = C:\Users\Kevin\AppData\LocalLow\DefaultCompany
        }


        public void BackToMenu_Click()
        {
            mainMenuScreen.SetActive(true);
            saveGameScreen.SetActive(false);
            _isNewGame = false;
            _isOverride = false;
            
            LoadedData.Clear();
        }
    }
}
