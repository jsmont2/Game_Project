using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pressure_pad : MonoBehaviour, IDataPersistence
{
    public void LoadData(GameData data)
    {

    }
    public void SaveData(GameData data)
    {
        
    }
    public Sprite pressed;
    public Sprite unpressed;

    private SpriteRenderer spriteRenderer;

    public enum PressurePadState
    {
        unpressed,
        pressed
    }

    public PressurePadState currentState;

    // Start is called before the first frame update
    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        unpressed = spriteRenderer.sprite;
        currentState = PressurePadState.unpressed;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("box") || other.CompareTag("Player"))
        {
            Debug.Log("Pressure pad pressed");
            ChangeSprite(pressed);
            currentState = PressurePadState.pressed;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("box") || other.CompareTag("Player"))
        {
            Debug.Log("Pressure pad released");
            ChangeSprite(unpressed);
            currentState= PressurePadState.unpressed;
        }
    }

    private void ChangeSprite(Sprite newSprite)
    {
        // Change the sprite
        if (spriteRenderer != null && newSprite != null)
        {
            spriteRenderer.sprite = newSprite;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public PressurePadState GetPressurePadState()
    {
        return currentState;
    }
}
