using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderElevation : MonoBehaviour
{
    [SerializeField] private GameObject collisionUpperLevel;
    [SerializeField] private GameObject groundCollisionAbovePlayer;
    [SerializeField] private GameObject thisTrigger;
    private void Update()
    {
        SetVariables();
    }
    private void SetVariables()
    {
        if (collisionUpperLevel != null)
        {
            if (GameObject.Find("collision_upper_level").active)
                collisionUpperLevel = GameObject.Find("collision_upper_level");
        }
        if (groundCollisionAbovePlayer != null)
        {
            if (GameObject.Find("GroundCollisionAbovePlayer").active)
                groundCollisionAbovePlayer = GameObject.Find("GroundCollisionAbovePlayer");
        }

    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (this.tag == "Elevation_Down" && (other.tag == "enemy" || other.tag == "Player"))
        {
            Physics2D.IgnoreCollision(collisionUpperLevel.GetComponent<Collider2D>(), other.GetComponent<Collider2D>(), true);
            Physics2D.IgnoreCollision(groundCollisionAbovePlayer.GetComponent<Collider2D>(), other.GetComponent<Collider2D>(), false);
            other.gameObject.GetComponent<Renderer>().sortingOrder = 0;
            other.gameObject.GetComponent<character>().isElevated = false;
        }
        if (this.tag == "Elevation_Up" && (other.tag == "enemy" || other.tag == "Player"))
        {
            Physics2D.IgnoreCollision(collisionUpperLevel.GetComponent<Collider2D>(), other.GetComponent<Collider2D>(), false);
            Physics2D.IgnoreCollision(groundCollisionAbovePlayer.GetComponent<Collider2D>(), other.GetComponent<Collider2D>(), true);
            other.gameObject.GetComponent<Renderer>().sortingOrder = 2;
            other.gameObject.GetComponent<character>().isElevated = true;
        }
    }
    private void OnTriggerEnter2D(Collider2D other) {
        if(this.tag == "Go_In_Room")
        {
            collisionUpperLevel.SetActive(false);
            groundCollisionAbovePlayer.SetActive(false);
        }
        if(this.tag == "Go_Out_Room")
        {
            collisionUpperLevel.SetActive(true);
            groundCollisionAbovePlayer.SetActive(true);
        }
    }
}
