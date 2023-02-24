using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed;
    private Rigidbody2D rb;
    private Vector3 change;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        change = Vector2.zero;
        change.x = Input.GetAxisRaw("Horizontal"); //getaxisraw normalizes the movement when moving diagonal so that both vectors aren't added 
        change.y = Input.GetAxisRaw("Vertical");
        if (change != Vector3.zero)
        {
            MoveCharacter();    //Joel: left off at part 2
        }
    }

    void MoveCharacter() //function that moves player
    {
        rb.MovePosition(transform.position + change * speed * Time.deltaTime);
    }

}
