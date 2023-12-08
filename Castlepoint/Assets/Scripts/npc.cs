using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class npc : MonoBehaviour
{
    public GameObject dialogBox;
    public Text dialogText;
    public string dialog;
    public bool playerInRange;
    public GameObject prompt;
    public float textSpeed;
    public AudioClip typingSound;
    private bool isTyping;
    private AudioSource audioSource;
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = typingSound;
    }

    // Update is called once per frame
    void Update()
    {
        if (playerInRange)
        {
            prompt.SetActive(true);
        }
        else
        {
            prompt.SetActive(false);
        }

        if (Input.GetKeyDown(KeyCode.E) && playerInRange)
        {
            if (dialogBox.activeInHierarchy)
            {
                dialogBox.SetActive(false);
                audioSource.Stop(); // Stop playing the typing sound if dialog is closed
            }
            else
            {
                dialogBox.SetActive(true);
                audioSource.Play(); // Start playing the typing sound
                //dialogText.text = dialog;
                StartCoroutine(TypeText(dialog));   
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;

        }

    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            dialogBox.SetActive(false);
        }
    }

   IEnumerator TypeText(string textToType)
    {
        isTyping = true;
        dialogText.text = "";

        foreach (char letter in textToType)
        {
            dialogText.text += letter;
      
            yield return new WaitForSeconds(textSpeed);
        }
        isTyping = false;
        audioSource.Stop();
    }

  

}
