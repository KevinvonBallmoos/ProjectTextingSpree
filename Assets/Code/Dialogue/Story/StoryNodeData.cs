using System.Collections.Generic;

namespace Code.Dialogue.Story
{
    public class StoryNodeDataProperty
    {
        public StoryNodeDataProperty(string nodeId, string labelText, string text, bool isChoiceNode, bool isRootNode, bool isEndOfStory, bool isEndOfChapter, bool isGameOver, string image, string item, string background, List<string> childNodes)
        {
            NodeId = nodeId;
            LabelText = labelText;
            Text = text;
            IsChoiceNode = isChoiceNode;
            IsRootNode = isRootNode;
            IsEndOfStory = isEndOfStory;
            IsEndOfChapter = isEndOfChapter;
            IsGameOver = isGameOver;
            Image = image;
            Item = item;
            Background = background;
            ChildNodes = childNodes;
        }

        public string NodeId { get; }
        public string LabelText { get;}
        public string Text { get; }
        public bool IsChoiceNode { get; }
        public bool IsRootNode { get; }
        public bool IsEndOfStory { get; }
        public bool IsEndOfChapter { get; }
        public bool IsGameOver { get; }
        public string Image { get; }
        public string Item { get; }
        public string Background { get; }
        public List<string> ChildNodes { get; }
    }
    
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