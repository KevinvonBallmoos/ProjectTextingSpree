using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.Graphs;
using UnityEditor.MPE;
using UnityEngine;

namespace Dialogue.Editor
{
    public class DialogueEditor : EditorWindow
    {
        // Dialogue
        private Dialogue _selectedDialogue = null;
        private LoadDialogueText _dialogueText;
        // Vector
        private Vector2 _scrollPosition;
        // Float
        private const float CanvasSize = 4000;
        private const float BackGround = 50;
        // Xml
        private XmlDocument _xmlDoc;
        private XmlNode _rootNode;
        
        // Style
        [NonSerialized] private GUIStyle _nodeStyle;
        [NonSerialized] private GUIStyle _playerNodeStyle;
        [NonSerialized] private GUIStyle _textFieldStyle;
        // Drag
        [NonSerialized] private DialogueNode _draggingNode = null;
        [NonSerialized] private Vector2 _draggingOffset;
        [NonSerialized] private bool _draggingCanvas = false;
        [NonSerialized] private Vector2 _draggingCanvasOffset;
        // Node
        [NonSerialized] private DialogueNode _creatingNode = null;
        [NonSerialized] private DialogueNode _creatingParentNode = null;
        [NonSerialized] private DialogueNode _deletingNode = null;
        [NonSerialized] private DialogueNode _linkinParentNode = null;
        // Count
        [NonSerialized] private int _choiceCount; 
        [NonSerialized] private int _nodeCount;

        /// <summary>
        /// Extends Unity with the Dialogue Editor, like the Scriptable Objects
        /// </summary>
        [MenuItem("Window/Dialogue Editor")]
        public static void ShowEditorWindow()
        {
            GetWindow(typeof(DialogueEditor), false, "Dialogue Editor");
        }

        // Asset Callback : In computer programming, a callback is executable code that is passed as an argument to other code.
        [OnOpenAsset(1)]
        public static bool OnOpenAsset(int instanceID, int line)
        {
            Dialogue dialogue = EditorUtility.InstanceIDToObject(instanceID) as Dialogue;
            if (dialogue != null)
            {
                ShowEditorWindow();
                return true;
            }

            return false;
        }

        /// <summary>
        /// Add the On SelectionChanged Event to the EventList
        /// </summary>
        private void OnEnable()
        {
            Selection.selectionChanged += OnSelectionChanged;

            _nodeStyle = new GUIStyle
            {
                normal =
                {
                    background = EditorGUIUtility.Load("node0") as Texture2D,
                    textColor = Color.white
                },
                padding = new RectOffset(20, 20, 20, 20),
                border = new RectOffset(12, 12, 12, 12)
            };

            _playerNodeStyle = new GUIStyle
            {
                normal =
                {
                    background = EditorGUIUtility.Load("node1") as Texture2D,
                    textColor = Color.white
                },
                padding = new RectOffset(20, 20, 20, 20),
                border = new RectOffset(12, 12, 12, 12)
            };
        }

        /// <summary>
        /// When the selection is changed
        /// </summary>
        private void OnSelectionChanged()
        {
            Dialogue newDialogue = Selection.activeObject as Dialogue;
            if (newDialogue != null)
            {
                _selectedDialogue = newDialogue;
                Repaint();
            }
        }

        /// <summary>
        /// Creates the Gui and its object fields
        /// </summary>
        private void OnGUI()
        {
            // If no Dialogue is selected, displays message below
            if (_selectedDialogue == null)
            {
                EditorGUILayout.LabelField("No Dialogue Selected.");
            }
            else
            {
                // Draw GUI
                ProcessEvents();
                _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

                // Draws canvas
                Rect canvas = GUILayoutUtility.GetRect(CanvasSize, CanvasSize);
                // Draw Image with Coordinates
                Texture2D backGroundTex = Resources.Load("background") as Texture2D;
                // Width and Height are how many times the images has to appear (tile)
                Rect texCoords = new Rect(0,0, CanvasSize / BackGround, CanvasSize / BackGround);
                //GUI.DrawTexture(canvas, backGroundTex);
                GUI.DrawTextureWithTexCoords(canvas, backGroundTex, texCoords);

                // Draw Nodes and Connection
                _choiceCount = 0;
                _nodeCount = 0;
                // Draws all Nodes
                foreach (var node in _selectedDialogue.GetAllNodes())
                {
                    if (node.IsPlayerSpeaking() && !node.IsRootNode())
                        _choiceCount++;
                    else if (!node.IsPlayerSpeaking() && !node.IsRootNode())
                        _nodeCount++;
                    if(DrawNode(node))
                        DrawConnections(node);
                    else
                    {
                        _creatingParentNode = null;
                        _creatingNode = null;
                    }
                }
                
                // // Draws connections between the nodes
                // foreach (var node in selectedDialogue.GetAllNodes())
                // {
                //     DrawConnections(node);
                // }
                
                EditorGUILayout.EndScrollView();

                // EDIT 01.11.22
                if (_creatingParentNode != null)
                {
                    _selectedDialogue.CreateNode(_creatingParentNode, false);
                    _creatingParentNode = null;
                }
                // END
                if (_creatingNode != null)
                {
                    _selectedDialogue.CreateNode(_creatingNode);
                    _creatingNode = null;
                }

                if (_deletingNode != null)
                {
                    _selectedDialogue.DeleteNode(_deletingNode);
                    _deletingNode = null;
                }
            }
        }
        
        #region DrawNode
        /// <summary>
        /// Extract Method with Refactor
        /// </summary>
        /// <param name="node"></param>
        private bool DrawNode(DialogueNode node)
        {
            _xmlDoc = new XmlDocument();
            _xmlDoc.Load($@"C:\Users\Kevin\Unity Projects\Test Dialogue\Assets\Story Files\{_selectedDialogue.name}.xml");
            _rootNode = _xmlDoc.SelectSingleNode("//Chapter1");
            
            if (!CheckCount()) return false;
            // When Player is speaking then the node is blue
            GUIStyle style = _nodeStyle;
            if (node.IsPlayerSpeaking())
                style = _playerNodeStyle;
            
            //node.rect = new Rect(node.rect.x + scrollPosition.x, node.rect.y + scrollPosition.y, 200, 100);
            GUILayout.BeginArea(node.GetRect(), style);

            // Create LabelField and pass the text from the xml File
            var text = "";
            if (node.IsRootNode())
                text = _rootNode.ChildNodes[0].Name;
            else
            {
                if (node.IsPlayerSpeaking())
                    text = _rootNode.ChildNodes[1].ChildNodes[_choiceCount - 1].Name + " " +
                           _rootNode.ChildNodes[1].ChildNodes[_choiceCount - 1].Attributes["id"].Value;
                else
                    text = _rootNode.ChildNodes[2].ChildNodes[_nodeCount - 1].Name + " " +
                           _rootNode.ChildNodes[2].ChildNodes[_nodeCount - 1].Attributes["id"].Value;
            }
            
            EditorGUILayout.LabelField(text);

            // Create TextField
            _textFieldStyle = new GUIStyle();
            _textFieldStyle.normal.textColor = Color.white;

            EditorGUILayout.TextArea(node.text);
            
            EditorGUI.BeginChangeCheck();
            // Safe last text and compare, if it has changed, set scriptable object dirty
            // Set Text
            SetText(node);
            
            GUILayout.BeginHorizontal();

            if (GUILayout.Button(" AI "))
                
                _creatingParentNode = node;
            if (GUILayout.Button(" Add "))
                _creatingNode = node;
            
            DrawLinkButtons(node);

            if (GUILayout.Button(" Del "))
                _deletingNode = node;
            
            GUILayout.EndHorizontal();
            EditorGUI.EndChangeCheck();
            GUILayout.EndArea();

            return true;
        }

        /// <summary>
        /// Checks if the count of nodes in Editor is not higher than the count of Nodes in the Xml
        /// </summary>
        /// <returns></returns>
        private bool CheckCount()
        {
            var choices = _rootNode.ChildNodes[1].ChildNodes.Count;
            var nodes = _rootNode.ChildNodes[2].ChildNodes.Count;
            
            if (_choiceCount > choices || _nodeCount > nodes)
                return false;
            else
                return true;
        }

        /// <summary>
        /// Sets the Text from the Xml to the according node
        /// </summary>
        /// <param name="node"></param>
        private void SetText(DialogueNode node)
        {
            if (node.IsRootNode())
            {
                node.text = _rootNode.ChildNodes[0].InnerText;
            }
            else
            {
                if (node.IsPlayerSpeaking()) // Get Choice Node
                    node.text = _rootNode.ChildNodes[1].ChildNodes[_choiceCount - 1].InnerText;
                else if (!node.IsPlayerSpeaking()) // Get Node Node
                    node.text = _rootNode.ChildNodes[2].ChildNodes[_nodeCount - 1].InnerText;
            }
        }
        #endregion

        /// <summary>
        /// Draws Nodes
        /// </summary>
        /// <param name="node"></param>
        private void DrawLinkButtons(DialogueNode node)
        {
            if (_linkinParentNode == null)
            {
                if (GUILayout.Button("Link"))
                {
                    _linkinParentNode = node;
                }
            }
            else if (_linkinParentNode == node)
            {
                if (GUILayout.Button("Cancel"))
                {
                    _linkinParentNode = null;
                }
            }
            else if (_linkinParentNode.GetChildren().Contains(node.name))
            {
                if (GUILayout.Button("Unlink"))
                {
                    _linkinParentNode.RemoveChildren(node.name);
                    _linkinParentNode = null;
                }
            }
            else
            {
                if (GUILayout.Button("Child"))
                {
                    _linkinParentNode.AddChildren(node.name);
                    _linkinParentNode = null;
                }
            }
        }

        /// <summary>
        /// Add Bezier Curve between the nodes to connect parent and childnodes
        /// </summary>
        /// <param name="node"></param>
        private void DrawConnections(DialogueNode node)
        {
            // Set the start point of the Bezier - parentNode
            Vector2 startPos = new Vector2(node.GetRect().xMax, node.GetRect().center.y);
            foreach (DialogueNode childNode in _selectedDialogue.GetAllChildren(node))
            {
                // Set the end point of the Bezier - childNode
                Vector2 endPos = new Vector2(childNode.GetRect().xMin, childNode.GetRect().center.y);
                // Set a offset for the Tangent
                Vector2 controlPointOffset = endPos - startPos;
                controlPointOffset.y = 0;
                controlPointOffset.x *= 0.8f;
                // Create Bezier
                Handles.DrawBezier(startPos, endPos,
                    startPos + controlPointOffset, endPos - controlPointOffset,
                    Color.white, null, 4f);
            }
        }

        #region ProcessEvents
        
        /// <summary>
        /// Different types of Events
        /// </summary>
        private void ProcessEvents()
        {
            // If Mouse Down is true 
            if (Event.current.type == EventType.MouseDown && _draggingNode == null)
            {
                _draggingNode = GetNodeAtPoint(Event.current.mousePosition + _scrollPosition);
                if (_draggingNode != null)
                {
                    _draggingOffset = new Vector2(_draggingNode.GetRect().position.x - Event.current.mousePosition.x,
                        _draggingNode.GetRect().position.y - Event.current.mousePosition.y);
                    Selection.activeObject = _draggingNode;
                }
                else
                {
                    _draggingCanvas = true;
                    _draggingCanvasOffset = Event.current.mousePosition + _scrollPosition;
                    Selection.activeObject = _selectedDialogue;
                }
                // Record offset
            }
            // If Mouse Drag is true
            else if (Event.current.type == EventType.MouseDrag && _draggingNode != null)
            {
                // Current mouse position
                _draggingNode.SetRect(Event.current.mousePosition + _draggingOffset);
                // When the GUI changes, also when the position of a node changes
                GUI.changed = true;
            }
            // When Mouse Drag and draggingCanvas is true
            else if (Event.current.type == EventType.MouseDrag && _draggingCanvas)
            {
                _scrollPosition = _draggingCanvasOffset - Event.current.mousePosition;
                GUI.changed = true;
            }
            // If Mouse Up is true 
            else if (Event.current.type == EventType.MouseUp && _draggingNode != null)
            {
                _draggingNode = null;
            }
            // Wheen MouseUp and draggingCanvas is true
            else if (Event.current.type == EventType.MouseUp && _draggingCanvas)
            {
                _draggingCanvas = false;
            }
        }
        
        /// <summary>
        /// Returns the current selected node
        /// </summary>
        /// <param name="point"></param>
        /// <returns>node</returns>
        private DialogueNode GetNodeAtPoint(Vector2 point)
        {
            DialogueNode returnNode = null;
            // for all nodes check if current Mouse position is true
            foreach (var node in _selectedDialogue.GetAllNodes())
                // if node contains the Mouse position return node
                if (node.GetRect().Contains(point))
                {
                    returnNode = node;
                    // return the node which is selected by user Mouse
                }

            return returnNode;
        }
        #endregion
    }
}