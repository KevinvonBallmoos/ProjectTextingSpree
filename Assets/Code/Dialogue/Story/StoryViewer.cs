using System;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

using Code.Logger;
using Unity.VisualScripting;
using UnityEditor.Callbacks;


namespace Code.Dialogue.Story
{
    /// <summary>
    /// Displays the Story
    /// If there are 0 nodes then it reads them from the xml file
    /// </summary>
    /// <para name="author">Kevin von Ballmoos</para>
    /// <para name="date">10.05.2023</para>
    public class StoryViewer : EditorWindow
    {
        // Logger
        private readonly GameLogger _logger = new GameLogger("StoryViewer");
        // Story
        private StoryAsset _selectedChapter;
        // Vector
        private Vector2 _scrollPosition;
        private Vector2 _scrollPositionTextArea = Vector2.zero;
        private Vector2 _scrollbarDragStartPos;
        private float _scrollbarDragStartScrollPos;
        private float _scrollbarHeight;
        private float _maxScrollPosition;
        // Rect
        private const float CanvasSize = 4000;
        private const float BackGround = 50;
        // Node style
        [NonSerialized] private GUIStyle _storyNodeStyle;
        [NonSerialized] private GUIStyle _choiceNodeStyle;
        [NonSerialized] private GUIStyle _textAreaStyle;
        // Drag
        [NonSerialized] private StoryNode _storyNode;
        [NonSerialized] private bool _dragCanvas;
        [NonSerialized] private Vector2 _dragOffset;
        [NonSerialized] private Vector2 _dragCanvasOffset;
        
        // Object
        private Object[] _xmlFiles;
        // Text
        private string _textContent;

        #region StoryWindow

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
        /// Extends Unity with the Story Viewer
        /// </summary>
        [MenuItem("Window/Story Viewer")]
        public static void ShowEditorWindow()
        {
            GetWindow(typeof(StoryViewer), false, "Story Viewer");
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

            _textAreaStyle = new GUIStyle
            {
                normal =
                {
                    textColor = Color.white,
                    background = EditorGUIUtility.Load("node2") as Texture2D,
                },
                padding = new RectOffset(20,20,20,20),
                border = new RectOffset(12, 12, 12, 12),
                wordWrap = true
            };
            
            wantsMouseMove = true;
        }
        
        /// <summary>
        /// Loads the selected chapter
        /// </summary>
        private void OnSelectionChanged()
        {
            var newChapter = Selection.activeObject as StoryAsset;
            if (newChapter == null) return;
            
            _selectedChapter = null;
            _storyNode = null;
            _xmlFiles = Resources.LoadAll($@"StoryFiles/", typeof(TextAsset));
            foreach (var file in _xmlFiles)
            {
                if (file.name != newChapter.name) continue;
                _selectedChapter = newChapter;
                break;
            }
            if (_selectedChapter == null) return;
            
            StoryAsset.ReadNodes(_selectedChapter);

            Repaint();
        }
        
        #endregion
        
        #region OnGUI

        private void OnGUI()
        {
            // Mouse Events
            ProcessEvents();
            
            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);
            DrawSurface();

            if (_selectedChapter.GetAllNodes() == null) return;
            foreach (var node in _selectedChapter.GetAllNodes())
                DrawNode(node);
            
            foreach (var child in _selectedChapter.GetAllNodes())
            {
                if (!child.IsRootNode()) continue;
                DrawConnections(child);
                break;
            }
            EditorGUILayout.EndScrollView();
        }
        
        #endregion
        
        #region Nodes and Surface
        
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
        
        /// <summary>
        /// Draws node from Xml File
        /// </summary>
        /// <param name="node">Next Node to Draw</param>
        private void DrawNode(StoryNode node)
        {
            var style = _storyNodeStyle;
            if (node.IsChoiceNode())
                style = _choiceNodeStyle; 
            
            GUILayout.BeginArea(node.GetRect(), style);
            EditorGUILayout.LabelField(node.GetLabel());
            
            _scrollPositionTextArea = EditorGUILayout.BeginScrollView(_scrollPositionTextArea, GUILayout.Width(260), GUILayout.Height(110));
            EditorGUILayout.LabelField(node.GetText(), _textAreaStyle);
            EditorGUILayout.EndScrollView();

            GUILayout.EndArea();
        }
        
        /// <summary>
        /// Add Bezier Curve between the nodes to connect parent and child nodes
        /// </summary>
        /// <param name="node"></param>
        private void DrawConnections(StoryNode node)
        {
            var children = node.GetChildNodes();
            if (children.Count == 0) return;

            for (var i = 0; i < children.Count; i++)
            {
                var child = _selectedChapter.GetAllChildNodes(node)[i];
                // Set the start point of the Bezier - parentNode
                var startPos = new Vector2(node.GetRect().xMax, node.GetRect().center.y);

                // Set the end point of the Bezier - childNode
                var endPos = new Vector2(child.GetRect().xMin, child.GetRect().center.y);
                // Set a offset for the Tangent
                var controlPointOffset = endPos - startPos;
                controlPointOffset.y = 0;
                controlPointOffset.x *= 0.8f;
                // Create Bezier
                Handles.DrawBezier(startPos, endPos,
                    startPos + controlPointOffset, endPos - controlPointOffset,
                    Color.white, null, 4f);
                DrawConnections(child);
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
            var e = Event.current;

            switch (e.type)
            {
                // Mouse Down is true 
                case EventType.MouseDown when _storyNode == null:
                {
                    _storyNode = GetNodeAtPoint(Event.current.mousePosition + _scrollPositionTextArea);
                    if (_storyNode == null)
                    {
                        _dragCanvas = true;
                        _dragCanvasOffset = Event.current.mousePosition + _scrollPosition;
                        Selection.activeObject = _selectedChapter;
                    }
                    break;
                }
                // Mouse Drag is true
                case EventType.MouseDrag when _storyNode != null:
                    _scrollPositionTextArea.y += e.delta.y;
                    GUI.changed = true;
                    break;
                // Mouse Drag and draggingCanvas is true
                case EventType.MouseDrag when _dragCanvas:
                    _scrollPosition = _dragCanvasOffset - Event.current.mousePosition;
                    GUI.changed = true;
                    break;
                // Mouse Up is true 
                case EventType.MouseUp:
                    _storyNode = null;
                    _dragCanvas = false;
                    break;
                // case EventType.MouseMove when _storyNode == null:
                //     _storyNode = GetNodeAtPoint(Event.current.mousePosition + _scrollPositionTextArea);
                //     if (_storyNode != null)
                //         Selection.activeObject = _storyNode;
                //     break;
                // case EventType.ScrollWheel:
                //     if (_storyNode != null)
                //     {
                //         //Debug.Log(_storyNode.name);
                //         _scrollPositionTextArea.y += e.delta.y * 2;
                //         e.Use();
                //         _storyNode = null;
                //         GUI.changed = true;
                //     }
                //     break;
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
                if (node.GetTextRect().Contains(point))
                {
                    selectedNode = node;
                }

            return selectedNode;
        }
        #endregion
    }
}