using System.Collections.Generic;
using Newtonsoft.Json;

namespace Code.Model.Node
{
    /// <summary>
    /// Object Class to save the Story nodes
    /// </summary>
    /// <para name="author">Kevin von Ballmoos</para>
    /// <para name="date">15.08.2023</para>
    [System.Serializable]
    public class StoryNodeSerializeModel
    {
        [JsonProperty] private readonly string _nodeId;
        [JsonProperty] private readonly string _labelText;
        [JsonProperty] private readonly string _text;
        [JsonProperty] private readonly bool _isChoiceNode;
        [JsonProperty] private readonly bool _isRootNode;
        [JsonProperty] private readonly bool _isEndOfStory;
        [JsonProperty] private readonly bool _isEndOfChapter;
        [JsonProperty] private readonly bool _isGameOver;
        [JsonProperty] private readonly string _image;
        [JsonProperty] private readonly string _item;
        [JsonProperty] private readonly string _background;
        [JsonProperty] private readonly List<string> _childNodes;
        
        /// <summary>
        /// Constructor to save an Object of type StoryNode
        /// </summary>
        /// <param name="node">contains all properties</param>
        public StoryNodeSerializeModel(StoryNodeModel node)
        {
            _background = node.Background;
            _childNodes = node.ChildNodes;
            _image = node.Image;
            _isChoiceNode = node.IsChoiceNode;
            _isEndOfChapter = node.IsEndOfChapter;
            _isEndOfStory = node.IsEndOfPart;
            _isGameOver = node.IsGameOver;
            _isRootNode = node.IsRootNode;
            _item = node.Item;
            _labelText = node.LabelText;
            _nodeId = node.NodeId;
            _text = node.Text;
        }
    }
}