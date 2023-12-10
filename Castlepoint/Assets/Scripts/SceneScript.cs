using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement;

public class SceneScript : MonoBehaviour , IDataPersistence
{
    public int currentScene;
    public void LoadData(GameData data)
    {        
        this.currentScene = data.currentScene;
    }
    public void SaveData(GameData data)
    {
        if (SceneManager.GetActiveScene().name != "MainMenu")
        {
            data.currentScene = SceneManager.GetActiveScene().buildIndex;            
        }
    }
    private void Awake() {
        currentScene = SceneManager.GetActiveScene().buildIndex;
    }   
      
}
