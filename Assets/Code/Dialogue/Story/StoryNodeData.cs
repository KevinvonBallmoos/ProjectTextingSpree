using System.Collections.Generic;

namespace Code.Dialogue.Story
{
    /// <summary>
    /// Object Class to save the Story nodes
    /// </summary>
    /// <para name="author">Kevin von Ballmoos</para>
    /// <para name="date">15.08.2023</para>
    public class StoryNodeData
    {
        private readonly string _nodeId;
        private readonly string _labelText;
        private readonly string _text;
        private readonly bool _isChoiceNode;
        private readonly bool _isRootNode;
        private readonly bool _isEndOfStory;
        private readonly bool _isEndOfChapter;
        private readonly bool _isGameOver;
        private readonly string _image;
        private readonly string _item;
        private readonly string _background;
        private readonly List<string> _childNodes;
        
        /// <summary>
        /// Constructor to save an Object of type StoryNode
        /// </summary>
        /// <param name="node">contains all properties</param>
        public StoryNodeData(StoryNode node)
        {
            _nodeId = node.NodeId;
            _labelText = node.LabelText;
            _text = node.Text;
            _isChoiceNode = node.IsChoiceNode;
            _isRootNode = node.IsRootNode;
            _isEndOfStory = node.IsEndOfPart;
            _isEndOfChapter = node.IsEndOfChapter;
            _isGameOver = node.IsGameOver;
            _image = node.Image;
            _item = node.Item;
            _background = node.Background;
            _childNodes = node.ChildNodes;
        }
    }
}