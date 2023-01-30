using Code.Dialogue.Story;
using UnityEngine;

namespace Code.DataPersistence.Data
{
    [System.Serializable]
   public class GameData
   {
       public readonly Story CurrentChapter;
       public readonly StoryNode ParentNode;
       public readonly bool IsStoryNode;
       public readonly bool IsNull;

       public GameData (SaveData saveData)
       {
           CurrentChapter = saveData.CurrentChapter;
           ParentNode = saveData.ParentNode;
           IsStoryNode = saveData.IsStoryNode;
           IsNull = saveData.IsNull;
       }
   }
}
