using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public Transform target; //The obj you want the camera to follow
    public  float smoothing; // how quickly the cam moves towards the target.
    public Vector2 maxPosition;
    public Vector2 minPosition;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    private void Update() {
        target = GameObject.FindGameObjectWithTag("Player").transform;
    }
    // Update is called once per frame
    void LateUpdate()
    {
        if(transform.position != target.position)//this finds the distance between itself and the target so it can move at a certain percentage.
        {
            Vector3 targetPosition = new Vector3(target.position.x,target.position.y, transform.position.z);

            targetPosition.x = Mathf.Clamp(targetPosition.x,minPosition.x, maxPosition.x);
            targetPosition.y = Mathf.Clamp(targetPosition.y,minPosition.y, maxPosition.y);
            
            transform.position = Vector3.Lerp(transform.position,targetPosition, smoothing);

        }

    }
}
