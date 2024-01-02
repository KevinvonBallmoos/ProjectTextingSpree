using System;
using System.IO;
using Code.Model.Settings;
using UnityEngine;

namespace Code.Controller.FileController
{
    /// <summary>
    /// 
    /// </summary>
    /// <para name="author">Kevin von Ballmoos</para>
    /// <para name="date">23.12.2023</para>
    public static class JsonController
    {
        // Settings path
        private static readonly string SettingsPath = Application.persistentDataPath + "/Settings/Settings.json";

        #region Settings
        
        /// <summary>
        /// Saves the settings into a json file
        /// </summary>
        /// <param name="serializableSettings">setting properties</param>
        public static void SaveSettings(SettingsSerializeModel serializableSettings)
        {
            var json = JsonUtility.ToJson(serializableSettings);
            var path = Application.persistentDataPath + "/Settings/Settings.json";
            File.WriteAllText(path, json);
        }
        
        /// <summary>
        /// Loads the settings from the json file
        /// </summary>
        /// <returns>settings serialize model</returns>
        public static SettingsSerializeModel LoadSettings()
        {
            try
            {
                return JsonUtility.FromJson<SettingsSerializeModel>(SettingsPath);
            }
            catch (Exception)
            {
                return null;
            }
        }
        
        #endregion
    }
}