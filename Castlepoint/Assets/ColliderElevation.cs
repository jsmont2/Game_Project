using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderElevation : MonoBehaviour
{
    [SerializeField] private GameObject collisionUpperLevel;
    [SerializeField] private GameObject groundCollisionAbovePlayer;
    private void OnEnable() {
        StartCoroutine(SetVariables());
    }
    private IEnumerator SetVariables()
    {
        yield return new WaitForSeconds(.3f);
        collisionUpperLevel = GameObject.Find("collision_upper_level");
        groundCollisionAbovePlayer = GameObject.Find("GroundCollisionAbovePlayer");
    }
    private IEnumerator WaitForRoomToSpawn(Collider2D other)
    {
        yield return new WaitForSeconds(.4f);
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
    private void OnTriggerEnter2D(Collider2D other) {
        StartCoroutine(WaitForRoomToSpawn(other));
    }
}
