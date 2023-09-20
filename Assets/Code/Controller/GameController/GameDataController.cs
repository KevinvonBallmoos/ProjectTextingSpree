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

using Code.Controller.FileControllers;
using Code.Dialogue.Story;
using Code.GameData;
using Code.Logger;

namespace Code.Controller.GameController
{
    /// <summary>
    /// This class is used to temporarily store the game data, until it is passed to the GameData class
    /// TODO: in separate File?
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
    /// This controller is responsible for loading and saving the game states
    /// </summary>
    /// <para name="author">Kevin von Ballmoos</para>
    /// <para name="date">30.01.2023</para>
    public class GameDataController : MonoBehaviour
    {
        // Logger
        private readonly GameLogger _logger = new GameLogger("GameManager");
        // GameDataController
        public static GameDataController Gdc;
        // Load save
        [Header("Load Game Text")]
        [SerializeField] private Text loadGameText;
        // Savedata Placeholders
        [Header("Savedata Placeholders")]
        [SerializeField] private GameObject[] placeholders;
        // Main Menu, Message Box and Character Screen Objects
        [Header("Main Menu, Message Box and Character Screens")]
        [SerializeField] private GameObject[] screenObjects;
        // Placeholder view
        [Header("Placeholder view")]
        [SerializeField] private GameObject placeholderView;
        // Placeholder Number
        private int _placeholderNum;
        // SaveData
        private static SaveData _saveData;
        private static readonly List<SaveData> LoadedData = new ();
        // Save Time
        private string _saveTime;
        
        // TODO : GetInstance Method like StoryViewer for Gdc

        #region Awake and Start
        
        /// <summary>
        /// Sets the language of the game to en-US
        /// TODO: Why again / reason? // Maybe its the Time spent in Game Line 291
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
        /// Starts the Timer
        /// In the save data the User can see how much time he spent already for the game
        /// </summary>
        private void Start()
        {
            TimeAndProgress.StartTime();
        }
        
        #endregion
        
        #region Button Events
        
        #region GameStates
        
        /// <summary>
        /// Starts either a new game or loads a selected one 
        /// </summary>
        public void LoadGame_Click()
        {
            SetPlaceholderNum();
            
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
        /// Button Click to get back to the main menu
        /// [0]: Disables the title screen
        /// [2]: Enables the character properties screen
        /// TODO : delete when unused
        /// </summary>
        public void BackToMenu_Click()
        {
            screenObjects[0].SetActive(true);
            screenObjects[2].SetActive(false);
        }
        
        #endregion
        
        #region Messagebox Buttons
        
        /// <summary>
        /// When continue is clicked, the User can choose a save to override the old data with the new Game
        /// [0]: Enables the title screen
        /// [1]: Disables the messagebox
        /// </summary>
        public void Continue_Click()
        {
            screenObjects[0].SetActive(true);
            screenObjects[1].SetActive(false);
        }
        
        /// <summary>
        /// Action to cancel the Messagebox
        /// [0]: Enables the title screen
        /// [1]: Disables the messagebox
        /// </summary>
        public void Cancel_CLick()
        {
            screenObjects[0].SetActive(true);
            screenObjects[1].SetActive(false);
        }
        
        #endregion
        
        #region Remove Data
        
        /// <summary>
        /// Display the messagebox with the according text, to remove a selected save
        /// [1]: Enables the messagebox
        /// </summary>
        public void RemoveData_Click()
        {
            GameManager.Gm.SetMessageBoxProperties(GameDataRemover.RemoveData_Click, XmlController.GetMessageBoxText(1));
            screenObjects[1].SetActive(true);
            SetPlaceholderNum();
        }
        
        #endregion

        #endregion

        #region Game States
        
        /// <summary>
        /// There is a limit of 3 Savedata files
        /// When a new Game is started, it checks if there are 3 savedata files or not
        /// If not, the it just creates a new on,
        /// else the User has to choose an old save to override with the new Game
        /// </summary>
        public bool NewGame(string player, string background)
        {
            _saveTime = "";
            PlayerName = player;
            PlayerBackground = background;
            var length = Directory.GetFiles(Application.persistentDataPath + "/SaveData").Length;
            if (length == 3)
                return false;
            
            _placeholderNum = length;
            
            SaveNewGame();
            return true;
        }

        /// <summary>
        /// Initializes the Save panel and loads the data into the placeholders
        /// </summary>
        public void LoadGame()
        {
            InitializeSaveDataPanel("LOAD", 0);
            LoadDataIntoPlaceholders();
        }

        /// <summary>
        /// Initializes the Savedata panel
        /// Disables the check Images of all placeholders
        /// Sets the Button text and the overview text
        /// </summary>
        /// <param name="text">Text for the Button, either Load Game or New Game</param>
        /// <param name="index">Identifies which text from the xml file should be displayed</param>
        public void InitializeSaveDataPanel(string text, int index)
        {
            var holders = placeholderView.GetComponentsInChildren<Image>();
            for (var i = 0; i < holders.Length; i++)
            {
                if (i is 1 or 3 or 5)
                    holders[i].enabled = false;
            }
            loadGameText.text = text;
            placeholderView.GetComponentsInChildren<Text>()[0].text = XmlController.GetInformationText(index);
        }
        
        #endregion
        
        #region Placeholder View

        /// <summary>
        /// Gets all save data files and stores the data in the placeholders
        /// </summary>
        public void LoadDataIntoPlaceholders()
        {
            GetLoadedData();
            
            for (var i = 0; i < 3; i++)
            {
                UpdatePlaceholderView(i, i < LoadedData.Count ? LoadedData : null);
            }
        }

        /// <summary>
        /// Searches all save files
        /// Deserializes the data into the LoadedData List
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
        /// Updates the Placeholder view with data.
        /// If loaded data is null, then the placeholder is empty
        /// </summary>
        /// <param name="placeholderNum">Placeholder number where the data has to be placed</param>
        /// <param name="loadedData">List of all loaded data to display</param>
        private void UpdatePlaceholderView(int placeholderNum, IReadOnlyList<SaveData> loadedData)
        {
            var placeholderObject = placeholderNum switch
            {
                0 => placeholders[0],
                1 => placeholders[1],
                2 => placeholders[2],
                _ => null
            };

            for (var i = 0; i < 5; i++)
            {
                if (placeholderObject == null) continue;
                
                var time = DateTime.Now;
                if (loadedData != null && loadedData[placeholderNum].TimeOfSave != null) 
                    time = DateTime.ParseExact(loadedData[placeholderNum].TimeOfSave, "yyyy-dd-M--HH-mm-ss", CultureInfo.InvariantCulture);
                
                var obj = placeholderObject.transform.GetChild(i).gameObject;
                var text = i switch
                {
                    0 => loadedData != null ? $"Player: {loadedData[placeholderNum].PlayerName}" : "Player: No data",
                    1 => loadedData != null ? $"Chapter: {loadedData[placeholderNum].Title}" : "Chapter: No data saved",
                    2 => loadedData != null ? $"Completion: {loadedData[placeholderNum].ProgressPercentage} %" : "Completion: ... %",
                    3 => loadedData != null ? $"Time of last Save: \n{time}" : "Time of last Save: No data",
                    4 => loadedData != null ? $"Time spent in Game: {loadedData[placeholderNum].TimeSpent}" : "Time spent in Game: 00:00:00",
                    _ => obj.GetComponent<TextMeshProUGUI>().text
                };
                obj.GetComponent<TextMeshProUGUI>().text = text;
            }
        }

        /// <summary>
        /// Checks on which placeholder the check Image is active and saves the number
        /// </summary>
        /// <returns></returns>
        private void SetPlaceholderNum()
        {
            var holders = placeholderView.GetComponentsInChildren<Image>();
            for (var i = 1; i < holders.Length; i += 2)
            {
                if (!holders[i].enabled) continue;
                _placeholderNum = (i - 1) / 2;
                break;
            }
        }

        /// <summary>
        /// Returns the placeholder number
        /// </summary>
        /// <returns></returns>
        public int GetPlaceholderNum()
        {
            return _placeholderNum;
        }
        
        #endregion
        
        #region Load Game
        
        /// <summary>
        /// Loads the data for the selected game
        /// In case the deserializing throws an error, the method is executed again
        /// TODO : Bug Fix: Try catch can result in endless loop
        /// </summary>
        private void LoadSelectedGame()
        {
            var files = Directory.GetFiles(Application.persistentDataPath + "/SaveData");
            
            if (files.Length <= _placeholderNum) return;
            var json = File.ReadAllText(files[_placeholderNum], Encoding.UTF8);

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
        /// Saves the name and the Character Properties as first Save for a New Game in a Json File
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
            var filename = Application.persistentDataPath + "/SaveData" +
                        $"/SaveGame_{Directory.GetFiles(Application.persistentDataPath + "/SaveData").Count() + 1}_{_saveTime}.json";
 
            File.WriteAllText(filename, json);
        }

        /// <summary>
        /// Saves the the status of the Game in a Json File
        /// </summary>
        /// <param name="save">The SaveData sent from StoryUI</param>
        public void SaveGame(SaveData save)
        {
            GetLoadedData();
            _saveTime = DateTime.Now.ToString("yyyy-dd-M--HH-mm-ss");
            TimeAndProgress.StopTime();

            var time = LoadedData[_placeholderNum].TimeSpent;
            var timeSaved = time == "00.00.00" ? TimeSpan.Zero : TimeSpan.Parse(time);
            var elapsedTime = Math.Floor(timeSaved.TotalSeconds + TimeAndProgress.GetElapsedTime().TotalSeconds);

            var progress = TimeAndProgress.GetProgress(save.ParentNode);
            if (progress <= LoadedData[_placeholderNum].ProgressPercentage)
                progress += Math.Round(LoadedData[_placeholderNum].ProgressPercentage, 2);
            
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
                    // TODO : More Variables for Inventory
                }
            );
            var json = JsonConvert.SerializeObject(gameData, Formatting.Indented);
            var filename = Directory.GetFiles(Application.persistentDataPath + "/SaveData")[_placeholderNum];
            if (File.Exists(filename))
                File.Delete(filename);

            filename = filename[..^24] + _saveTime + ".json";
            File.WriteAllText(filename, json);
            
            TimeAndProgress.StartTime();
        }
        
        #endregion

        #region Player and PlayerBackground
        
        /// <summary>
        /// Property for the Player name
        /// </summary>
        public string PlayerName
        {
            get;
            private set;
        }

        /// <summary>
        /// Property for the Player Background/Character
        /// </summary>
        public string PlayerBackground
        {
            get;
            private set;
        }

        #endregion
    }
}
