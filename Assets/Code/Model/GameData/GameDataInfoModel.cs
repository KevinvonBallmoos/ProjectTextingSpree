using System.Linq;
using UnityEngine.UI;

namespace Code.Model.GameData
{
    /// <summary>
    /// Class provides additional game data information
    /// </summary>
    /// <para name="author">Kevin von Ballmoos</para>
    /// <para name="date">09.12.2023</para>
    public static class GameDataInfoModel
    {
        #region PlaceHolder

        /// <summary>
        /// Property for the Player name
        /// </summary>
        public static int Placeholder { get; set; }
        
        /// <summary>
        /// Checks on which placeholder the check Image is active and saves the number
        /// </summary>
        /// <param name="holders">controls that hold images</param>
        public static void SetPlaceholderNum(Image[] holders)
        {
            var holder = holders.Where(h => h.name.Equals("CheckImage")).ToList();
            var count = 0;
            foreach (var h in holder)
            {
                count++;
                if (h.enabled)
                {
                    Placeholder = count - 1;
                    break;
                }
                if (count == 3 && Placeholder != 2)
                    Placeholder = -1;
            }
        }

        #endregion
        
        #region Player and PlayerBackground
        
        /// <summary>
        /// Property for the Player name
        /// </summary>
        public static string PlayerName { get; set; }

        /// <summary>
        /// Property for the Player Background/Character
        /// </summary>
        public static string PlayerBackground { get; set; }

        #endregion

        #region Override

        /// <summary>
        /// Property for the is Override state
        /// </summary>
        public static bool IsOverride { get; set; }

        #endregion
    }
}