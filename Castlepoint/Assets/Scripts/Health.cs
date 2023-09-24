using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Health : character
{
    //public int health; // the current number of full hearts the player has
    // health already in character script
    public int numOfHearts; //sets the max number of hearts the player should have

    public GameObject background;
    public Text go;

    public Image[] hearts;
    public Sprite fullHeart;
    public Sprite emptyHeart;

    // For sound FX's
    public AudioClip collisionSound;
    private AudioSource heartSound;

    // Hit animation
    private Animator animator;

    private void Start()
    {
        heartSound = GetComponent<AudioSource>();
        heartSound.clip = collisionSound;
		animator = GetComponent<Animator>();
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
            StartCoroutine(PlayDeathAnimationAndLoadGameOver());
        }
    }

    IEnumerator PlayDeathAnimationAndLoadGameOver()
    {
        // Play the death animation
        Animator animator = GetComponent<Animator>();
        Debug.Log("Playing Death Animation");
        animator.SetTrigger("isDead");

        // Wait for the duration of the death animation
        //yield return new WaitForSeconds(animator.GetCurrentAnimatorClipInfo(0)[0].clip.length);
        yield return new WaitForSeconds(9.0f);

        // Load the game over scene
        SceneManager.LoadScene("game_over");
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
            Debug.Log("Took dmg");
            TakeDamage(collision.gameObject.GetComponent<character>().getDmg());
            this.Knock(collision.transform, collision.gameObject.GetComponent<character>().getThrust(), collision.gameObject.GetComponent<character>().getknockTime());
            Animator animator = GetComponent<Animator>();
            animator.SetTrigger("isHurt");
            


        }

    }

    private void OnTriggerEnter2D(Collider2D other) // moved the heartup to the oncollisionenter2d above
    {
        Debug.Log("A trigger happened");
        if (other.tag == "heartUp" && health != maxHealth)
        {
            Debug.Log("Picked up heart");
            other.gameObject.SetActive(false);
            health += 1;
            heartSound.Play();
        }


    }
}
