using System;
using System.Xml;
using UnityEditor;
using UnityEngine;

using Code.Logger;
using UnityEditor.Callbacks;

namespace Code.Dialogue.Story 
{
    /// <summary>
    /// Story Editor, Creates Nodes used for the Story
    /// </summary>
    /// <para name="author">Kevin von Ballmoos</para>
    /// <para name="date">04.12.2022</para>
    public class StoryEditor : EditorWindow
    {
        // Logger
        private readonly GameLogger _logger = new GameLogger("StoryEditor");
        // Story
        private Story _selectedChapter;
        // Vector
        private Vector2 _scrollPosition;
        // Rect
        private const float CanvasSize = 4000;
        private const float BackGround = 50;
        // Xml
        private XmlDocument _xmlDoc;
        private XmlNode _rootNode;
        // Node style
        [NonSerialized] private GUIStyle _storyNodeStyle;
        [NonSerialized] private GUIStyle _choiceNodeStyle;
        [NonSerialized] private GUIStyle _textAreaStyle;
        // Drag
        [NonSerialized] private StoryNode _storyNode;
        [NonSerialized] private bool _dragCanvas;
        [NonSerialized] private Vector2 _dragOffset;
        [NonSerialized] private Vector2 _dragCanvasOffset;
        // Node
        [NonSerialized] private StoryNode _createChoiceNode ;
        [NonSerialized] private StoryNode _createStoryNode ;
        [NonSerialized] private StoryNode _deleteNode;
        [NonSerialized] private StoryNode _linkParentNode;
        // Count
        [NonSerialized] private int _choiceNodeCount; 
        [NonSerialized] private int _storyNodeCount;

        #region StoryWindow
        
        /// <summary>
        /// Extends Unity with the Story Editor
        /// </summary>
        [MenuItem("Window/Story Editor")]
        public static void ShowEditorWindow()
        {
            GetWindow(typeof(StoryEditor), false, "Story Editor");
        }
        
        /// <summary>
        /// Shows the Editor Window, depending if a Story is loaded or not
        /// Asset Callback : In computer programming, a callback is executable code that is passed as an argument to other code.
        /// </summary>
        /// <param name="instanceId"></param>
        /// <returns>true when a Story is loaded and false when not</returns>
        [OnOpenAsset(1)]
        public static bool OnOpenAsset(int instanceId)
        {
            var story = EditorUtility.InstanceIDToObject(instanceId) as Story;
            if (story == null) return false;
            ShowEditorWindow();
            return true;
        }
        
        /// <summary>
        /// When the Dialog is Enabled, initialize the node Styles.
        /// </summary>
        private void OnEnable()
        {
            Selection.selectionChanged += OnSelectionChanged;

            _storyNodeStyle = new GUIStyle
            {
                normal =
                {
                    background = EditorGUIUtility.Load("node0") as Texture2D,
                    textColor = Color.white
                },
                padding = new RectOffset(20, 20, 20, 20),
                border = new RectOffset(12, 12, 12, 12)
            };

            _choiceNodeStyle = new GUIStyle
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
        #endregion
        
        /// <summary>
        /// Loads the selected chapter
        /// </summary>
        private void OnSelectionChanged()
        {
            var newChapter = Selection.activeObject as Story;
            if (newChapter == null) return;
            _selectedChapter = newChapter;
            Repaint();
        }

        #region Draw GUI

        /// <summary>
        /// Creates the Rect, the Editor and the Nodes 
        /// </summary>
        private void OnGUI()
        {
            // If no Dialogue is selected, displays message below
            if (_selectedChapter == null)
            {
                EditorGUILayout.LabelField("No Dialogue Selected.");
            }
            else
            {
                // Mouse Events
                ProcessEvents();
                
                _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);
                // Draw Surface
                DrawSurface();
                    
                _choiceNodeCount = 0;
                 _storyNodeCount = 0;
                foreach (var node in _selectedChapter.GetAllNodes())
                {
                    if (node.IsChoiceNode())
                        _choiceNodeCount++;
                    else // if (!node.IsChoiceNode() //&& !node.IsRootNode()) EDIT
                        _storyNodeCount++;
                    if (DrawNode(node))
                        DrawConnections(node);
                    else
                    {
                        _createStoryNode = null;
                        _createChoiceNode = null;
                    } 
                }

                EditorGUILayout.EndScrollView();
                if (_createStoryNode != null)
                {
                    _logger.LogEntry("Editor log", "Add Story Node", GameLogger.GetLineNumber());

                    _selectedChapter.AddNode(_createStoryNode, false);
                    _createStoryNode = null;
                }

                if (_createChoiceNode != null)
                {
                    _logger.LogEntry("Editor log", "Add Choice Node", GameLogger.GetLineNumber());

                    _selectedChapter.AddNode(_createChoiceNode, true);
                    _createChoiceNode = null;
                }

                if (_deleteNode != null)
                {
                    if (_deleteNode.IsChoiceNode())
                        _choiceNodeCount--;
                    else if (!_deleteNode.IsChoiceNode())
                        _storyNodeCount--;
                    _selectedChapter.DeleteNode(_deleteNode);
                    _deleteNode = null;
                }
            }
        }

        /// <summary>
        /// Draws the Surface of the Editor
        /// </summary>
        private static void DrawSurface()
        {
            // Draws canvas
            var canvas = GUILayoutUtility.GetRect(CanvasSize, CanvasSize);
            // Draw Background
            var backGroundTex = Resources.Load("Editor/background") as Texture2D;
            // Width and Height are how many times the images has to appear (tile)
            var texCoords = new Rect(0,0, CanvasSize / BackGround, CanvasSize / BackGround);
            // Draw Surface
            GUI.DrawTextureWithTexCoords(canvas, backGroundTex, texCoords);
        }
        #endregion

        #region Draw Nodes and Connections

        /// <summary>
        /// Draws node from Xml File
        /// </summary>
        /// <param name="node">Next Node to Draw</param>
        private bool DrawNode(StoryNode node)
        {
            _xmlDoc = new XmlDocument();
            _xmlDoc.Load($@"{Application.dataPath}/StoryFiles/{_selectedChapter.name}.xml");
            _rootNode = _xmlDoc.SelectSingleNode($"//{_selectedChapter.name}"); // TODO In Doc rein
            
            if (!CheckCount()) return false;
            
            var style = _storyNodeStyle;
            if (node.IsChoiceNode())
                style = _choiceNodeStyle; 
            
            GUILayout.BeginArea(node.GetRect(), style);
            
            // Create LabelField and pass the text from the xml File
            var text = "";
            // if (node.IsRootNode())
            //     text = _rootNode.ChildNodes[0].Name;
            // else
            // {
                if (node.IsChoiceNode())
                {
                    text = _rootNode.ChildNodes[1].ChildNodes[_choiceNodeCount - 1].Name + " " +
                           _rootNode.ChildNodes[1].ChildNodes[_choiceNodeCount - 1].Attributes?["id"].Value;
                }
                else
                {
                    text = _rootNode.ChildNodes[2].ChildNodes[_storyNodeCount - 1].Name + " " +
                           _rootNode.ChildNodes[2].ChildNodes[_storyNodeCount - 1].Attributes?["id"].Value;
                }
            //} EDIT
            EditorGUILayout.LabelField(text);
            
            // Create TextField
            _textAreaStyle = new GUIStyle();
            _textAreaStyle.normal.textColor = Color.white;

            EditorGUILayout.TextArea(node.GetText());
            
            EditorGUI.BeginChangeCheck();
            
            SetText(node);
            
            if (!node.IsChoiceNode())
                SetProperties(node);
            
            GUILayout.BeginHorizontal();

            if (GUILayout.Button(" Story "))
                 _createStoryNode = node;

            if (GUILayout.Button(" Choice "))
                _createChoiceNode = node;

            DrawLinkButtons(node);
            
            if (GUILayout.Button(" Remove "))
                _deleteNode = node;

            GUILayout.EndHorizontal();
            EditorGUI.EndChangeCheck();
            GUILayout.EndArea();
            
            return true;
        }
        /// <summary>
        /// Checks if the count of nodes in Editor is not higher than the count of Nodes in the Xml
        /// </summary>
        /// <returns>true when the count is node higher than the count of Nodes in the File</returns>
        private bool CheckCount()
        {
            var choices = _rootNode.ChildNodes[1].ChildNodes.Count;
            var nodes = _rootNode.ChildNodes[2].ChildNodes.Count;
            
            if (_choiceNodeCount > choices || _storyNodeCount > nodes)
                return false;
            
            return true;
        }

        /// <summary>
        /// Sets the Text from the Xml to the according node
        /// </summary>
        /// <param name="node"></param>
        private void SetText(StoryNode node)
        {
            // if (node.IsRootNode())
            // {
            //     node.SetText(_rootNode.ChildNodes[0].InnerText);
            // }
            // else
            // {
                if (node.IsChoiceNode())
                    node.SetText(_rootNode.ChildNodes[1].ChildNodes[_choiceNodeCount - 1].InnerText);
                else if (!node.IsChoiceNode())
                    node.SetText(_rootNode.ChildNodes[2].ChildNodes[_storyNodeCount - 1].InnerText);
            //} EDIT
        }
        
        /// <summary>
        /// Sets the Properties from the Xml to the according node
        /// </summary>
        /// <param name="node"></param>
        private void SetProperties(StoryNode node)
        {
            int? attributesCount = 0;
            var xmlNode = _rootNode.ChildNodes[2].ChildNodes[_storyNodeCount - 1];
            try  { attributesCount = xmlNode.Attributes?.Count; }
            catch (Exception) 
            {
                // ignored
            }
            
            if (attributesCount == 0) return;

            for (int i = 0; i < attributesCount; i++)
            {
                var attribute = xmlNode.Attributes[i].Name;
                switch (attribute)
                {
                    case "image":
                        node.SetImage(xmlNode.Attributes[attribute].Value);
                        break;
                    case "isGameOver":
                        node.SetIsGameOver(Convert.ToBoolean(xmlNode.Attributes[attribute].Value));
                        break;
                    case "isEndOfChapter":
                        node.SetIsEndOfChapter(Convert.ToBoolean(xmlNode.Attributes[attribute].Value));
                        break;
                    case "isEndOfStory":
                        node.SetIsEndOfStory(Convert.ToBoolean(xmlNode.Attributes[attribute].Value));
                        break;
                }
            }
        }
        
        /// <summary>
        /// Draws additional Buttons
        /// </summary>
        /// <param name="node">Node to draw the Buttons on</param>
        private void DrawLinkButtons(StoryNode node)
        {
            if (_linkParentNode == null)
            {
                if (GUILayout.Button("Link"))
                    _linkParentNode = node;
            }
            else if (_linkParentNode == node)
            {
                if (GUILayout.Button("Cancel"))
                    _linkParentNode = null;
            }
            else if (_linkParentNode.GetChildNodes().Contains(node.name))
            {
                if (GUILayout.Button("Unlink"))
                {
                    _linkParentNode.RemoveChildNode(node.name);
                    _linkParentNode = null;
                }
            }
            else
            {
                if (GUILayout.Button("Child"))
                {
                    _linkParentNode.AddChildNode(node.name);
                    _linkParentNode = null;
                }
            }
        }

        /// <summary>
        /// Add Bezier Curve between the nodes to connect parent and child nodes
        /// </summary>
        /// <param name="node"></param>
        private void DrawConnections(StoryNode node)
        {
            // Set the start point of the Bezier - parentNode
            Vector2 startPos = new Vector2(node.GetRect().xMax, node.GetRect().center.y);
            foreach (StoryNode childNode in _selectedChapter.GetAllChildNodes(node))
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


        #endregion
        
        #region ProcessEvents
        
        /// <summary>
        /// Different Mouse Events such as
        /// Mouse Down
        /// Mouse Up
        /// Mouse Dragging
        /// </summary>
        private void ProcessEvents()
        {
            switch (Event.current.type)
            {
                // Mouse Down is true 
                case EventType.MouseDown when _storyNode == null:
                {
                    _storyNode = GetNodeAtPoint(Event.current.mousePosition + _scrollPosition);
                    if (_storyNode != null)
                    {
                        _dragOffset = new Vector2(_storyNode.GetRect().position.x - Event.current.mousePosition.x,
                            _storyNode.GetRect().position.y - Event.current.mousePosition.y);
                        Selection.activeObject = _storyNode;
                    }
                    else
                    {
                        _dragCanvas = true;
                        _dragCanvasOffset = Event.current.mousePosition + _scrollPosition;
                        Selection.activeObject = _selectedChapter;
                    }
                    break;
                }
                // Mouse Drag is true
                case EventType.MouseDrag when _storyNode != null:
                    _storyNode.SetRect(Event.current.mousePosition.x + _dragOffset.x, Event.current.mousePosition.y + _dragOffset.y );
                    GUI.changed = true;
                    break;
                // Mouse Drag and draggingCanvas is true
                case EventType.MouseDrag when _dragCanvas:
                    _scrollPosition = _dragCanvasOffset - Event.current.mousePosition;
                    GUI.changed = true;
                    break;
                // Mouse Up is true 
                case EventType.MouseUp when _storyNode != null:
                    _storyNode = null;
                    break;
                // MouseUp and draggingCanvas is true
                case EventType.MouseUp when _dragCanvas:
                    _dragCanvas = false;
                    break;
            }
        }
        
        /// <summary>
        /// Returns the current selected node
        /// If node contains the Mouse position return node
        /// </summary>
        /// <param name="point">Point where the Mouse currently is</param>
        /// <returns>node</returns>
        private StoryNode GetNodeAtPoint(Vector2 point)
        {
            StoryNode selectedNode = null;
            foreach (var node in _selectedChapter.GetAllNodes())
                if (node.GetRect().Contains(point))
                    selectedNode = node;
                
            return selectedNode;
        }
        #endregion
    }
}