using System.Collections.Generic;

namespace Code.Controller.LocalizationController
{
    /// <summary>
    /// Class provides all keys to localize values
    /// </summary>
    /// <para name="author">Kevin von Ballmoos</para>
    /// <para name="date">11.12.2023</para>
    public static class LocalizationKeyController
    {
        #region Error Label Keys
        
        // Save file error label override save
        public const string SaveFileErrorLabelOverrideCaptionKey = "savefile_errorlabel_override_caption";
        // Save file error label load save
        public const string SaveFileErrorLabelLoadCaptionKey = "savefile_errorlabel_load_caption";
        // Save file error label load save
        public const string SaveFileErrorLabelRemoveCaptionKey = "savefile_errorlabel_remove_caption";
        
        #endregion
        
        #region Message Box Keys
        
        // Save file error label override save
        public const string MessageBoxText1CaptionKey = "messagebox_text_1_caption";
        // Save file error label load save
        public const string MessageBoxText2CaptionKey = "messagebox_text_2_caption";
        
        #endregion
        
        #region GameDataPaper Keys
        
        // Game data paper information text 1
        private const string InformationText1CaptionKey = "information_text_1_caption";
        // Game data paper information text 2
        private const string InformationText2CaptionKey = "information_text_2_caption";

        public static readonly List<string> InformationKeys = new () { InformationText1CaptionKey, InformationText2CaptionKey };

        #endregion
        
        #region Menu Label Keys
        
        // Menu information label new game hover text
        public const string MenuInformationLabelNewGameKey = "menu_informationlabel_newgame_caption";
        // Menu information label load game hover text
        public const string MenuInformationLabelLoadGameKey = "menu_informationlabel_loadgame_caption";
        // Menu information label settings hover text
        public const string MenuInformationLabelSettingsKey = "menu_informationlabel_settings_caption";
        // Menu information label quit hover text
        public const string MenuInformationLabelQuitKey = "menu_informationlabel_quit_caption";
        
        #endregion
    }
}