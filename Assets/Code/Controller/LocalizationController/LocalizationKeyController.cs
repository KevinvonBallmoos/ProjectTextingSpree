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
        #region Safe File Keys
        
        // Save file error label override save
        public const string SaveFileErrorLabelOverrideCaptionKey = "savefile_errorlabel_override_caption";
        // Save file error label load save
        public const string SaveFileErrorLabelLoadCaptionKey = "savefile_errorlabel_load_caption";
        // Save file error label load save
        public const string SaveFileErrorLabelRemoveCaptionKey = "savefile_errorlabel_remove_caption";
        
        #endregion
        
        #region Message box keys
        
        // Save file error label override save
        public const string MessageBoxText1CaptionKey = "messagebox_text_1_caption";
        // Save file error label load save
        public const string MessageBoxText2CaptionKey = "messagebox_text_2_caption";
        // Save file error label load save
        public const string InformationText1CaptionKey = "information_text_1_caption";
        // Save file error label load save
        public const string InformationText2CaptionKey = "information_text_2_caption";

        public static readonly List<string> InformationKeys = new () { InformationText1CaptionKey, InformationText2CaptionKey };

        #endregion
    }
}