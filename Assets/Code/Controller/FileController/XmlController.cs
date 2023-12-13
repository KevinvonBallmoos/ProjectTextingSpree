using System.IO;
using System.Xml;
using Code.Controller.DialogueController.StoryDialogueController;
using UnityEngine;

namespace Code.Controller.FileController
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
        /// Reads the title node from the current StoryAssets/StoryChapter.xml 
        /// </summary>
        /// <param name="currentChapter">Current Chapter that is loaded</param>
        /// <returns>Chapter name</returns>
        public static string GetChapterTitle(StoryAssetController currentChapter)
        {
            var xmlDoc = new XmlDocument();
            var filePath = Path.Combine(Application.streamingAssetsPath, $"StoryFiles/{currentChapter.name}.xml");
            var xmlFile = File.ReadAllText(filePath);
            xmlDoc.LoadXml(xmlFile);
            var rootNode = xmlDoc.SelectSingleNode($"//{currentChapter.name}");
            return rootNode?.FirstChild.InnerText;
        }

        /// <summary>
        /// Reads the Node information of the chapter
        /// </summary>
        /// <param name="chapter">The current chapter that need's to be created</param>
        /// <returns>The XmlDoc, with the loaded File</returns>
        public static XmlDocument GetXmlDocOfStoryFile(string chapter)
        {
            var xmlDoc = new XmlDocument();
            var filePath = Path.Combine(Application.streamingAssetsPath, $"StoryFiles/{chapter}.xml");
            var xmlFile = File.ReadAllText(filePath);
            xmlDoc.LoadXml(xmlFile);
            return xmlDoc;
        }
    }
}