using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public Transform target; // The object you want the camera to follow
    public float smoothing; // How quickly the camera moves towards the target.
    public Vector2 maxPosition;
    public Vector2 minPosition;

    // Set the initial target in Start()
    void Start()
    {
        // Make sure to set the initial target (player) in the Unity Editor or dynamically in your script
        SetTarget(GameObject.FindGameObjectWithTag("Player").transform);
    }

    // LateUpdate is called once per frame, after other updates
    void LateUpdate()
    {
        if (target != null && transform.position != target.position)
        {
            Vector3 targetPosition = new Vector3(target.position.x, target.position.y, transform.position.z);

            targetPosition.x = Mathf.Clamp(targetPosition.x, minPosition.x, maxPosition.x);
            targetPosition.y = Mathf.Clamp(targetPosition.y, minPosition.y, maxPosition.y);

            transform.position = Vector3.Lerp(transform.position, targetPosition, smoothing);
        }
    }

    // Method to dynamically set the camera target
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }
}
