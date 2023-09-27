using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Health : MonoBehaviour
{
    //public int thePlayer.health; // the current number of full hearts the player has
    // thePlayer.health already in character script
    public int numOfHearts; //sets the max number of hearts the player should have

    public GameObject background;
    public Text go;

    public Image[] hearts;
    public Sprite fullHeart;
    public Sprite emptyHeart;
    [SerializeField]
    public GameObject thePlayer;

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
        if (thePlayer.GetComponent<character>().health > numOfHearts)
        {
            thePlayer.GetComponent<character>().health = numOfHearts;
        }

        for (int i = 0; i < hearts.Length; i++)
        {
            if (i < thePlayer.GetComponent<character>().health)
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

        if (thePlayer.GetComponent<character>().health != 0)
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
        Animator animator = thePlayer.GetComponent<Animator>();
        Debug.Log("Playing Death Animation");
        animator.SetTrigger("isDead");

        // Wait for the duration of the death animation
        //yield return new WaitForSeconds(animator.GetCurrentAnimatorClipInfo(0)[0].clip.length);
        yield return new WaitForSeconds(0.9f);

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
            
           // TakeDamage(collision.gameObject.GetComponent<character>().getDmg());
            //this.Knock(collision.transform, collision.gameObject.GetComponent<character>().getThrust(), collision.gameObject.GetComponent<character>().getknockTime());
            Animator animator = GetComponent<Animator>();
            animator.SetTrigger("isHurt");
            


        }

    }

    private void OnTriggerEnter2D(Collider2D other) // moved the heartup to the oncollisionenter2d above
    {
        Debug.Log("A trigger happened");
        if (other.tag == "heartUp" && thePlayer.GetComponent<character>().health != thePlayer.GetComponent<character>().maxHealth)
        {
            Debug.Log("Picked up heart");
            other.gameObject.SetActive(false);
            thePlayer.GetComponent<character>().health += 1;
            heartSound.Play();
        }


    }
}
