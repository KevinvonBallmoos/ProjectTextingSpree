using Code.Logger;
using UnityEditor;

namespace Code.Dialogue 
{
    public class DialogueEditor : EditorWindow
    {
        private readonly GameLogger _logger = new GameLogger("DialogueEditor");
        
        private void OnEnable(){
            _logger.LogEntry("Test", "Logging Info", _logger.GetLineNumber());
        }
    }
}