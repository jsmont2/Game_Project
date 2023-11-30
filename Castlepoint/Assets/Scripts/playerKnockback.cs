using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerKnockback : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D other)
    {
        var player = other.collider.GetComponent<PlayerController>();
        if (player != null)
        {
            //player.knockBack();
        }
    }
}

