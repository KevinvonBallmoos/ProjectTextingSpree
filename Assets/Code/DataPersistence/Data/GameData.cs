using System.Collections;
using System.Collections.Generic;
using Code.Dialogue.Story;
using UnityEngine;

[System.Serializable]
public class GameData
{
   // Root node
   private StoryNode _rootNode;
   private Story _story;

   public GameData()
   {
      _rootNode = _story.GetRootNode();
      Debug.Log("This is the root node of the story in question!!!!!!!!!!!!!!");
   }
   
   
}
