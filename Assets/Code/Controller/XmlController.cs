using System.Xml;
using UnityEngine;

namespace Code
{
    /// <summary>
    /// This class contains Methods to access Xml File
    /// Returns XmlDocument, XmlReader, XmlWriter, XmlNode, Node InnerText or Content
    /// </summary>
    /// <para name="author">Kevin von Ballmoos</para>
    /// <para name="date">07.05.2023</para>
    public static class XmlController
    {
        /// <summary>
        /// Returns the root node of the strings xml
        /// </summary>
        /// <param name="index"></param>
        public static string GetMessageBoxText(int index)
        {
            var xmlFile = Resources.Load<TextAsset>("OutsourcedStrings/strings");
            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xmlFile.text);
            var messageNodes = xmlDoc.GetElementsByTagName("MessageBoxText");
            return messageNodes[index].InnerText;
        }
        
        /// <summary>
        /// Returns the root node of the strings xml
        /// </summary>
        /// <param name="index"></param>
        public static string GetInformationText(int index)
        {
            var xmlFile = Resources.Load<TextAsset>("OutsourcedStrings/strings");
            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xmlFile.text);
            var messageNodes = xmlDoc.GetElementsByTagName("Information");
            return messageNodes[index].InnerText;
        }
    }
}