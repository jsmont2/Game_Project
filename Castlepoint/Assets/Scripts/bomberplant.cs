using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bomberplant : Enemy
{
    //private Rigidbody2D myRigidbody;
    public Transform target;
    public float chaseRadius;
    public float attackRadius;
    public Transform homePosition;
    public Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        currentState = characterState.idle;
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        target = GameObject.FindWithTag("Player").transform;    //allows the enemy to find the player and chase him
        
    }
    void OnEnable() {
        isElevated = false;
    }
    // Update is called once per frame
    void FixedUpdate()
    {
       if (isElevated == target.gameObject.GetComponent<character>().isElevated)
        {
            CheckDistance();
        }
    }

    void CheckDistance()
    {
        if(Vector3.Distance(target.position, transform.position) <= chaseRadius && Vector3.Distance(target.position, transform.position) > attackRadius) //this checks the distance between enemy and player to dictate what the enemy will do.
        {
            if (currentState == characterState.idle || currentState == characterState.walk && currentState != characterState.stagger )
            {
            Vector3 temp = Vector3.MoveTowards(transform.position, target.position, moveSpeed * Time.deltaTime);

             changeAnim(temp - transform.position);
             rb.MovePosition(temp);

            ChangeState(characterState.walk);
                anim.SetBool("wakeUp", true);
            }
        }   else if (Vector3.Distance(target.position, transform.position) > chaseRadius)
            {
            anim.SetBool("wakeUp", false);
            }
    }
    private void Staggared()
    {

    }
    private void SetAnimFloat(Vector2 setVector)    
    {
        anim.SetFloat("moveX", setVector.x);
        anim.SetFloat("moveY", setVector.y);
    }

    private void changeAnim(Vector2 direction)      //sets the animations for the enemy when it chases the player whether up and down or left or right
    {
        if(Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
        {
            if(direction.x < 0)
            {
                SetAnimFloat(Vector2.right);
            }else if(direction.x < 0)
            {
                SetAnimFloat(Vector2.left);

            }
        } else if(Mathf.Abs(direction.x) < Mathf.Abs(direction.y))
        {
            if(direction.y > 0)
            {
                SetAnimFloat(Vector2.up);
            }
            else if (direction.y < 0)
            {
                SetAnimFloat(Vector2.down);
            }
        }
    }

    private void ChangeState(characterState newState)
    {
        if(currentState != newState)
        {
            currentState = newState;

        }
    }
    
}