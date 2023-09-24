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
    //public FloatValue maxHealth;
    //public float health;
    //public string enemyName;
    //public int baseAttack;
    //public float moveSpeed;
    //public SpriteRenderer sprite;
    ////public Animator hitAnim; //tried to set the animation for when the enemy gets hit
    //private float knockTime = 0.5f;


    //code that makes the enemies flash red when hit
    public IEnumerator FlashRed()
    {
        sprite.color = Color.red;
        yield return new WaitForSeconds(0.1f); //f is used when using a decimal and not a whole number which stands for float.
        sprite.color = Color.white;// after turning red for a second, the sprite will go back to turning white since the default color for all enemies/sprites are already white even though they appear to be normal(i.e. they won't turn plain white
    }

    void OnCollisionEnter2D(Collision2D collision)  // Knockback and damage
    {
        if (isElevated == GameObject.FindGameObjectWithTag("Player").gameObject.GetComponent<character>().isElevated)
        {
            Physics2D.IgnoreCollision(this.GetComponent<Collider2D>(), collision.gameObject.GetComponent<Collider2D>(), false);
            if (collision.gameObject.CompareTag("Player"))
            {
                GameObject temp = collision.gameObject;
                //temp.GetComponent<character>().Knock(temp.transform, thrust, temp.GetComponent<character>().getknockTime());            
            }
            Debug.Log("knock");
            GameObject temp = collision.gameObject;
            temp.GetComponent<character>().Knock(temp.transform, thrust, temp.GetComponent<character>().getknockTime());            
        }
        else if(collision.gameObject.tag == "arrow") // trying to get arrow to do damage 
        {
            Debug.Log("hit");
            StartCoroutine(FlashRed());
            Knock(collision.transform, GameObject.FindGameObjectWithTag("arrow").GetComponent<character>().getThrust(), GameObject.FindGameObjectWithTag("arrow").GetComponent<character>().getknockTime());
            TakeDamage(GameObject.FindGameObjectWithTag("arrow").GetComponent<character>().getDmg());
        }
        else { Physics2D.IgnoreCollision(this.GetComponent<Collider2D>(), collision.gameObject.GetComponent<Collider2D>(), true); }

    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (isElevated == GameObject.FindGameObjectWithTag("Player").gameObject.GetComponent<character>().isElevated)
        {
            if (other.gameObject.tag == "sword")
            {
                StartCoroutine(FlashRed());
                Knock(other.transform, GameObject.FindGameObjectWithTag("Player").GetComponent<character>().getThrust(), GameObject.FindGameObjectWithTag("Player").GetComponent<character>().getknockTime());
                TakeDamage(GameObject.FindGameObjectWithTag("Player").GetComponent<character>().getDmg());
                //hitAnim = GetComponent<Animator>();
                //hitAnim.SetBool("hit", true); // tried to set the animation for when the enemy gets hit
                //hitAnim.SetBool("hit", false);
            }
        }
        
    }


    private void Awake()
    {
        animator = GetComponent<Animator>();
    }




}
