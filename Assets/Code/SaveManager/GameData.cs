using System;
using Code.Dialogue.Story;

namespace Code.SaveManager
{

    [System.Serializable]
    public class GameData
    {
        public readonly string Title; // SaveGame1, SaveGame2, SaveGame3
        public readonly int ProgressPercentage;
        public readonly int TimeSpent; // Save in minutes, then convert into new TimeSpan(), Work with StopWatch Class to count the running time
        public readonly string TimeOfSave;
        public readonly string CurrentChapter; // string because StoryNode is too much
        public readonly StoryNode ParentNode;
        public readonly bool IsStoryNode; 

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
