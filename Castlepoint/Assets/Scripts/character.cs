using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class character : MonoBehaviour
{
    // Character script for both the player and enemies
    public enum characterState  // the character state machine for all "players" in the game
    {
        idle,
        walk,
        attack, 
        stagger,
        interact
    }

    public enum objectType
    {
        character,
        chest
    }


    public characterState currentState;
    public float speed;
    public Rigidbody2D rb;
    public objectType objType;
    public float maxHealth;
    public float health;
    public string enemyName;
    public int baseAttack;
    public float moveSpeed;
    public SpriteRenderer sprite;
    //public Animator hitAnim; //tried to set the animation for when the enemy gets hit
    protected float knockTime = 0.5f;
    public float thrust;
    public float damage;
    public GameObject thisObject;

    public void Knock(Transform other) // Knockback
    {
            Rigidbody2D hit = this.GetComponent<Rigidbody2D>();
            Vector2 difference = hit.transform.position - other.position;
            difference = difference.normalized * thrust;
            hit.AddForce(difference, ForceMode2D.Impulse);

            if (hit != null)
            {
                if (this.gameObject.CompareTag("enemy"))
                {

                    hit.GetComponent<character>().currentState = characterState.stagger;
                    this.GetComponent<character>().KnockCo(hit, knockTime);
                    //anim.SetTrigger("hit");
                }
                if (this.gameObject.CompareTag("Player"))
                {

                    hit.GetComponent<PlayerMovement>().currentState = characterState.stagger;
                    this.GetComponent<PlayerMovement>().Knock(knockTime);
                }
            }

            StartCoroutine(KnockCo(hit, knockTime));
            TakeDamage(damage);
     
    }


    private IEnumerator KnockCo(Rigidbody2D myRigidbody, float knockTime)
    {
        if (myRigidbody != null)
        {
            yield return new WaitForSeconds(knockTime);
            myRigidbody.velocity = Vector2.zero;
            myRigidbody.GetComponent<Enemy>().currentState = characterState.idle;
            myRigidbody.velocity = Vector2.zero;
        }
    }

    private void TakeDamage(float damage)
    {
        health -= damage;
        if (health > 0)
        {

        }
        if (health <= 0)
        {

            this.gameObject.SetActive(false);
        }
    }



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public objectType GetType()
    {
        return objType;
    }


}
