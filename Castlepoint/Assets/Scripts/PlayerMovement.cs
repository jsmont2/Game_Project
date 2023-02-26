using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed;
    private Rigidbody2D rb;
    private Vector3 change;
    private Animator animator;


    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate() //changed Update to FixedUpdate to fix jitter bug
    {
        change = Vector3.zero;
        change.x = Input.GetAxisRaw("Horizontal"); //GetAxisRaw normalizes the movement so that with each digital press the speed goes to the correct value
        change.y = Input.GetAxisRaw("Vertical");
        UpdateAnimationAndMove();

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
