using Code.Controller.GameController;
using Code.Model.Node;

namespace Code.Model.GameData
{
    /// <summary>
    /// Object Class to game the status of the Game
    /// </summary>
    /// <para name="author">Kevin von Ballmoos</para>
    /// <para name="date">30.01.2023</para>
    [System.Serializable]
    public class GameDataSerializeModel
    {
        public readonly string PlayerName;
        public readonly string PlayerBackground;
        public readonly string Title;
        public readonly double ProgressPercentage;
        public readonly string TimeSpent;
        public readonly string TimeOfSave;
        public readonly string CurrentChapter;
        public readonly string ParentNode;
        public readonly bool IsStoryNode;
        public readonly string NodeIndex;
        public readonly StoryNodeModel[] PastStoryNodes;
        public readonly StoryNodeModel[] SelectedChoices;

		/// <summary>
		/// Constructor to game an Object of type GameDataSerializeModel
		/// </summary>
		/// <param name="gameData">Contains all necessary elements to game the status</param>
		public GameDataSerializeModel (GameDataModel gameData)
        {
            PlayerName = gameData.PlayerName;
            PlayerBackground = gameData.PlayerBackground;
            Title = gameData.Title;
            ProgressPercentage = gameData.ProgressPercentage;
            TimeSpent = gameData.TimeSpent;
            TimeOfSave = gameData.TimeOfSave;
            CurrentChapter = gameData.CurrentChapter;
            ParentNode = gameData.ParentNode;
            IsStoryNode = gameData.IsStoryNode;
            NodeIndex = gameData.NodeIndex;
            PastStoryNodes = gameData.PastStoryNodes;
            SelectedChoices = gameData.SelectedChoices;
        }
    }
}
