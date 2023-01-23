using System.Collections;
using System.Collections.Generic;
using Code.Dialogue.Story;
using UnityEngine;

namespace Code.DataPersistence.Data
{
   public class GameData
   {
      // Root node
      private StoryNode _rootNode;
      private Story _story;

      public GameData()
      {
         //_rootNode = _story.GetRootNode();
         //var gameManager = GameObject.FindGameObjectWithTag("GameController");
         GameManager.LoadNewGame();
         //Debug.Log("This is the root node of the story in question!!!!!!!!!!!!!!");
      }
   }
}
