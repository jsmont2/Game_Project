using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room_Transition : MonoBehaviour
{
    [SerializeField]
    private Camera getCamera;
    [SerializeField]
    private Vector2 cameraCffset;
    private void Awake()
    {
        getCamera = Camera.main;
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            getCamera.GetComponent<CameraMovement>().maxPosition += cameraCffset;
            getCamera.GetComponent<CameraMovement>().minPosition += cameraCffset;
        }
    }
}
