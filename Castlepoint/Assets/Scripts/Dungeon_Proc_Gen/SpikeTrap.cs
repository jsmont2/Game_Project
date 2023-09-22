using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeTrap : MonoBehaviour
{
    Animator anim;
    private void Awake()
    {
        anim = this.GetComponent<Animator>();
        anim.SetBool("Spikes_Down", true);
        AnimateSpikeTrap();
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
            yield return new WaitForSeconds(2f);
            anim.SetBool("Spikes_Up", false);
            anim.SetBool("Spikes_Lowering", true);
            yield return new WaitForSeconds(.3f);
            anim.SetBool("Spikes_Lowering", false);
            anim.SetBool("Spikes_Down", true);
            yield return new WaitForSeconds(2f);
        }

    }
    private void OnTriggerStay2D(Collider2D other) {
        if (other.tag == "Player")
        {
            if (anim.GetBool("Spikes_Up") || anim.GetBool("Spikes_Lowering"))
            {
                other.GetComponent<character>().Knock(this.transform, 10f, .1f);
                other.GetComponent<character>().TakeDamage(1);
            }
        }
    }
        
    
}
