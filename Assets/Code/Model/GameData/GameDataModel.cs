using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

using Code.Controller.GameController;
using Code.Model.Dialogue.StoryModel;
using Code.Model.Node;
using Code.View.Dialogue.StoryView;

namespace Code.Model.GameData
{
    /// <summary>
    /// Class provides the GameDataModel and Methods to save a game
    /// </summary>
    public class GameDataModel
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
        public StoryNodeModel[] PastStoryNodes { get; set; }
        public StoryNodeModel[] SelectedChoices { get; set; }
        
        /// <summary>
        /// Creates a new GameDataSerialize Model to serialize the game data later
        /// </summary>
        /// <returns>game data as a serializable object</returns>
        public static GameDataSerializeModel SaveNewGame()
        {
            GameDataController.Gdc.SaveTime = DateTime.Now.ToString("yyyy-dd-M--HH-mm-ss");
            var gameData = new GameDataSerializeModel(new GameDataModel
            {
                PlayerName = GameDataInfoModel.PlayerName,
                PlayerBackground = GameDataInfoModel.PlayerBackground, 
                Title = "",
                ProgressPercentage = 0,
                TimeSpent = "00.00.00",
                TimeOfSave = GameDataController.Gdc.SaveTime,
                CurrentChapter = "",
                ParentNode = "",
                IsStoryNode = false,
                NodeIndex = "0",
                PastStoryNodes = null,
                SelectedChoices = null
            });
            return gameData;
        }
        
        /// <summary>
        /// Saves the the status of the Game in a Json File
        /// </summary>
        /// <param name="gameDataModel">The GameData to save</param>
        public static void SaveRunningGame(GameDataModel gameDataModel)
        {
            GameDataController.Gdc.SaveTime = DateTime.Now.ToString("yyyy-dd-M--HH-mm-ss");
            TimeAndProgress.StopTime();
            
            var loadedGameData = GetLoadedData();
            var placeHolderNum = GameDataInfoModel.Placeholder;
            var time = loadedGameData[placeHolderNum].TimeSpent;
            var timeSaved = time == "00.00.00" ? TimeSpan.Zero : TimeSpan.Parse(time);
            var elapsedTime = Math.Floor(timeSaved.TotalSeconds + TimeAndProgress.GetElapsedTime().TotalSeconds);

            var progress = TimeAndProgress.GetProgress(gameDataModel.ParentNode);
            if (progress <= loadedGameData[placeHolderNum].ProgressPercentage)
                progress += Math.Round(loadedGameData[placeHolderNum].ProgressPercentage, 2);

            try
            {

                var gameData = new GameDataSerializeModel(
                    new GameDataModel
                    {
                        PlayerName = GameDataInfoModel.PlayerName,
                        PlayerBackground = GameDataInfoModel.PlayerBackground,
                        Title = gameDataModel.Title,
                        ProgressPercentage = progress,
                        TimeSpent = TimeSpan.FromSeconds(elapsedTime).ToString(),
                        TimeOfSave = GameDataController.Gdc.SaveTime,
                        CurrentChapter = StoryAssetModel.CurrentAsset.name,
                        ParentNode = gameDataModel.ParentNode,
                        IsStoryNode = gameDataModel.IsStoryNode,
                        NodeIndex = gameDataModel.NodeIndex,
                        PastStoryNodes = gameDataModel.PastStoryNodes,
                        SelectedChoices = gameDataModel.SelectedChoices,
                        // TODO : More Variables for Inventory
                    }
                );
                GameDataController.Gdc.SaveRunningGame(gameData);
            }
            catch (Exception ex)
            {
                Debug.Log(ex.Message);    
            }
            
        }
        
        /// <summary>
        /// Searches all save files
        /// </summary>
        public static List<GameDataModel> GetLoadedData()
        {
            var loadedGameData = new List<GameDataModel>();
            foreach (var file in Directory.GetFiles(Application.persistentDataPath + "/SaveData"))
            {
                var json = File.ReadAllText(file, Encoding.UTF8);
                loadedGameData.Add(GameDataController.Gdc.GetLoadedGameData(json));
            }

            return loadedGameData;
        }
    }
}