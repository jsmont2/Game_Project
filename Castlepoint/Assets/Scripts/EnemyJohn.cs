using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyJohn : character
{
    public Rigidbody2D myRigidbody;
    public Transform target;
    public float chaseRadius;
    public float attackRadius;
    public Transform homePosition;
    public Animator anim;
    //code that makes the enemies flash red when hit
    public IEnumerator FlashRed()
    {
        sprite.color = Color.red;
        yield return new WaitForSeconds(0.1f); //f is used when using a decimal and not a whole number which stands for float.
        sprite.color = Color.white;// after turning red for a second, the sprite will go back to turning white since the default color for all enemies/sprites are already white even though they appear to be normal(i.e. they won't turn plain white
    }
    public void CheckDistance()
    {
        if (Vector3.Distance(target.position, transform.position) <= chaseRadius && Vector3.Distance(target.position, transform.position) > attackRadius) //this checks the distance between enemy and player to dictate what the enemy will do.
        {
            if (currentState == characterState.idle || currentState == characterState.walk && currentState != characterState.stagger)
            {
                Vector3 temp = Vector3.MoveTowards(transform.position, target.position, moveSpeed * Time.deltaTime);

                changeAnim(temp - transform.position);
                myRigidbody.MovePosition(temp);

                ChangeState(characterState.walk);
                anim.SetBool("wakeUp", true);
            }
        }
        else if (Vector3.Distance(target.position, transform.position) > chaseRadius)
        {
            anim.SetBool("wakeUp", false);
        }
    }
    private void changeAnim(Vector2 direction)      //sets the animations for the enemy when it chases the player whether up and down or left or right
    {
        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
        {
            if (direction.x < 0)
            {
                SetAnimFloat(Vector2.right);
            }
            else if (direction.x < 0)
            {
                SetAnimFloat(Vector2.left);
            }
        }
        else if (Mathf.Abs(direction.x) < Mathf.Abs(direction.y))
        {
            if (direction.y > 0)
            {
                SetAnimFloat(Vector2.up);
            }
            else if (direction.y < 0)
            {
                SetAnimFloat(Vector2.down);
            }
        }
        if(this.charType == characterType.boss)
        {
            if (direction.x < 0)
            {
                this.transform.rotation = Quaternion.Euler(0,180,0);
            }
            else{this.transform.rotation = Quaternion.Euler(0,0,0);}
        }
    }
    private void SetAnimFloat(Vector2 setVector)
    {
        anim.SetFloat("moveX", setVector.x);
        anim.SetFloat("moveY", setVector.y);
    }
    
    void OnCollisionEnter2D(Collision2D collision)  // Knockback and damage
    {
        if (isElevated == GameObject.FindGameObjectWithTag("Player").gameObject.GetComponent<character>().isElevated)
        {
            Physics2D.IgnoreCollision(this.GetComponent<Collider2D>(), collision.gameObject.GetComponent<Collider2D>(), false);
            if (collision.gameObject.CompareTag("Player"))
            {
                
                GameObject temp = collision.gameObject;
                collision.gameObject.GetComponent<character>().TakeDamage(damage); 
                if(collision.gameObject.GetComponent<character>().health != 0)
                {
                    collision.gameObject.GetComponent<character>().Knock(this.transform, thrust, knockTime);   
                }  
                                  
            }
            if (collision.gameObject.tag == "arrow") // trying to get arrow to do damage 
            {
                StartCoroutine(FlashRed());
                Knock(collision.transform, GameObject.FindGameObjectWithTag("Player").GetComponent<character>().getThrust(), GameObject.FindGameObjectWithTag("Player").GetComponent<character>().getknockTime());
                TakeDamage(GameObject.FindGameObjectWithTag("Player").GetComponent<character>().getDmg());
            }
        }
        else { Physics2D.IgnoreCollision(this.GetComponent<Collider2D>(), collision.gameObject.GetComponent<Collider2D>(), true); }

    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (isElevated == GameObject.FindGameObjectWithTag("Player").gameObject.GetComponent<character>().isElevated && currentState != characterState.invulnerable)
        {
            if (other.gameObject.tag == "sword")
            {
                StartCoroutine(FlashRed());
                Knock(other.transform, GameObject.FindGameObjectWithTag("Player").GetComponent<character>().getThrust(), GameObject.FindGameObjectWithTag("Player").GetComponent<character>().getknockTime());
                TakeDamage(GameObject.FindGameObjectWithTag("Player").GetComponent<character>().getDmg());
           
            }
        }

    }


    private void Awake()
    {
        animator = GetComponent<Animator>();
    }




}
