using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knockback : character
{
    //public float thrust;
    //public float knockTime;
    //public float damage;
    //public Animator anim;




    private void OnTriggerEnter2D(Collider2D other)
    {

        //if (other.gameObject.GetComponent<character>().GetType() == objectType.character)
        //{
        //    Rigidbody2D hit = other.GetComponent<Rigidbody2D>();
        //    Vector2 difference = hit.transform.position - transform.position;
        //    difference = difference.normalized * thrust;
        //    hit.AddForce(difference, ForceMode2D.Impulse);

        //    if (hit != null)
        //    {
        //        if (other.gameObject.CompareTag("enemy"))
        //        {

        //            hit.GetComponent<character>().currentState = characterState.stagger;
        //            other.GetComponent<character>().Knock(hit, knockTime, damage);
        //            //anim.SetTrigger("hit");
        //        }
        //        if (other.gameObject.CompareTag("Player"))
        //        {

        //            hit.GetComponent<PlayerController>().currentState = characterState.stagger;
        //            other.GetComponent<PlayerController>().Knock(knockTime);
        //        }
        //    }
        //}

        //other.GetComponent<character>().Knock(other);


    }
}
        