using Code.Dialogue.Story;

namespace Code.SaveManager
{
    /// <summary>
    /// Object Class to save the status of the Game
    /// </summary>
    /// <para name="author">Kevin von Ballmoos</para>
    /// <para name="date">30.01.2023</para>
    [System.Serializable]
    public class GameData
    {
        public readonly string Title;
        public readonly int ProgressPercentage;
        public readonly int TimeSpent;
        public readonly string TimeOfSave;
        public readonly string CurrentChapter;
        public readonly StoryNode ParentNode;
        public readonly bool IsStoryNode; 

        /// <summary>
        /// Constructor to save an Object of type GameData
        /// </summary>
        /// <param name="saveData"></param>
        public GameData (SaveData saveData)
        {
            Title = saveData.Title;
            ProgressPercentage = saveData.ProgressPercentage;
            TimeSpent = saveData.TimeSpent;
            TimeOfSave = saveData.TimeOfSave;
            CurrentChapter = saveData.CurrentChapter;
            ParentNode = saveData.ParentNode;
            IsStoryNode = saveData.IsStoryNode;
        }
   }
}
