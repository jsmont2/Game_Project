/*
  items list:
  Element 0 = Key
  Element 1 = Magic Bottle
  Element 2 = Heart
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour
{
    [SerializeField] private List<GameObject> items;
    private Animator anim;
    private bool hasKey;
    private bool playerInRange;
    public GameObject itemInChest;
    private void Awake()
    {
        anim = GetComponent<Animator>();
        anim.SetBool("Locked", true);

    }
    public void OpenChest()
    {
        anim.SetBool("Locked", false);
        GameObject temp = GameObject.FindWithTag("Player");
        if (hasKey)
        {
            itemInChest.GetComponent<ChestItem>().ChooseItem(0);
            temp.GetComponent<PlayerController>().hasKey = true;
        }
        else
        {
            int rand = UnityEngine.Random.Range(1, 2);
            itemInChest.GetComponent<ChestItem>().ChooseItem(rand);
            if(rand == 1)temp.GetComponent<PlayerController>().playerInventory.currentMagic += 1;
            else 
            {
                if(temp.GetComponent<character>().health < temp.GetComponent<character>().maxHealth)
                temp.GetComponent<character>().health += 1;
            }
        }
        
    }
    public void SetHasKey()
    {
        hasKey = true;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && playerInRange && anim.GetBool("Locked"))
        {
            OpenChest();
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
        }
    }
}
