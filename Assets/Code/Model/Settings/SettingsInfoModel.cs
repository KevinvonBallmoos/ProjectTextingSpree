using System;
using System.Collections.Generic;

namespace Code.Model.Settings
{
    /// <summary>
    /// This class provides the properties for all settings
    /// </summary>
    /// <para name="author">Kevin von Ballmoos</para>
    /// <para name="date">23.12.2023</para>
    public abstract class SettingsInfoModel
    {
        /// <summary>
        /// Generic class for Settings
        /// </summary>
        /// <typeparam name="T">generic value: bool, int</typeparam>
        public class Setting<T>
        {
            public string SettingName;
            public string InfoLabelText;
            public T SettingValue;
        }
        
        /// <summary>
        /// Class to save all the setting properties
        /// </summary>
        /// <typeparam name="T"></typeparam>
        [Serializable]
        public class SettingList<T>
        {
            public List<Setting<T>> Settings;
        }
        
        #region Video Settings
        
        /// <summary>
        /// Contains all video properties
        /// int, string
        /// </summary>
        public static SettingList<int> videoList;
        public static SettingList<string> videoList2;
        
        public static Setting<int> FpsValue { get; set; }
        
        public static Setting<string> WindowSize { get; set; }
        
        #endregion
        
        #region Audio Settings

        /// <summary>
        /// Contains all audio properties
        /// </summary>
        public static SettingList<int> audioList;
        
        /// <summary>
        /// Volume of all audio sounds
        /// </summary>
        public static Setting<int> AllVolume { get; set; }
        
        /// <summary>
        /// Volume of the vfx sounds
        /// </summary>
        public static Setting<int> VfxVolume { get; set; }
        
        /// <summary>
        /// Volume of the music
        /// </summary>
        public static Setting<int> MusicVolume { get; set; }
        
        #endregion
        
        #region Game Settings
        
        /// <summary>
        /// Contains all in game properties
        /// </summary>
        public static SettingList<bool> inGameList;
        
        /// <summary>
        /// true  : the Story Text will appear letter by letter
        /// false : the Story Text will appear directly
        /// </summary>
        public static Setting<bool> IsTextSlowed { get; set; }
        
        /// <summary>
        /// Turn on or off the light flicker
        /// </summary>
        public static Setting<bool> LightFlicker { get; set; }
        
        #endregion
    }
}