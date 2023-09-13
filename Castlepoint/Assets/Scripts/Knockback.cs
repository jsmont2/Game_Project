using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knockback : character
{
    public float thrust;
    public float knockTime;
    public float damage;
    public Animator anim;

    private void OnTriggerEnter2D(Collider2D other)
    {
        /*if(other.gameObject.CompareTag("enemy") || other.gameObject.CompareTag("Player"))
        {
            other.GetComponent<pinkslime>().Hit();
        }*/ 

        if (other.gameObject.CompareTag("enemy"))
        {
            Rigidbody2D hit = other.GetComponent<Rigidbody2D>();
            Vector2 difference = hit.transform.position - transform.position;
            difference = difference.normalized * thrust;
            hit.AddForce(difference, ForceMode2D.Impulse);            
       
            if (hit != null)
            {
                if (other.gameObject.CompareTag("enemy") && other.isTrigger)
                {
                     hit.GetComponent<Enemy>().currentState = characterState.stagger;
                     other.GetComponent<Enemy>().Knock(hit, knockTime, damage);
                     //anim.SetTrigger("hit");
                }
                if (other.gameObject.CompareTag("Player"))
                {

                    hit.GetComponent<PlayerMovement>().currentState = characterState.stagger;
                    other.GetComponent<PlayerMovement>().Knock(knockTime);    
                }
            }
        }

        if (other.gameObject.CompareTag("Player"))
        {
            Rigidbody2D hit = other.GetComponent<Rigidbody2D>();
            Vector2 difference = hit.transform.position - transform.position;
            difference = difference.normalized * thrust;
            hit.AddForce(difference, ForceMode2D.Impulse);

            if (hit != null)
            {
                if (other.gameObject.CompareTag("Player") && other.isTrigger)
                {
                    hit.GetComponent<Enemy>().currentState = characterState.stagger;
                    other.GetComponent<Enemy>().Knock(hit, knockTime, damage);
                    //anim.SetTrigger("hit");
                }
                if (other.gameObject.CompareTag("enemy"))
                {
                    hit.GetComponent<PlayerMovement>().currentState = characterState.stagger;
                    other.GetComponent<PlayerMovement>().Knock(knockTime);
                }
            }
        }   

    }

}
        