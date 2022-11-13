using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Dialogue
{
    public class PlayerConversant : MonoBehaviour
    {
        // Current Dialogue only for Testing purposes
        [SerializeField] private Dialogue currentDialogue;
        private DialogueNode currentNode = null;
        private bool isChoosing = false;

        /// <summary>
        /// Initializes the currentNode
        /// </summary>
        private void Awake()
        {
            Debug.Log(currentDialogue.name);
            currentNode = currentDialogue.GetRootNode();
        }

        public bool IsChoosing()
        {
            return isChoosing;
        }

        /// <summary>
        /// Gets the text of the Root node of the Dialog Editor
        /// </summary>
        /// <returns></returns>
        public string GetText()
        {
            if (currentNode == null)
                return "";
            else
                return currentNode.GetText();
        }

        /// <summary>
        /// Returns Choices
        /// </summary>
        /// <returns></returns>
        public IEnumerable<DialogueNode> GetChoices()
        {
           return currentDialogue.GetPlayerChildren(currentNode);
        }

        /// <summary>
        /// Select the choices
        /// </summary>
        /// <param name="chosenNode"></param>
        public void SelectChoice(DialogueNode chosenNode)
        {
            currentNode = chosenNode;
            isChoosing = false;
            Next();
            // Debug.Log(currentNode.GetChildren().Count());
            // if (currentNode.GetChildren().Any())
            //     isChoosing = false;
            // else
            //     isChoosing = true;
        }
        
        /// <summary>
        /// When the next Button is clicked
        /// The next Dialogue should appear
        /// </summary>
        public void Next()
        {
            int numOfPlayerResponses = currentDialogue.GetPlayerChildren(currentNode).Count();
            if (numOfPlayerResponses > 0)
            {
                isChoosing = true;
                return;
            }
            
            DialogueNode[] children = currentDialogue.GetAIChildren(currentNode).ToArray();
            int index = Random.Range(0, children.Length);
            currentNode = children[index];
        }

        /// <summary>
        /// If there is more dialog
        /// returns true
        /// </summary>
        /// <returns></returns>
        public bool HasNext()
        {
            return currentDialogue.GetAllChildren(currentNode).Any();
        }
    }
}
