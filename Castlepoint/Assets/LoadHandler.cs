using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
