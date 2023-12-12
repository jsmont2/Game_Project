using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class game_over : MonoBehaviour
{
    public GameObject DataManager;
    public GameObject sceneHandle;
    private void Start() 
    {
        DataManager = GameObject.Find("DataPersistenceManager");
        sceneHandle = GameObject.Find("Scene Data");
    }
    public void MainMenu()
    {
        SceneManager.LoadScene("MainMenu");
        Time.timeScale = 1f;
    }

    public void PlayGame()
    {
        DataManager.GetComponent<DataPersistenceManager>().NewGame();
        SceneManager.LoadSceneAsync(1);
    }

    public void LoadGame()
    {        
        //SceneManager.LoadScene(PlayerPrefs.GetInt("SavedScene"));
        DataManager.GetComponent<DataPersistenceManager>().LoadGame();
        SceneManager.LoadSceneAsync(sceneHandle.GetComponent<SceneScript>().currentScene);
    }
}