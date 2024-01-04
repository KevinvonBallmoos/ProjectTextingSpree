using System;
using System.IO;
using System.Text;
using Code.Model.Settings;
using Newtonsoft.Json;
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
            var json = JsonConvert.SerializeObject(serializableSettings, Formatting.Indented);
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
                var json = File.ReadAllText(SettingsPath, Encoding.UTF8);
                return JsonConvert.DeserializeObject<SettingsSerializeModel>(json);
            }
            catch (Exception ex)
            {
                Debug.Log(ex.Message);
                return null;
            }
        }
        
        #endregion
    }
}