using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderElevation : MonoBehaviour
{
    [SerializeField] private GameObject collisionUpperLevel;
    [SerializeField] private GameObject abovePlayer;
    [SerializeField] private GameObject groundCollisionAbovePlayer;
    // Start is called before the first frame update
    private void OnTriggerEnter2D(Collider2D other) {
        if(this.tag == "Elevation_Down" && other.tag == "Player")
        {
            collisionUpperLevel.SetActive(false);
            abovePlayer.SetActive(true);
            groundCollisionAbovePlayer.SetActive(true);
        }
        if(this.tag == "Elevation_Up" && other.tag == "Player")
        {
            collisionUpperLevel.SetActive(true);
            abovePlayer.SetActive(false);
            groundCollisionAbovePlayer.SetActive(false);
        }
    }
}
