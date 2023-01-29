using System.Collections;
using System.Collections.Generic;
using Code.Dialogue.Story;
using UnityEngine;

namespace Code.DataPersistence.Data
{
    [System.Serializable]
   public class GameData
   {
       public Story CurrentChapter { get; set; }
       public string CurrentNode { get; set; }

   }
}
