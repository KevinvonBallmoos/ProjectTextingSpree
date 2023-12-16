using System;
using System.Collections.Generic;
using System.Linq;
using Code.Controller.LocalizationController;
using UnityEditor;
using UnityEngine;

namespace Code
{
    /// <summary>
    /// Class provides localization for strings
    /// </summary>
    /// <para name="author">Kevin von Ballmoos</para>
    /// <para name="date">11.12.2023</para>
    public static class LocalizationManager
    {
        // Current language
        private static string _currentLanguage;
        // Dictionary for strings
        private static readonly Dictionary<string, Dictionary<string, string>> Languages = new ();
        
        /// <summary>
        /// Loads all localizable values from the LocalizedStrings.json files
        /// </summary>
        public static void LoadLocalizableValues()
        {
            var jsonFiles = Resources.LoadAll<TextAsset>("LocalizedStrings");
            foreach (var file in jsonFiles)
            {
                var localizationData = JsonUtility.FromJson<LocalizationData>(file.text);
                _currentLanguage = localizationData.languages[0];
                try
                {
                    foreach (var language in localizationData.languages)
                    {
                        foreach (var entry in localizationData.table)
                        {
                            foreach (var data in entry.values.Where(data => data.lang == language))
                            {
                                // Check if the language key already exists
                                if (Languages.TryGetValue(language, out var lan))
                                {
                                    // If it does, add the new string to the existing entry
                                    lan.Add(entry.key, data.value);
                                }
                                else
                                {
                                    // If it doesn't, create a new entry for that language
                                    var newLanguageStrings = new Dictionary<string, string>
                                    {
                                        { entry.key, data.value }
                                    };
                                    Languages.Add(language, newLanguageStrings);
                                }
                            }
                        }
                    }

                }
                catch (Exception ex)
                {
                    //Debug.Log(ex.Message);
                }
            }
        }
        
        /// <summary>
        /// Gets the value from the key
        /// </summary>
        /// <param name="key">Key that value is needed</param>
        /// <returns>value of the key</returns>
        public static string GetLocalizedValue(string key)
        {
            if (Languages.TryGetValue(_currentLanguage, out var language))
            {
                if (language.TryGetValue(key, out var value))
                {
                    return value;
                }
            }
            // If the key is not found, return a placeholder or the key itself
            return "Localization key not found: " + key;
        }
    }
}