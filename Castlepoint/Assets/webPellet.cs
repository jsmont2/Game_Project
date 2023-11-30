using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class webPellet : MonoBehaviour
{
    public Rigidbody2D thisBody;
    public float speed;
    public float damage;


    private void Update()
    {
        transform.Translate(Vector2.up * speed * Time.deltaTime);
    }


    private void Awake()
    {
        thisBody = GetComponent<Rigidbody2D>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("collision") || other.CompareTag("Player"))
        {
            other.gameObject.GetComponent<character>().TakeDamage(damage);
            Destroy(this.gameObject);
        }
    }
}
