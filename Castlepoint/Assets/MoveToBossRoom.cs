using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveToBossRoom : MonoBehaviour
{
    private Camera getCamera;
    private void Awake()
    {
        getCamera = Camera.main;
    }
    private void OnTriggerEnter2D(Collider2D other) {
        other.transform.position = new Vector3(2500,2500,0);
        getCamera.GetComponent<CameraMovement>().maxPosition = new Vector2(2519f,2531.5f);
        getCamera.GetComponent<CameraMovement>().minPosition = new Vector2(2481f,2469.5f);
    }
}
