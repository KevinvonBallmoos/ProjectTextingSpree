using Newtonsoft.Json;
using System;
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
using Unity.VisualScripting;

namespace Code.GameDataManager
{
    /// <summary>
    /// Saves and Loads the status of the Game
    /// </summary>
    /// <para name="author">Kevin von Ballmoos</para>
    /// <para name="date">30.01.2023</para>
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
    
    /// <summary>
    /// Saves and Loads the status of the Game
    /// </summary>
    /// <para name="author">Kevin von Ballmoos</para>
    /// <para name="date">30.01.2023</para>
    public class GameDataManager : MonoBehaviour
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
        [SerializeField] private GameObject overrideSaveGameScreen;
        
        private static GameDataManager _sm;
        // SaveData
        private static SaveData _saveData;
        private static readonly List<SaveData> LoadedData = new List<SaveData>();
        private static int _slotNum;
        private static bool _isNewGame;
        private static bool _isNewOverride;
        private static bool _isOverride;
        private static string _filename;

        /// <summary>
        /// Sets the language of the program to en-US
        /// </summary>
        private void Awake()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            
            if (_sm == null)
                _sm = this;
            else
                Destroy(gameObject);
        }
        
        /// <summary>
        /// If there are any save files, then the LoadSaveGame Button in the Menu is activated
        /// </summary>
        private void Start()
        {
            if (Directory.GetFiles(Application.persistentDataPath).Length > 0)
                loadSaveGame.interactable = true;
        }

        /// <summary>
        /// When a new Game is started, it checks for a open save slot, are there non,
        /// then the User has to choose an old save slot to override the date with the new Game
        /// </summary>
        public void NewGame_Click()
        {
            _isNewGame = true;
            if (Directory.GetFiles(Application.persistentDataPath).Length == 3)
            {
                overrideSaveGameScreen.SetActive(true);
                _isOverride = false;
                return;
            }
            
            GameManager.LoadNewGame();
        }

        /// <summary>
        /// When continue is clicked, the User can select a save slot to override the old data
        /// </summary>
        public void Continue_Click()
        {
            mainMenuScreen.SetActive(false);
            saveGameScreen.SetActive(true);
            overrideSaveGameScreen.SetActive(false);
            LoadDataIntoSlots_Click();
        }
        
        /// <summary>
        /// When cancel is clicked, then the menu screen is visible
        /// </summary>
        public void Cancel_CLick()
        {
            mainMenuScreen.SetActive(true);
            saveGameScreen.SetActive(false);
            overrideSaveGameScreen.SetActive(false);
        }
        
        /// <summary>
        /// When the LoadGame Button is clicked, then the save files getting loaded into the save slots
        /// Is there no save for a slot, then the slot stays empty
        /// </summary>
        public void LoadDataIntoSlots_Click()
        {
            if (_isNewGame)
            {
                var btnObject = saveGameScreen.GetComponentInChildren<Button>().GetComponentsInChildren<Text>();
                foreach (var obj in btnObject)
                {
                    if (obj.name == "OverrideInformation")
                        obj.enabled = true;
                }
            }

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

        /// <summary>
        /// Updates the Slot view with loaded data
        /// </summary>
        /// <param name="slotNum"></param>
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

        /// <summary>
        /// Updates the Slot view with empty data, if there is no save 
        /// </summary>
        /// <param name="slotNum"></param>
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

        /// <summary>
        /// Loads the Clicked Game
        /// </summary>
        /// <param name="slotNum"></param>
        private static void LoadGame(int slotNum)
        {
            _slotNum = slotNum;
            var files = Directory.GetFiles(Application.persistentDataPath);
            var json = File.ReadAllText(files[slotNum -1], Encoding.UTF8);
            
            _saveData = JsonConvert.DeserializeObject<SaveData>(json);
            GameManager.LoadSavedScene(_saveData.CurrentChapter);
        }

        /// <summary>
        /// Override for a new Game or Load Game
        /// </summary>
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

        /// <summary>
        /// Override for a new Game or Load Game
        /// </summary>
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

        /// <summary>
        /// Override for a new Game or Load Game
        /// </summary>
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

        /// <summary>
        /// Returns true if Data has been loaded,
        /// else when not
        /// </summary>
        /// <returns></returns>
        public static bool LoadData()
        {
            return _saveData != null;
        }
        
        /// <summary>
        /// Returns the loaded Data
        /// </summary>
        /// <returns></returns>
        public static SaveData GetSaveData()
        {
            return _saveData;
        }

        /// <summary>
        /// Saves the the status of the Game in a JSON File
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
                // More Variables for Inventory
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
        
        /// <summary>
        /// Button Click to get back to the main menu
        /// </summary>
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
