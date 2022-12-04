using Code.Logger;
using UnityEditor;

namespace Code.Dialogue 
{
    /// <summary>
    /// Story Editor, Creates Nodes used for the Story
    /// </summary>
    /// <para name="author">Kevin von Ballmoos></para>
    /// <para name="date">04.12.2022</para>
    public class DialogueEditor : EditorWindow
    {
        private readonly GameLogger _logger = new GameLogger("DialogueEditor");
     
        
        /// <summary>
        /// Extends Unity with the Story Editor
        /// </summary>
        [MenuItem("Window/Story Editor")]
        public static void ShowEditorWindow()
        {
            GetWindow(typeof(StoryEditor), false, "Story Editor");
        }
        
        private void OnEnable(){
            _logger.LogEntry("Test", "Logging Info", _logger.GetLineNumber());
        }
    }
}