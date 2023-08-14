#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Callbacks;
#endif
using System;
using Code.Dialogue.Combat;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Editor.Combat
{
    /// <summary>
    /// Displays the selected Combat 
    /// If there are 0 nodes then it reads them from the xml file
    /// </summary>
    /// <para name="author">Kevin von Ballmoos</para>
    /// <para name="date">14.07.2023</para>
    public class CombatViewer : EditorWindow
    {
        // Combat Asset
        private CombatAsset _selectedCombat;
        // Scroll Area / Position
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
        [NonSerialized] private GUIStyle _combatNodeStyle;
        [NonSerialized] private GUIStyle _choiceNodeStyle;
        [NonSerialized] private GUIStyle _textAreaStyle;
        // Drag
        [NonSerialized] private CombatNode _combatNode;
        [NonSerialized] private bool _dragCanvas;
        [NonSerialized] private Vector2 _dragOffset;
        [NonSerialized] private Vector2 _dragCanvasOffset;
        
        // Object
        private Object[] _xmlFiles;
        private Object _previousSelection;
        // Text
        private string _textContent;

        #region CombatWindow

        /// <summary>
        /// Shows the Editor Window, depending if a Combat Asset is loaded or not
        /// Asset Callback : In computer programming, a callback is executable code that is passed as an argument to other code.
        /// </summary>
        /// <param name="instanceId"></param>
        /// <returns>true when a Combat is loaded and false when not</returns>
        [OnOpenAsset(1)]
        public static bool OnOpenAsset(int instanceId)
        {
            var combat = EditorUtility.InstanceIDToObject(instanceId) as CombatAsset;
            if (combat == null) return false;
            ShowEditorWindow();
            return true;
        }

        /// <summary>
        /// Extends Unity with the Combat Viewer
        /// </summary>
        [MenuItem("Window/Combat Viewer")]
        public static void ShowEditorWindow()
        {
            GetWindow(typeof(CombatViewer), false, "Combat Viewer");
        }

        /// <summary>
        /// When the Dialog is Enabled, initialize the node Styles.
        /// </summary>
        private void OnEnable()
        {
            Selection.selectionChanged += OnSelectionChanged;

            _combatNodeStyle = new GUIStyle
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
            var newChapter = Selection.activeObject as CombatAsset;
            if (newChapter == null || newChapter.name == "") return;
            
            _selectedCombat = null;
            _combatNode = null;
            _xmlFiles = Resources.LoadAll($@"CombatFiles/", typeof(TextAsset));
            foreach (var file in _xmlFiles)
            {
                if (file.name != newChapter.name) continue;
                _selectedCombat = newChapter;
                break;
            }
            if (_selectedCombat == null) return;
            
            _selectedCombat.ReadNodes(_selectedCombat);

            Repaint();
            
            Selection.activeObject = null;
        }

				#endregion

		#region OnGUI

		/// <summary>
		/// Gets called every time a change happens on the Editor. Displays the nodes.
		/// </summary>
		private void OnGUI()
        {
            if (!_selectedCombat.HasReadNodes) return;
            
            // Mouse Events
            ProcessEvents();
            
            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);
            DrawSurface();
            
            //if (_selectedChapter.GetAllNodes() == null) return;
            foreach (var node in _selectedCombat.GetAllNodes())
                DrawNode(node);
            
            foreach (var node in _selectedCombat.GetAllNodes())
            {
                if (!node.IsRootNode()) continue;
                DrawConnections(node);
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
        /// Draws the node
        /// </summary>
        /// <param name="node">Next node to draw</param>
        private void DrawNode(CombatNode node)
        {
            var style = _combatNodeStyle;
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
        private void DrawConnections(CombatNode node)
        {
            var children = node.GetChildNodes();
            if (children.Count == 0) return;

             for (var i = 0; i < children.Count; i++)
             {
                 var child = _selectedCombat.GetAllChildNodes(node)[i];
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
                case EventType.MouseDown when _combatNode == null:
                {
                    _combatNode = GetNodeAtPoint(Event.current.mousePosition + _scrollPositionTextArea);
                    if (_combatNode == null)
                    {
                        _dragCanvas = true;
                        _dragCanvasOffset = Event.current.mousePosition + _scrollPosition;
                        Selection.activeObject = _selectedCombat;
                    }
                    else
                    {
                        Selection.activeObject = _combatNode;
                    }
                    break;
                }
                // Mouse Drag is true
                case EventType.MouseDrag when _combatNode != null:
                    break;
                // Mouse Drag and draggingCanvas is true
                case EventType.MouseDrag when _dragCanvas:
                    _scrollPosition = _dragCanvasOffset - Event.current.mousePosition;
                    GUI.changed = true;
                    break;
                // Mouse Up is true 
                case EventType.MouseUp when _combatNode != null:
                    _combatNode = null;
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
        private CombatNode GetNodeAtPoint(Vector2 point)
        {
            CombatNode selectedNode = null;
            foreach (var node in _selectedCombat.GetAllNodes())
                if (node.GetTextRect().Contains(point))
                {
                    selectedNode = node;
                }

            return selectedNode;
        }
        #endregion
    }
}