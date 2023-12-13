using System.Collections.Generic;

namespace Code.Controller.LocalizationController
{
    /// This class is used to store the language data from the localized strings json file
    /// <para name="author">Kevin von Ballmoos</para>
    /// <para name="date">11.12.2023</para>
    [System.Serializable]
    public class LanguageData
    {
        // Language
        public string lang;
        // Value
        public string value;
    }

    /// This class is used to store the table data from the localized strings json file
    /// <para name="author">Kevin von Ballmoos</para>
    /// <para name="date">11.12.2023</para>
    [System.Serializable]
    public class LocalizationTableEntry
    {
        // Key of the value
        public string key;
        // LanguageData values
        public List<LanguageData> values;
    }

    /// This class is used to store the language data and table data from the localized strings json file
    /// <para name="author">Kevin von Ballmoos</para>
    /// <para name="date">11.12.2023</para>
    [System.Serializable]
    public class LocalizationData
    {
        // All languages
        public List<string> languages;
        // All keys and their values
        public List<LocalizationTableEntry> table;
    }
}