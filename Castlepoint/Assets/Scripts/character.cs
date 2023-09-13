using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class character : MonoBehaviour
{
    // Character script for both the player and enemies
    public enum characterState  // the character state machine for all "players" in the game
    {
        idle,
        walk,
        attack, 
        stagger,
        interact
    }

    public characterState currentState;
    public float speed;
    public Rigidbody2D rb;



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
