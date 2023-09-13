using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*public enum characterState {
    idle, 
    walk,
    attack,
    stagger
}*/
public class Enemy : character
{
    //public characterState currentState;
    public FloatValue maxHealth;
    public float health;
    public string enemyName;
    public int baseAttack;
    public float moveSpeed;
    public SpriteRenderer sprite;
    //public Animator hitAnim; //tried to set the animation for when the enemy gets hit
    private float knockTime = 0.5f;

    //code that makes the enemies flash red when hit
    public IEnumerator FlashRed()
    {
        sprite.color = Color.red;
        yield return new WaitForSeconds(0.1f); //f is used when using a decimal and not a whole number which stands for float.
        sprite.color = Color.white;// after turning red for a second, the sprite will go back to turning white since the default color for all enemies/sprites are already white even though they appear to be normal(i.e. they won't turn plain white
    }

    void OnCollisionEnter2D(Collision2D collision)  // Knockback
    {
        if (collision.gameObject.CompareTag("hurtBox"))
        {
            collision.gameObject.GetComponent<PlayerMovement>().Knock(knockTime);
        }
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "sword")
        {
            StartCoroutine(FlashRed());
            //hitAnim = GetComponent<Animator>();
            //hitAnim.SetBool("hit", true); // tried to set the animation for when the enemy gets hit
            //hitAnim.SetBool("hit", false);
        }

    }


    private void Awake()
    {
        health = maxHealth.initialValue;
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
    
    public void Knock(Rigidbody2D myRigidbody, float knockTime, float damage) // Knockback
    {
        StartCoroutine(KnockCo(myRigidbody,knockTime));
        TakeDamage(damage);
    }


    private IEnumerator KnockCo(Rigidbody2D myRigidbody, float knockTime) 
    {
        if (myRigidbody != null)
        {
            yield return new WaitForSeconds(knockTime);
            myRigidbody.velocity = Vector2.zero;
            myRigidbody.GetComponent<Enemy>().currentState= characterState.idle;
            myRigidbody.velocity = Vector2.zero;
        }
    }
}
