using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveToBossRoom : MonoBehaviour
{
    private GameObject getBossRoom;
    private GameObject previousRoom;
    private Camera getCamera;
    private void Awake()
    {
        getCamera = Camera.main;
        getBossRoom = GameObject.Find("BossRoom_Room#1 ");
        previousRoom = GameObject.Find("D_ToBossRoom_Room#1 ");
        getBossRoom.SetActive(false);
    }
    private void OnTriggerEnter2D(Collider2D other) {
        other.transform.position = new Vector3(2500,2500,0);
        getCamera.GetComponent<CameraMovementDungeon>().maxPosition = new Vector2(2517f,2551f);
        getCamera.GetComponent<CameraMovementDungeon>().minPosition = new Vector2(2482f,2504f);
        getBossRoom.SetActive(true);
        previousRoom.SetActive(false);
    }
}
