using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class door : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public Sprite newSprite;
    void ChangeSprite()
    {
        spriteRenderer.sprite = newSprite;
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
