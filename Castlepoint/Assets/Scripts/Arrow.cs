using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    public float speed2;
    public Rigidbody2D myRigidbody;
    public float lifetime;
    private float lifetimeCounter;
    public float magicCost;

    // Start is called before the first frame update
    void Start()
    {
       lifetimeCounter = lifetime;

    }
    private void Awake() {
    }
    private void Update()// destroys arrows that dont hit an enemy
    {
        lifetimeCounter -= Time.deltaTime;
        if(lifetimeCounter <= 0)
        {
            Destroy(this.gameObject);
        }
    }

    public void Setup(Vector2 velocity, Vector3  direction) // changes arrow's direction 
    {
        myRigidbody.velocity = velocity.normalized * speed2;
        transform.rotation = Quaternion.Euler(direction);
    }
    void OnCollisionEnter2D(Collision2D collision)  
    {
        if (collision.gameObject.CompareTag("enemy"))
        {
            GameObject temp = collision.gameObject;
            Destroy(this.gameObject);
            
        }
    }




}