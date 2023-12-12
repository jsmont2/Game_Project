using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;
/*Okay, so this script will be the data that we actually want to store, whether it be the player's health, position, inventory, you name it
 in order to fully implement this class, one would need to also implement Data Persistence Interface into the scripts in which the desired data is being
 stored*/
[System.Serializable]
public class GameData
{
    public float currentHealth;
    public bool hasKey;
    public Vector3 currentPosition;
    public float currentMagic;
    public float currentXp;
    public float targetXp;
    public int level;
    public int currentScene;
    public int previousScene;
    public Vector2 maxPos;
    public Vector2 minPos;
    public Vector2 maxPosDung;
    public Vector2 minPosDung;
    public List<Vector3> boxPosition;
    public SerializableDictionary<int, bool> chestOpened;
    public int randSeed;
    public List<int>roomNumbers;
    public List<bool>roomsActive;
    public List<Vector3> boxPos;
    public List<List<bool>> enemyList;
    public GameData()//New Game values will be stored in this method
    {
        currentHealth = 3;
        hasKey = true;
        currentPosition = new Vector3(0, 0, 0);
        currentMagic = 0;
        currentXp = 0;
        targetXp = 0;
        level = 0;
        this.currentScene = 1;//This is where the name of the current scene will be kept
        previousScene = 0;
        maxPos = new Vector2(67f, 59f);
        minPos = new Vector2(.5f, .2f);
        maxPosDung = new Vector2(19f, 31.5f);
        minPosDung = new Vector2(-19f, -15.5f);
        boxPosition = new List<Vector3>();
        chestOpened = new SerializableDictionary<int, bool>();
        randSeed = 0;
        roomNumbers = new List<int>();
        roomsActive = new List<bool>();
        boxPos = new List<Vector3>();
        enemyList = new List<List<bool>>();
    }
}

