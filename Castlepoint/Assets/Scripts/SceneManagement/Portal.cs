using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour
{
    public string sceneToLoad;
    public Transform spawnPoint; // Set this in the Inspector to the desired spawn point in the "overworld" scene

    public void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player") && !other.isTrigger)
        {
            // Save the player's position before loading the new scene
            PlayerPrefs.SetFloat("PlayerX", other.transform.position.x);
            PlayerPrefs.SetFloat("PlayerY", other.transform.position.y);

            SceneManager.LoadScene(sceneToLoad);
        }
    }
    
}
