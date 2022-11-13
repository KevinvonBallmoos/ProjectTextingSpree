using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

namespace Dialogue
{
    public class LoadDialogueText : MonoBehaviour
    {
        private XmlDocument _xmlDoc;
        private XmlNode _rootNode;

        public LoadDialogueText()
        {
            
        }
        
        public void LoadText(List<DialogueNode> nodes)
        {
            _xmlDoc = new XmlDocument();
                _xmlDoc.Load(@"C:\Users\Kevin\Unity Projects\Test Dialogue\Assets\Story Files\Chapter1");
                _rootNode = _xmlDoc.SelectSingleNode("\\Chapter1");
                
                // Player Nodes
                
                // foreach (XmlNode node in _rootNode.ChildNodes[0])
                // {
                //     for (int i = 0; i < nodes.Count; i++)
                //     {
                //         if (nodes[i].IsPlayerSpeaking())
                //         {
                //             nodes[i].SetText(node.InnerText);
                //         }
                //     }
                // }
                
                // NPC Nodes
                foreach (XmlNode child in _rootNode.ChildNodes[1])
                {
                    
                }
        }
    }
}