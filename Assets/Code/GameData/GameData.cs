using Code.Controller.GameController;
using Code.Controller.NodeController;
using Code.Dialogue.Story;

namespace Code.GameData
{
    /// <summary>
    /// Object Class to save the status of the Game
    /// </summary>
    /// <para name="author">Kevin von Ballmoos</para>
    /// <para name="date">30.01.2023</para>
    [System.Serializable]
    public class GameData
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
        public readonly StoryNodeController[] PastStoryNodes;
        public readonly StoryNodeController[] SelectedChoices;
        

		/// <summary>
		/// Constructor to save an Object of type GameData
		/// </summary>
		/// <param name="saveData">Contains all necessary elements to save the status</param>
		public GameData (SaveData saveData)
        {
            PlayerName = saveData.PlayerName;
            PlayerBackground = saveData.PlayerBackground;
            Title = saveData.Title;
            ProgressPercentage = saveData.ProgressPercentage;
            TimeSpent = saveData.TimeSpent;
            TimeOfSave = saveData.TimeOfSave;
            CurrentChapter = saveData.CurrentChapter;
            ParentNode = saveData.ParentNode;
            IsStoryNode = saveData.IsStoryNode;
            NodeIndex = saveData.NodeIndex;
            PastStoryNodes = saveData.PastStoryNodes;
            SelectedChoices = saveData.SelectedChoices;
        }
   }
}
