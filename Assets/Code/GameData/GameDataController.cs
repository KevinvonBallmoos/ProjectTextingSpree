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

using Code.Controller;
using Code.Dialogue.Story;
using Code.Logger;

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
        public double ProgressPercentage { get; set; }
        public string TimeSpent { get; set; }
        public string TimeOfSave { get; set; }
        public string CurrentChapter { get; set; }
        public string ParentNode { get; set; }
        public bool IsStoryNode { get; set; }
        public string NodeIndex { get; set; }
        public StoryNode[] PastStoryNodes { get; set; }
        public StoryNode[] SelectedChoices { get; set; }
    }
    
    /// <summary>
    /// Saves and Loads the status of the Game
    /// </summary>
    /// <para name="author">Kevin von Ballmoos</para>
    /// <para name="date">30.01.2023</para>
    public class GameDataController : MonoBehaviour
    {
        // Logger
        private readonly GameLogger _logger = new GameLogger("GameManager");
        // Load save
        [SerializeField] private Text loadGameText;
        // GameObjects
        [SerializeField] private GameObject saveSlot1;
        [SerializeField] private GameObject saveSlot2;
        [SerializeField] private GameObject saveSlot3;
        // Screens
        [SerializeField] private GameObject mainMenuScreen;
        [SerializeField] private GameObject messageBoxScreen; 
        [SerializeField] private GameObject characterPropertiesScreen;
        // Slot view
        public GameObject slotView;
        // GameDataController
        public static GameDataController Gdc;
        // SaveData
        private static SaveData _saveData;
        private static readonly List<SaveData> LoadedData = new ();
        private static int _slotNum;
        private static string _filename;
        private static string _playerName;
        private static string _playerBackground;
        // Save Time
        private static string _saveTime;

        #region Awake and Start
        
        /// <summary>
        /// Sets the language of the program to en-US
        /// </summary>
        private void Awake()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            
            if (Gdc == null)
                Gdc = this;
            else
                Destroy(gameObject);
        }
        
        /// <summary>
        /// If there are any save files, then the LoadSaveGame Button in the Menu is activated
        /// </summary>
        private void Start()
        {
            TimeAndProgress.StartTime();
        }
        
        #endregion

        #region Game States
        
        /// <summary>
        /// When a new Game is started, it checks for a open save slot, are there none,
        /// then the User has to choose an old save slot to override the date with the new Game
        /// </summary>
        public bool NewGame(string player, string background)
        {
            _saveTime = "";
            PlayerName = player;
            PlayerBackground = background;
            var length = Directory.GetFiles(Application.persistentDataPath + "/SaveData").Length;
            if (length == 3)
                return false;
            
            _slotNum = length;
            
            SaveNewGame();
            return true;
        }

        /// <summary>
        /// Sets the SaveScreen and loads the data
        /// </summary>
        public void LoadGame()
        {
            SetSaveScreen("LOAD", 0);
            LoadDataIntoSlots();
        }

        /// <summary>
        /// Resets the Images of the Slots
        /// </summary>
        /// <param name="text"></param>
        /// <param name="index"></param>
        public void SetSaveScreen(string text, int index)
        {
            var slots = slotView.GetComponentsInChildren<Image>();
            for (var i = 0; i < slots.Length; i++)
            {
                if (i is 1 or 3 or 5)
                    slots[i].enabled = false;
            }
            loadGameText.text = text;
            slotView.GetComponentsInChildren<Text>()[0].text = XmlController.GetInformationText(index);
        }
        
        #endregion

        #region Button Events

        /// <summary>
        /// Loads the selected Game
        /// </summary>
        public void LoadGame_Click()
        {
            SetSlotNum();
            
            switch (loadGameText.text)
            {
                case "LOAD":
                    GameManager.ActiveScene = 2;
                    LoadSelectedGame();
                    break;
                case "NEW GAME":
                    GameManager.ActiveScene = 1;
                    GameManager.LoadScene();
                    break;
            }
        }
        
        /// <summary>
        /// When continue is clicked, the User can select a save slot to override the old data
        /// </summary>
        public void Continue_Click()
        {
            mainMenuScreen.SetActive(true);
            messageBoxScreen.SetActive(false);
        }
        
        /// <summary>
        /// The Messagebox closes
        /// </summary>
        public void Cancel_CLick()
        {
            messageBoxScreen.SetActive(false);
        }
        
        /// <summary>
        /// Button Click to get back to the main menu
        /// </summary>
        public void BackToMenu_Click()
        {
            mainMenuScreen.SetActive(true);
            characterPropertiesScreen.SetActive(false);
        }

        #endregion
        
        #region SlotView

        /// <summary>
        /// When the LoadGame Button is clicked, then the save files getting loaded into the save-slots
        /// Is there no save for a slot, then the slot stays empty
        /// </summary>
        private void LoadDataIntoSlots()
        {
            GetLoadedData();

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
        /// Reloads the save files
        /// </summary>
        private static void GetLoadedData()
        {
            LoadedData.Clear();
            foreach (var file in Directory.GetFiles(Application.persistentDataPath + "/SaveData"))
            {
                var json = File.ReadAllText(file, Encoding.UTF8);
                LoadedData.Add(JsonConvert.DeserializeObject<SaveData>(json));
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

            for (var i = 0; i < 5; i++)
            {
                if (slotObject == null) continue;
                var obj = slotObject.transform.GetChild(i).gameObject;
                var time = DateTime.Now;
                if (LoadedData[slotNum].TimeOfSave != null) 
                    time = DateTime.ParseExact(LoadedData[slotNum].TimeOfSave, "yyyy-dd-M--HH-mm-ss",
                        CultureInfo.InvariantCulture);
                obj.GetComponent<TextMeshProUGUI>().text = i switch
                {
                    0 => $"Player: {LoadedData[slotNum].PlayerName}",
                    1 => $"Chapter: {LoadedData[slotNum].Title}",
                    2 => $"Completion: {LoadedData[slotNum].ProgressPercentage} %",
                    3 => $"Time of last Save: \n{time:dddd, dd MMMM yyyy. HH:mm:ss}",
                    4 => $"Time spent in Game: {LoadedData[slotNum].TimeSpent}",
                    _ => obj.GetComponent<TextMeshProUGUI>().text
                };
            }
        }

        /// <summary>
        /// Updates the Slot view with empty data, if there is no save 
        /// </summary>
        /// <param name="slotNum">Slot number where the save data has to be placed</param>
        public void UpdateEmptySlot(int slotNum)
        {
            var slotObject = slotNum switch
            {
                1 => saveSlot1,
                2 => saveSlot2,
                3 => saveSlot3,
                _ => null
            };

            for (var i = 0; i < 5; i++)
            {
                if (slotObject == null) continue;
                var obj = slotObject.transform.GetChild(i).gameObject;
                obj.GetComponent<TextMeshProUGUI>().text = i switch
                {
                    0 => $"Player: No data",
                    1 => "Chapter: No data saved",
                    2 => "Completion: ... %",
                    3 => "Time of last Save: No data",
                    4 => "Time spent in Game: 00:00:00",
                    _ => obj.GetComponent<TextMeshProUGUI>().text
                };
            }
        }

        /// <summary>
        /// Loads the Selected Game
        /// </summary>
        private void LoadSelectedGame()
        {
            var files = Directory.GetFiles(Application.persistentDataPath + "/SaveData");
            
            if (files.Length <= _slotNum) return;
            var json = File.ReadAllText(files[_slotNum], Encoding.UTF8);

            try
            {
                _saveData = JsonConvert.DeserializeObject<SaveData>(json);
            }
            catch (Exception)
            {
                LoadSelectedGame();
            }

            PlayerName = _saveData?.PlayerName;
            PlayerBackground = _saveData?.PlayerBackground;
            GameManager.LoadScene();
        }

        /// <summary>
        /// Checks on which save slot the slotImage is active
        /// and saves the slot number
        /// </summary>
        /// <returns></returns>
        private void SetSlotNum()
        {
            var slots = slotView.GetComponentsInChildren<Image>();
            for (var i = 1; i < slots.Length; i += 2)
            {
                if (!slots[i].enabled) continue;
                _slotNum = (i - 1) / 2;
                break;
            }
        }

        /// <summary>
        /// Returns the slot number
        /// </summary>
        /// <returns></returns>
        public int GetSlotNum()
        {
            return _slotNum;
        }
        
        #endregion

        #region Loaded Data
        
        /// <summary>
        /// Returns true if Data has been loaded,
        /// else when not
        /// </summary>
        /// <returns></returns>
        public static bool LoadData()
        {
            return _saveData == null;
        }
        
        /// <summary>
        /// Returns the loaded Data
        /// </summary>
        /// <returns></returns>
        public static SaveData GetSaveData()
        {
            return _saveData;
        }

        #endregion
        
        #region Save
        
        /// <summary>
        /// Saves the name and the Character Properties as first Save for a New Game
        /// </summary>
        private void SaveNewGame()
        {
            
            _saveTime = DateTime.Now.ToString("yyyy-dd-M--HH-mm-ss");
            var gameData = new GameData(new SaveData
            {
                PlayerName = PlayerName,
                PlayerBackground = PlayerBackground, 
                Title = "",
                ProgressPercentage = 0,
                TimeSpent = "00.00.00",
                TimeOfSave = _saveTime,
                CurrentChapter = "",
                ParentNode = "",
                IsStoryNode = false,
                NodeIndex = "0",
                PastStoryNodes = null,
                SelectedChoices = null
            });
            var json = JsonConvert.SerializeObject(gameData, Formatting.Indented);
            _filename = Application.persistentDataPath + "/SaveData" +
                        $"/SaveGame_{Directory.GetFiles(Application.persistentDataPath + "/SaveData").Count() + 1}_{_saveTime}.json";
 
            File.WriteAllText(_filename, json);
        }

        /// <summary>
        /// Saves the the status of the Game in a JSON File
        /// </summary>
        /// <param name="save">The SaveData sent from StoryUI</param>
        public void SaveGame(SaveData save)
        {
            GetLoadedData();
            _saveTime = DateTime.Now.ToString("yyyy-dd-M--HH-mm-ss");
            TimeAndProgress.StopTime();

            var time = LoadedData[_slotNum].TimeSpent;
            var timeSaved = time == "00.00.00" ? TimeSpan.Zero : TimeSpan.Parse(time);
            var elapsedTime = Math.Floor(timeSaved.TotalSeconds + TimeAndProgress.GetElapsedTime().TotalSeconds);

            var progress = TimeAndProgress.GetProgress(save.ParentNode);
            if (progress <= LoadedData[_slotNum].ProgressPercentage)
                progress += Math.Round(LoadedData[_slotNum].ProgressPercentage, 2);
            
            var gameData = new GameData(
                new SaveData
                {
                    PlayerName = PlayerName,
                    PlayerBackground = PlayerBackground,
                    Title = save.Title,
                    ProgressPercentage = progress,
                    TimeSpent = TimeSpan.FromSeconds(elapsedTime).ToString(),
                    TimeOfSave = _saveTime,
                    CurrentChapter = GameObject.FindGameObjectWithTag("Story").GetComponent<StoryUI>()
                        .currentChapter
                        .name,
                    ParentNode = save.ParentNode,
                    IsStoryNode = save.IsStoryNode,
                    NodeIndex = save.NodeIndex,
                    PastStoryNodes = save.PastStoryNodes,
                    SelectedChoices = save.SelectedChoices,
                    // More Variables for Inventory
                }
            );
            var json = JsonConvert.SerializeObject(gameData, Formatting.Indented);
            _filename = Directory.GetFiles(Application.persistentDataPath + "/SaveData")[_slotNum];
            if (File.Exists(_filename))
                File.Delete(_filename);

            _filename = _filename[..^24] + _saveTime + ".json";
            File.WriteAllText(_filename, json);
            
            TimeAndProgress.StartTime();
        }
        
        #endregion

        #region Player and PlayerBackground
        
        public string PlayerName
        {
            get => _playerName;
            set => _playerName = value;
        }

        public string PlayerBackground
        {
            get => _playerBackground;
            set => _playerBackground = value;
        }

        #endregion

        #region Remove Data

        /// <summary>
        /// Asks if the User really want to delete the Saved Data
        /// </summary>
        public void RemoveData_Click()
        {
            GameManager.Gm.SetMessageBoxProperties(DataRemover.RemoveData_Click, XmlController.GetMessageBoxText(1));
            messageBoxScreen.SetActive(true);
            SetSlotNum();
        }

        #endregion
    }
}
