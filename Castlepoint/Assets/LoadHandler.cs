using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
This script prevents player duplicates from spawning when transitioning from scene to scene.
*/
public class LoadHandler : MonoBehaviour
{
    [SerializeField]
    private GameObject[] players;
    private void Awake()
    {
        players = GameObject.FindGameObjectsWithTag("Player");
        if (players.Length > 1)
        {
            for (int i = 1; i < players.Length; i++)
            {
                Destroy(players[i]);
            }
        }


    }
}
