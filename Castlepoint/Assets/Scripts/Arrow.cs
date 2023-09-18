using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : character
{
    public float speed2;
    public Rigidbody2D myRigidbody;
    public float lifetime;
    private float lifetimeCounter;
    public float magicCost;

    // Start is called before the first frame update
    void Start()
    {
       
    }

    private void Update()
    {

    }

    public void Setup(Vector2 velocity, Vector3  direction)
    {
        myRigidbody.velocity = velocity.normalized * speed2;
        transform.rotation = Quaternion.Euler(direction);
    }
    void OnCollisionEnter2D(Collision2D collision)  // Knockback and damage
    {
        if (collision.gameObject.CompareTag("enemy"))
        {
            Debug.Log("knock");
            GameObject temp = collision.gameObject;
            temp.GetComponent<character>().Knock(temp.transform, thrust, temp.GetComponent<character>().getknockTime());
            Destroy(this.gameObject);
            
        }
    }




}