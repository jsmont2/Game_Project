using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement;

public class SceneScript : MonoBehaviour, IDataPersistence
{
    public int currentScene;
    public int previousScene;
    public void LoadData(GameData data)
    {
        this.currentScene = data.currentScene;
    }
    public void SaveData(GameData data)
    {
        if (SceneManager.GetActiveScene().name != "MainMenu" || SceneManager.GetActiveScene().buildIndex != 6)
        {
            data.currentScene = SceneManager.GetActiveScene().buildIndex;
        }
    }
    private void Start()
    {
        currentScene = SceneManager.GetActiveScene().buildIndex;
    }

}
