using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
/*
This script prevents player duplicates from spawning when transitioning from scene to scene.
*/
public class LoadHandler : MonoBehaviour
{
    [SerializeField]
    private GameObject[] players;
    private GameObject[] heartUI;
    public static LoadHandler instance{get;private set;}
    private void OnEnable() {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {     
        players = GameObject.FindGameObjectsWithTag("Player");
        heartUI = GameObject.FindGameObjectsWithTag("HeartUI");
        if(heartUI.Length != 0)
        {DontDestroyOnLoad(heartUI[0].gameObject);}
        if (players.Length > 1)
        {
            for (int i = 1; i < players.Length; i++)
            {
               Destroy(players[i]);
            }
        }
        
        if(heartUI.Length > 1)
        {
            for (int i = 1; i < heartUI.Length; i++)
            {
                Destroy(heartUI[i]);
            }
        }
    }
    private void Awake()
    {
        if (instance != null)//checks to see if another LoadHandler object
        {
            Debug.Log("Found more than one LoadHandler in the scene. Destroying the newest one.");
            Destroy(this.gameObject);
            return;
        }
        instance = this;//creates the LoadHandler object
        DontDestroyOnLoad(this.gameObject);//protects the LoadHandler object between scenes
    }
}
