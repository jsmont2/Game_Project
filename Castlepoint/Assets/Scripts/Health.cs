using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Health : MonoBehaviour
{
    public int health; // the current number of full hearts the player has
    public int numOfHearts; //sets the max number of hearts the player should have

    public GameObject background;
    public Text go;

    public Image[] hearts;
    public Sprite fullHeart;
    public Sprite emptyHeart;

    // For sound FX's
    public AudioClip collisionSound;
    private AudioSource heartSound;

    private void Start()
    {
        heartSound = GetComponent<AudioSource>();
        heartSound.clip = collisionSound;
    }

    void Update()
    {
        if (health > numOfHearts)
        {
            health = numOfHearts;
        }

        for (int i = 0; i < hearts.Length; i++)
        {
            if (i < health)
            {
                hearts[i].sprite = fullHeart;
            }
            else
            {
                hearts[i].sprite = emptyHeart;
                
            }

            if (i < numOfHearts)
            {
                hearts[i].enabled = true;
            }
            else
            {
                hearts[i].enabled = false;
                
            }
        }

        if (health != 0)
        {
            background.SetActive(false);
        }
        else
        {
            background.SetActive(true);
            SceneManager.LoadScene("game_over"); // loads game over

        }
    }

    public void RestartButton()
    {
        SceneManager.LoadScene("overworld");
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.tag == "enemy")
        {
            // lose 1 heart if colliding with enemy
            health -= 1;
            
        }
        if (collision.collider.tag == "heartUp")
        {
            collision.gameObject.SetActive(false);
            health += 1;
            heartSound.Play();
        }
    }

    //private void OnTriggerEnter2D(Collider2D other) // Moved the heartUp to the OnCollisionEnter2D above
    //{
    //    if (other.tag == "heartUp")
    //    {
    //        other.gameObject.SetActive(false);
    //        health += 1;
    //        heartSound.Play();
    //    }
    //}
}
