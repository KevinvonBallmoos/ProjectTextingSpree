namespace Code.Model.GameData
{
    /// <summary>
    /// Class provides player information
    /// </summary>
    /// <para name="author">Kevin von Ballmoos</para>
    /// <para name="date">09.12.2023</para>
    public static class PlayerInfoModel
    {
        #region Player and PlayerBackground
        
        /// <summary>
        /// Property for the Player name
        /// </summary>
        public static string PlayerName
        {
            get;
            set;
        }

        /// <summary>
        /// Property for the Player Background/Character
        /// </summary>
        public static string PlayerBackground
        {
            get;
            set;
        }

        #endregion
    }
}