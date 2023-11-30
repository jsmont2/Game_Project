using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderElevation : MonoBehaviour
{
    [SerializeField] private GameObject collisionUpperLevel;
    [SerializeField] private GameObject groundCollisionAbovePlayer;
    // Start is called before the first frame update
    private void OnTriggerEnter2D(Collider2D other) {
        if(this.tag == "Elevation_Down" && (other.tag == "enemy" || other.tag == "Player"))
        {
            Physics2D.IgnoreCollision(collisionUpperLevel.GetComponent<Collider2D>(), other.GetComponent<Collider2D>(), true);
            Physics2D.IgnoreCollision(groundCollisionAbovePlayer.GetComponent<Collider2D>(), other.GetComponent<Collider2D>(), false);
            other.gameObject.GetComponent<Renderer>().sortingOrder = 0;
            other.gameObject.GetComponent<character>().isElevated = false;
        }
        if(this.tag == "Elevation_Up" && (other.tag == "enemy" || other.tag == "Player"))
        {
            Physics2D.IgnoreCollision(collisionUpperLevel.GetComponent<Collider2D>(), other.GetComponent<Collider2D>(), false);
            Physics2D.IgnoreCollision(groundCollisionAbovePlayer.GetComponent<Collider2D>(), other.GetComponent<Collider2D>(), true);
            other.gameObject.GetComponent<Renderer>().sortingOrder = 2;
            other.gameObject.GetComponent<character>().isElevated = true;
        }
    }
}
