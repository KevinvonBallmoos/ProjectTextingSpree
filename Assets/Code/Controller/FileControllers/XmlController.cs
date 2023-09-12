using System.IO;
using System.Xml;
using UnityEngine;

using Code.Dialogue.Story;

namespace Code.Controller.FileControllers
{
    /// <summary>
    /// This class contains Methods to access Xml Files
    /// Returns XmlDocument, XmlReader, XmlWriter, XmlNode, Node InnerText or Content
    /// </summary>
    /// <para name="author">Kevin von Ballmoos</para>
    /// <para name="date">07.05.2023</para>
    public static class XmlController
    {
        /// <summary>
        /// Reads the MessageBox Nodes from the OutsourcedStrings/strings.xml
        /// </summary>
        /// <param name="index">Determines the node to return</param>
        /// <returns>MessageBox nodes</returns>
        public static string GetMessageBoxText(int index)
        {
            var xmlFile = Resources.Load<TextAsset>("OutsourcedStrings/strings");
            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xmlFile.text);
            var messageNodes = xmlDoc.GetElementsByTagName("MessageBoxText");
            return messageNodes[index].InnerText;
        }
        
        /// <summary>
        /// Reads the MessageBox Nodes from the OutsourcedStrings/strings.xml
        /// </summary>
        /// <param name="index">Determines the node to return</param>
        /// <returns>Information nodes</returns>
        public static string GetInformationText(int index)
        {
            var xmlFile = Resources.Load<TextAsset>("OutsourcedStrings/strings");
            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xmlFile.text);
            var messageNodes = xmlDoc.GetElementsByTagName("Information");
            return messageNodes[index].InnerText;
        }
        
        /// <summary>
        /// Reads the title node from the current StoryAssets/StoryChapter.xml 
        /// </summary>
        /// <param name="currentChapter">current Chapter that is loaded</param>
        /// <returns>Chapter name</returns>
        public static string GetChapterTitle(StoryAsset currentChapter)
        {
            var xmlDoc = new XmlDocument();
            var filePath = Path.Combine(Application.streamingAssetsPath, $"StoryFiles/{currentChapter.name}.xml");
            var xmlFile = File.ReadAllText(filePath);
            xmlDoc.LoadXml(xmlFile);
            var rootNode = xmlDoc.SelectSingleNode($"//{currentChapter.name}");
            return rootNode?.FirstChild.InnerText;
        }
    }
}