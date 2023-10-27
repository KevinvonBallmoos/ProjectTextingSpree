using System.Collections.Generic;
using Newtonsoft.Json;

namespace Code.Model.NodeModels
{
    /// <summary>
    /// Object Class to cache the Story nodes
    /// </summary>
    /// <para name="author">Kevin von Ballmoos</para>
    /// <para name="date">15.08.2023</para>
    public class StoryNodeDeserializeModel
    {
        [JsonConstructor]
        protected StoryNodeDeserializeModel(
            string nodeId, 
            string labelText, 
            string text, 
            bool isChoiceNode, 
            bool isRootNode, 
            bool isEndOfStory, 
            bool isEndOfChapter, 
            bool isGameOver, 
            string image, 
            string item, 
            string background, 
            List<string> childNodes)
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
        public string LabelText { get; }
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
}