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
using Code.Inventory;

namespace Code.GameData
{
    /// <summary>
    /// Saves the content of the Game
    /// </summary>
    /// <para name="author">Kevin von Ballmoos</para>
    /// <para name="date">30.01.2023</para>
    public class SaveData
    {
        public string PlayerName { get; set; }
        public string PlayerBackground { get; set; }
        public string Title { get; set; }
        public int ProgressPercentage { get; set; }
        public int TimeSpent { get; set; }
        public string TimeOfSave { get; set; }
        public string CurrentChapter { get; set; }
        public string ParentNode { get; set; }
        public bool IsStoryNode { get; set; }
    }
    
    /// <summary>
    /// Saves and Loads the status of the Game
    /// </summary>
    /// <para name="author">Kevin von Ballmoos</para>
    /// <para name="date">30.01.2023</para>
    public class GameDataController : MonoBehaviour
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
		[SerializeField] private GameObject characterPropertiesScreen;
        
        private static GameDataController _sm;
        // SaveData
        private static SaveData _saveData;
        private static readonly List<SaveData> LoadedData = new ();
        private static int _slotNum;
        private static bool _isNewGame;
        private static bool _isOverrideNewGame;
        private static string _filename;
        private static string _playername;
        private static string _playerBackground;

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
        public bool NewGame()
        {
            // TODO Save first time only name and background
            _isNewGame = true;
            var length = Directory.GetFiles(Application.persistentDataPath).Length;
            if (length == 3)
            {
                //overrideSaveGameScreen.SetActive(true);
                return false;
            }

            _slotNum = length;
			
			var saveTime = DateTime.Now.ToString("yyyy-dd-M--HH-mm-ss");
			var gameobject = characterPropertiesScreen.GetComponent<Text>();
			var saveData = new SaveData { PlayerName = gameobject.text, PlayerBackground = gameobject.text };
            var gameData = new GameData(saveData);
            var json = JsonConvert.SerializeObject(gameData, Formatting.Indented);
            _filename = Application.persistentDataPath +
                        $"\\SaveGame_{Directory.GetFiles(Application.persistentDataPath).Count() + 1}_{saveTime}.json";

			File.WriteAllText(_filename, json);
            //GameManager.LoadNewGame();
			return true;
        }

        /// <summary>
        /// Loads the Save Slots
        /// </summary>
        public void LoadGame()
        {
            _isNewGame = false;
            mainMenuScreen.SetActive(false);
            saveGameScreen.SetActive(true);
            LoadDataIntoSlots();
        }

        /// <summary>
        /// When continue is clicked, the User can select a save slot to override the old data
        /// </summary>
        public void Continue_Click()
        {
            _isOverrideNewGame = true;
            mainMenuScreen.SetActive(false);
            saveGameScreen.SetActive(true);
            overrideSaveGameScreen.SetActive(false);
            LoadDataIntoSlots();
        }
        
        /// <summary>
        /// When cancel is clicked, then the menu screen is visible
        /// </summary>
        public void Cancel_CLick()
        {
            overrideSaveGameScreen.SetActive(false);
        }
        
        /// <summary>
        /// Button Click to get back to the main menu
        /// </summary>
        public void BackToMenu_Click()
        {
            EnableOverrideText(false);
            mainMenuScreen.SetActive(true);
            saveGameScreen.SetActive(false);
            _isNewGame = false;
            _isOverrideNewGame = false;
            LoadedData.Clear();
        }

        /// <summary>
        /// When the LoadGame Button is clicked, then the save files getting loaded into the save-slots
        /// Is there no save for a slot, then the slot stays empty
        /// </summary>
        private void LoadDataIntoSlots()
        {
            if (_isNewGame)
                EnableOverrideText(true);
            
            foreach (var file in Directory.GetFiles(Application.persistentDataPath))
            {
                var json = File.ReadAllText(file, Encoding.UTF8);
                LoadedData.Add(JsonConvert.DeserializeObject<SaveData>(json));
            }
           
            // Load Data into save-slots
            for (var i = 0; i < LoadedData.Count; i++)
                UpdateSlotView(i);
            
            if (LoadedData.Count >= 3) return;
            {
                var length = 3 - LoadedData.Count;
                switch (length)
                {
                    case 1:
                    {
                        UpdateEmptySlot(3);
                        break;
                    }
                    case 2:
                    {
                        for (var i = 2; i <= 3; i++)
                            UpdateEmptySlot(i);
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Enables or Disables the Override text
        /// </summary>
        private void EnableOverrideText(bool isEnabled)
        {
            var btnObject = saveGameScreen.GetComponentInChildren<Button>().GetComponentsInChildren<Text>();
            foreach (var obj in btnObject)
            {
                if (obj.name == "OverrideInformation")
                    obj.enabled = isEnabled;
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
                0 => saveSlot1,
                1 => saveSlot2,
                2 => saveSlot3,
                _ => null
            };

            for (var i = 0; i < 4; i++)
            {
                var obj = slotObject.transform.GetChild(i).gameObject;
                var time = DateTime.ParseExact(LoadedData[slotNum].TimeOfSave, "yyyy-dd-M--HH-mm-ss",
                    CultureInfo.InvariantCulture);
                obj.GetComponent<TextMeshProUGUI>().text = i switch
                {
                    0 => $"Chapter: {LoadedData[slotNum].Title}",
                    1 => $"Completion: {LoadedData[slotNum].ProgressPercentage} %",
                    2 => $"Time of last Save: \n{time:dddd, dd MMMM yyyy. HH:mm:ss}",
                    3 => $"Time spent in Game: {LoadedData[slotNum].TimeSpent}",
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
        private static void LoadSelectedGame()
        {
            var files = Directory.GetFiles(Application.persistentDataPath);
            
            if (files.Length == _slotNum) return;
            var json = File.ReadAllText(files[_slotNum], Encoding.UTF8);
            
            _saveData = JsonConvert.DeserializeObject<SaveData>(json);
            GameManager.LoadSavedScene(int.Parse(_saveData.CurrentChapter[5].ToString()));
        }

        /// <summary>
        /// Override for a new Game or Load Game
        /// </summary>
        public void LoadSlot1_Click()
        {
            _slotNum = 0;
            if (_isOverrideNewGame)
                GameManager.LoadSavedScene(1);
            else
                LoadSelectedGame();
        }

        /// <summary>
        /// Override for a new Game or Load Game
        /// </summary>
        public void LoadSlot2_Click()
        {
            _slotNum = 1;
            if (_isOverrideNewGame)
                GameManager.LoadSavedScene(1);
            else
                LoadSelectedGame();
        }

        /// <summary>
        /// Override for a new Game or Load Game
        /// </summary>
        public void LoadSlot3_Click()
        { 
            _slotNum = 2;
            if (_isOverrideNewGame)
                GameManager.LoadSavedScene(1);
            else
                LoadSelectedGame();
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

            // TODO Save first time only name and background
            var saveTime = DateTime.Now.ToString("yyyy-dd-M--HH-mm-ss");
            var saveData = new SaveData
            {
                PlayerName = "", // if InputName.GetPlayername "" then take the GetPlayername instead
                PlayerBackground = "", // same here | ^
                Title = save.Title, // Title maybe missing in Dialog System
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
            if (_isNewGame && _isOverrideNewGame == false)
            {
                _filename = Application.persistentDataPath +
                            $"\\SaveGame_{Directory.GetFiles(Application.persistentDataPath).Count() + 1}_{saveTime}.json";
                File.WriteAllText(_filename, json);
                _isNewGame = false;
                _isOverrideNewGame = true;
            }
            else if (_isOverrideNewGame)
            {
                _filename = Directory.GetFiles(Application.persistentDataPath)[_slotNum];
                if (File.Exists(_filename))
                    File.Delete(_filename);

                _filename = _filename[..^24] + saveTime + ".json";
                File.WriteAllText(_filename, json);
            }
            // Application.persistentDataPath = C:\Users\Kevin\AppData\LocalLow\DefaultCompany
        }
        
        public static string GetPlayerName()
        {
            return _playername;
        }

        public static  string GetPlayerBackground()
        {
            return _playerBackground;
        }
    }
}
