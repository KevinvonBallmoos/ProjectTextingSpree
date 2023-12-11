using Newtonsoft.Json;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using UnityEngine;

using Code.Logger;
using Code.Model.GameData;

namespace Code.Controller.GameController
{
    /// <summary>
    /// This controller is responsible for loading and saving the game states
    /// </summary>
    /// <para name="author">Kevin von Ballmoos</para>
    /// <para name="date">30.01.2023</para>
    public class GameDataController : MonoBehaviour
    {
        // Logger
        private readonly GameLogger _logger = new GameLogger("GameManager");
        // GameDataController instance
        public static GameDataController Gdc;
        // GameDataModel
        private GameDataModel _gameData;
        // Time of save
        public string SaveTime { get; set; }

        #region Awake and Start
        
        /// <summary>
        /// Sets the language of the game to en-US
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
        /// In the game data the User can see how much time he spent already for the game
        /// </summary>
        private void Start()
        {
            TimeAndProgress.StartTime();
        }
        
        #endregion

        #region Game State
        
        /// <summary>
        /// There is a limit of 3 Savedata files
        /// When a new Game is started, it checks if there are 3 savedata files or not
        /// If not, it just creates a new one,
        /// else the User must choose an old game to override with the new Game
        /// </summary>
        public bool NewGame(string player, string background)
        {
            SaveTime = "";
            GameDataInfoModel.PlayerName = player;
            GameDataInfoModel.PlayerBackground = background;
            var length = Directory.GetFiles(Application.persistentDataPath + "/SaveData").Length;
            if (length == 3)
                return false;
            
            GameDataInfoModel.Placeholder = length;
            
            SaveNewGame(GameDataModel.SaveNewGame());
            return true;
        }
        
        #endregion
        
        #region Load Game state
        
        /// <summary>
        /// Loads the data for the selected game
        /// In case the deserializing throws an error, the method is executed again
        /// </summary>
        public void LoadSelectedGame()
        {
            var files = Directory.GetFiles(Application.persistentDataPath + "/SaveData");
            
            if (files.Length <= GameDataInfoModel.Placeholder) return;
            var json = File.ReadAllText(files[GameDataInfoModel.Placeholder], Encoding.UTF8); 
            _gameData = GetLoadedGameData(json);
            
            GameDataInfoModel.PlayerName = _gameData?.PlayerName;
            GameDataInfoModel.PlayerBackground = _gameData?.PlayerBackground;
            GameManager.LoadScene();
        }
        
        /// <summary>
        /// Returns the loaded Data
        /// </summary>
        /// <returns></returns>
        public GameDataModel GetSaveData()
        {
            return _gameData;
        }
        
        #endregion
        
        #region Save and Load Game Data
        
        /// <summary>
        /// Saves the game data in a json file
        /// </summary>
        /// <param name="gameData">Game data as a serializable object</param>
        private void SaveNewGame(GameDataSerializeModel gameData)
        {
            var json = JsonConvert.SerializeObject(gameData, Formatting.Indented);
            var filename = Application.persistentDataPath + "/SaveData" +
                        $"/SaveGame_{Directory.GetFiles(Application.persistentDataPath + "/SaveData").Count() + 1}_{SaveTime}.json";
 
            File.WriteAllText(filename, json);
        }

        /// <summary>
        /// Saves the the status of the Game in a Json File
        /// </summary>
        /// <param name="gameData">Game data as a serializable object</param>
        public void SaveRunningGame(GameDataSerializeModel gameData)
        {
            var json = JsonConvert.SerializeObject(gameData, Formatting.Indented);
            var filename = Directory.GetFiles(Application.persistentDataPath + "/SaveData")[GameDataInfoModel.Placeholder];
            if (File.Exists(filename))
                File.Delete(filename);

            filename = filename[..^24] + SaveTime + ".json";
            File.WriteAllText(filename, json);
            
            TimeAndProgress.StartTime();
        }
        
        /// <summary>
        /// Deserializes the data into the LoadedData List
        /// </summary>
        /// <param name="json">save data file to deserialize</param>
        public GameDataModel GetLoadedGameData(string json)
        {
            return JsonConvert.DeserializeObject<GameDataModel>(json);
        }
        
        #endregion
    }
}
