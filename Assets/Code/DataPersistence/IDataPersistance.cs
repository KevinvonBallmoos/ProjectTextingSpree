using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public interface IDataPersistance
{
    
    
    /// <summary>
    /// We load data through the implementing script here. Here we want our scripts 
    /// </summary>
    /// <param name="data"></param>
    public void LoadData(GameData data);

    /// <summary>
    /// Saves data. We pass the GameData here by reference because we want the implementing scripts
    /// to be able to manipulate the data that is send to this method in the first place.
    /// </summary>
    /// <param name="data"></param>
    public void SaveData(GameData data);
}
