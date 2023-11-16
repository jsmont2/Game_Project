using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pressure_pad : MonoBehaviour
{

    public Sprite pressed;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("box"))
        {
            // Change the sprite when colliding with an object with the specified tag
            SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();

            if (spriteRenderer != null && pressed != null)
            {
                spriteRenderer.sprite = pressed;
                Debug.Log("pressure pad pressed");
            }
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
