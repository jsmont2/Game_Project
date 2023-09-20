using System.Collections.Specialized;
//using System.Runtime.Intrinsics; 
using System.Numerics;
//using System.Threading.Tasks.Dataflow;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Vector3 = UnityEngine.Vector3;
using Vector2 = UnityEngine.Vector2;
using Quaternion = UnityEngine.Quaternion;
/*public enum PlayerState //the state machine for the player
{
    attack,
    walk,
    interact,
    stagger,
    idle
}*/

public class PlayerController : character
{
    //public PlayerState currentState;
    //public float speed;
    //private Rigidbody2D rb;
    private Vector3 move;
    private Animator animator;
    public GameObject projectile;
    

    // Start is called before the first frame update
    void Start()
    {
        currentState= characterState.idle;
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        animator.SetFloat("moveX", 0);
        animator.SetFloat("moveY", -1);
    }
    void Update()
    {
        if (Input.GetButtonDown("attack") && currentState != characterState.attack && currentState != characterState.stagger)
                {
                    StartCoroutine(AttackCo());
                }

                else if(Input.GetButtonDown("Second Weapon") && currentState != characterState.attack && currentState != characterState.stagger)
                {
                    StartCoroutine(SecondAttackCo());
                }
    }

    // Update is called once per frame
    void FixedUpdate() //changed Update to FixedUpdate to fix jitter bug
    {
        if (currentState != characterState.stagger)
        {
            move = Vector3.zero;
            move.x = Input.GetAxisRaw("Horizontal"); //GetAxisRaw normalizes the movement so that with each digital press the speed goes to the correct value
            move.y = Input.GetAxisRaw("Vertical");
            if (currentState == characterState.idle || currentState == characterState.walk)
            {
                UpdateAnimationAndMove();
            }

        }  

    }
    void UpdateAnimationAndMove() 
    {
        if (move != Vector3.zero)
        {
            MoveCharacter();    
            animator.SetFloat("moveX", move.x);
            animator.SetFloat("moveY", move.y);
            animator.SetBool("moving",true);

        }else
        {
            animator.SetBool("moving",false);

        }        
    }
    void MoveCharacter() //function that moves player
    {
        //making change to change.normalized normalized the speed when moving the player diagonally
        rb.MovePosition(transform.position + move.normalized * speed * Time.deltaTime); 
    }
   

    private IEnumerator AttackCo()
    {
        animator.SetBool("attacking", true);
        currentState = characterState.attack;
        yield return null;
        animator.SetBool("attacking", false);
        yield return new WaitForSeconds(.15f);
        currentState = characterState.walk;
    }
   
    
    private IEnumerator SecondAttackCo()
    {
        animator.SetBool("attacking", true);
        currentState = characterState.attack;
        yield return null;
        MakeArrow();
        animator.SetBool("attacking", false);
        yield return new WaitForSeconds(.15f);
        currentState = characterState.walk;
    }

    private void MakeArrow()
    {
        Vector2 temp = new Vector2(animator.GetFloat("moveX"), animator.GetFloat("moveY"));
        Arrow arrow = Instantiate(projectile, transform.position,Quaternion.identity).GetComponent<Arrow>();
        arrow.Setup(temp, ChooseArrowDirection());
    }

    Vector3 ChooseArrowDirection()
    {
        float temp = Mathf.Atan2(animator.GetFloat("moveY"), animator.GetFloat("moveX"))* Mathf.Rad2Deg;
        return new Vector3(0, 0, temp);
    }

    //public void Knock(float knockTime)      // Knockback
    //{
    //    StartCoroutine(KnockCo(knockTime));
    //}

    //public IEnumerator KnockCo(float knockTime)
    //{
    //    if(rb != null)
    //    {
    //        yield return new WaitForSeconds(knockTime);
    //        rb.velocity = Vector2.zero;
    //        currentState = characterState.idle;
    //        rb.velocity = Vector2.zero;
    //    }
    //}
}
