using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class HealthJohn : MonoBehaviour
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
    private GameObject[] heartsUI;

    // For sound FX's
    public AudioClip heartUpSound;
    private AudioSource heartSound;

    // Hit animation
    private Animator animator;

    private void Start()
    {
        heartSound = GetComponent<AudioSource>();
        heartSound.clip = heartUpSound;
		animator = GetComponent<Animator>();
	}
    void Update()
    {
        if (thePlayer.GetComponent<character>().health > numOfHearts)
        {
            thePlayer.GetComponent<character>().health = numOfHearts;
        }
        heartsUI = GameObject.FindGameObjectsWithTag("HeartUI");
        for (int i = 0; i < heartsUI.Length; i++)
        {
            hearts[i] = heartsUI[i].GetComponent<Image>();
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
           // background.SetActive(false);
        }
    }

    

    public void RestartButton()
    {
        SceneManager.LoadScene("overworld");
    }   

    private void OnTriggerEnter2D(Collider2D other) // moved the heartup to the oncollisionenter2d above
    {
        if (other.tag == "heartUp" && thePlayer.GetComponent<character>().health != thePlayer.GetComponent<character>().maxHealth)
        {
            Debug.Log("Picked up heart");
            other.gameObject.SetActive(false);
            thePlayer.GetComponent<character>().health += 1;
            heartSound.Play();
        }


    }
}
