using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDataPersistence//for loading or saving data
{
    void LoadData(GameData data);
    void SaveData(GameData data);
}