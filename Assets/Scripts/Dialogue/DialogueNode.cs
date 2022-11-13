using System;
using System.Collections.Generic;
using System.Drawing;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

namespace Dialogue
{
    // Inherit from ScriptableObject
    public class DialogueNode : ScriptableObject
    {
        [SerializeField] private bool isPlayerSpeaking = false;
        [SerializeField] private bool isRootNode = false;
        public string text;
        [SerializeField] private List<string> children = new List<string>();
        [SerializeField] private Rect rect = new Rect(10, 10, 400, 200);

#if UNITY_EDITOR
        // Getter and Setter for properties
        public void SetText(string txt)
        {
            // if (!string.Equals(txt, text))
            // {
            //     // Undo marks scriptable Object automatically as Dirty
            //     Undo.RecordObject(this, "Update Dialogue Text");
            //     text = txt;
            //     EditorUtility.SetDirty(this);
            // }
        }

        public void AddChildren(string childID)
        {
            Undo.RecordObject(this, "Remove Dialogue Link");
            children.Add(childID);
            EditorUtility.SetDirty(this);
        }
        
        public void RemoveChildren(string childID)
        {
            Undo.RecordObject(this, "Remove Dialogue Link");
            children.Remove(childID);
            EditorUtility.SetDirty(this);
        }
        
        public void SetPlayerSpeaking(bool newIsPlayerSpeaking)
        {
            Undo.RecordObject(this, "Change Dialogue Speaker");
            isPlayerSpeaking = newIsPlayerSpeaking;
            EditorUtility.SetDirty(this);
        }
#endif                                                  

        public void SetRect(Vector2 vector)
        {
            Undo.RecordObject(this, "Move Dialogue Node");
            rect.position = vector;
        }

        public string GetText()
        {
            return text;
        }

        public List<string> GetChildren()
        {
            return children;
        }

        public Rect GetRect()
        {
            return rect;
        }

        public Rect GetRect(Vector2 pos)
        {
            rect.position = rect.position + pos;
            return rect;
        }

        public bool IsPlayerSpeaking()
        {
            return isPlayerSpeaking;
        }
        
        public bool IsRootNode()
        {
            return isRootNode;
        }
        
        
    }
}
