using System.Collections.Generic;
using System.Linq;
using Code.Controller.LocalizationController;
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
        private static string currentLanguage;
        // Dictionary for strings
        private static readonly Dictionary<string, Dictionary<string, string>> languages = new ();
        
        /// <summary>
        /// Loads all localizable values from the LocalizedStrings.json files
        /// </summary>
        public static void LoadLocalizableValues()
        {
            var jsonFile = Resources.Load<TextAsset>("LocalizedStrings/LocalizedStrings");
            var localizationData = JsonUtility.FromJson<LocalizationData>(jsonFile.text);
            currentLanguage = localizationData.languages[0];

            foreach (var language in localizationData.languages)
            {
                var languageStrings = new Dictionary<string, string>();

                foreach (var entry in localizationData.table)
                {
                    foreach (var data in entry.values.Where(data => data.lang == language))
                    {
                        languageStrings.Add(entry.key, data.value);
                    }
                }
                languages.Add(language, languageStrings);
            }
        }
        
        /// <summary>
        /// Gets the value from the key
        /// </summary>
        /// <param name="key">Key that value is needed</param>
        /// <returns>value of the key</returns>
        public static string GetLocalizedValue(string key)
        {
            if (languages.TryGetValue(currentLanguage, out var language))
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