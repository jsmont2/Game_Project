using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeTrap : MonoBehaviour
{
    private Animator anim;
    private AudioSource shing;
    
    private void Awake()
    {
        anim = this.GetComponent<Animator>();
        anim.SetBool("Spikes_Down", true);
        shing = this.GetComponent<AudioSource>();
    }
    private void OnBecameVisible() {
        AnimateSpikeTrap();
    }
    private void OnBecameInvisible() {
        anim.SetBool("Spikes_Down", true);
        anim.SetBool("Spikes_Rising", false);
        anim.SetBool("Spikes_Up", false);
        anim.SetBool("Spikes_Lowering", false);
    }
    private void AnimateSpikeTrap()
    {
        StartCoroutine(RaiseSpikes());
    }
    IEnumerator RaiseSpikes()
    {
        while (true)
        {
            anim.SetBool("Spikes_Down", false);
            anim.SetBool("Spikes_Rising", true);
            yield return new WaitForSeconds(1.3f);
            anim.SetBool("Spikes_Rising", false);
            anim.SetBool("Spikes_Up", true);
            shing.Play();
            yield return new WaitForSeconds(2f);
            anim.SetBool("Spikes_Up", false);
            anim.SetBool("Spikes_Lowering", true);
            yield return new WaitForSeconds(.3f);
            anim.SetBool("Spikes_Lowering", false);
            anim.SetBool("Spikes_Down", true);
            yield return new WaitForSeconds(8f);
        }

    }
    private void OnTriggerStay2D(Collider2D other) {
        if (other.tag == "Player")
        {
            if (anim.GetBool("Spikes_Up"))
            {
                other.GetComponent<character>().Knock(this.transform, 10f, .1f);
                other.GetComponent<character>().TakeDamage(1);
            }
        }
    }
        
    
}
