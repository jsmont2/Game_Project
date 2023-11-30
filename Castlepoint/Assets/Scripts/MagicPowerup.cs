using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicPowerup : Powerup
{

    public Inventory playerInventory;
    public float magicValue;
    public AudioClip magicUpSound;
    private AudioSource magicUp;

    // Start is called before the first frame update
    void Start()
    {
        magicUp = GetComponent<AudioSource>();
        magicUp.clip = magicUpSound;
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            magicUp.Play();
            playerInventory.currentMagic += magicValue;
            powerupSignal.Raise();
            Destroy(this.gameObject);
        }
    }
}
