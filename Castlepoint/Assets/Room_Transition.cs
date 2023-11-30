using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;

public class Room_Transition : MonoBehaviour
{
    [SerializeField]
    private Camera getCamera;
    [SerializeField]
    private Vector2 cameraCffset;
    [SerializeField]
    private Vector3 playerOffset;
    [SerializeField]
    private Room thisRoom;
    private void Awake()
    {
        getCamera = Camera.main;
        thisRoom = this.transform.parent.gameObject.transform.parent.GetComponent<Room>();
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            getCamera.GetComponent<CameraMovement>().maxPosition += cameraCffset;
            getCamera.GetComponent<CameraMovement>().minPosition += cameraCffset;
            other.gameObject.transform.position += playerOffset;
            TurnRoomOff();
        }
    }
    private void TurnRoomOff()
    {
        if(this.tag == "TransitionUp")
        {
            thisRoom.adjacentRooms.GetRoomAbove().gameObject.SetActive(true);
            thisRoom.gameObject.SetActive(false);
        }
        if(this.tag == "TransitionDown")
        {
            thisRoom.adjacentRooms.GetRoomBelow().gameObject.SetActive(true);
            thisRoom.gameObject.SetActive(false);
        }
        if(this.tag == "TransitionLeft")
        {
            thisRoom.adjacentRooms.GetRoomLeft().gameObject.SetActive(true);
            thisRoom.gameObject.SetActive(false);
        }
        if(this.tag == "TransitionRight")
        {
            thisRoom.adjacentRooms.GetRoomRight().gameObject.SetActive(true);
            thisRoom.gameObject.SetActive(false);
        }
    }
}
