using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerState
{
    attack,
    walk,
    interact
}

public class PlayerMovement : MonoBehaviour
{
    public PlayerState currentState;
    public float speed;
    private Rigidbody2D rb;
    private Vector3 change;
    private Animator animator;


    // Start is called before the first frame update
    void Start()
    {
        currentState= PlayerState.walk;
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }
    void Update()
    {
        if (Input.GetButtonDown("attack") && currentState != PlayerState.attack)
                {
                    StartCoroutine(AttackCo());
                }
    }

    // Update is called once per frame
    void FixedUpdate() //changed Update to FixedUpdate to fix jitter bug
    {
        change = Vector3.zero;
        change.x = Input.GetAxisRaw("Horizontal"); //GetAxisRaw normalizes the movement so that with each digital press the speed goes to the correct value
        change.y = Input.GetAxisRaw("Vertical");
        if (currentState == PlayerState.walk) 
        {
            UpdateAnimationAndMove();
        }
    }
   

    private IEnumerator AttackCo()
    {
        animator.SetBool("attacking", true);
        currentState = PlayerState.attack;
        yield return null;
        animator.SetBool("attacking", false);
        yield return new WaitForSeconds(.15f);
        currentState = PlayerState.walk;
    }
    void UpdateAnimationAndMove() // Omar: Left off at part 3
    {
        if (change != Vector3.zero)
        {
            MoveCharacter();    //Joel: left off at part 2
            animator.SetFloat("moveX", change.x);
            animator.SetFloat("moveY", change.y);
            animator.SetBool("moving",true);

        }else
        {
            animator.SetBool("moving",false);
        }        
    }

    void MoveCharacter() //function that moves player
    {
        rb.MovePosition(transform.position + change.normalized * speed * Time.deltaTime); //making change to change.normalized normalized the speed when moving the player diagonally
    }

}
