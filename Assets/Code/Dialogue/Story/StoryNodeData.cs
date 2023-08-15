using System.Collections.Generic;
using UnityEngine;

namespace Code.Dialogue.Story
{
    public class StoryNodeData
    {
        public readonly string NodeId;
        public readonly string LabelText;
        public readonly string Text;
        public readonly bool IsChoiceNode;
        public readonly bool IsRootNode;
        public readonly bool IsEndOfStory;
        public readonly bool IsEndOfChapter;
        public readonly bool IsGameOver;
        public readonly string Image;
        public readonly string Item;
        public readonly string Background;
        public readonly List<string> ChildNodes;
        
        public StoryNodeData(StoryNode node)
        {
            NodeId = node.nodeId;
            LabelText = node.labelText;
            Text = node.text;
            IsChoiceNode = node.isChoiceNode;
            IsRootNode = node.isRootNode;
            IsEndOfStory = node.isEndOfStory;
            IsEndOfChapter = node.isEndOfChapter;
            IsGameOver = node.isGameOver;
            Image = node.image;
            Item = node.item;
            Background = node.background;
            ChildNodes = node.childNodes;
        }
    }
}