using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraBehavior : MonoBehaviour
{
    public Transform target; // The character or target the camera follows
    public float xOffsetForward = 2f; // Offset when moving forward
    public float xOffsetBackward = -1f; // Offset when moving backward
    public float smoothTime = 0.3f; // Damping time for smooth movement

    private Camera _cam;
    private Vector3 _currentVelocity;




    void Start()
    {
        _cam = GetComponent<Camera>();
    }

    void LateUpdate()
    {
        if (target == null) return;

        float yOffset = _cam.orthographicSize/2; // Half the camera's height, so we get the middle

        Vector3 desiredPosition = transform.position;
        float targetDirection = target.GetComponent<Rigidbody2D>().velocity.x;

        // Only adjust camera's desired X position if there's significant movement
        if (Mathf.Abs(targetDirection) > 0.01f)  // Use 0.01f or some small value to filter out negligible movement
        {
            if (targetDirection > 0)
            {
                desiredPosition.x = target.position.x + xOffsetForward;
            }
            else if (targetDirection < 0)
            {
                desiredPosition.x = target.position.x + xOffsetBackward;
            }
        }
        

        // Calculate Y position
        if (target.position.y > transform.position.y + yOffset) // If above the center line
        {
            Debug.Log("Above center line");
            desiredPosition.y = target.position.y - yOffset; // Move up to keep the target below the center line
        }
        else if (target.position.y < transform.position.y - yOffset) // If below the center line
        {
            Debug.Log("Below center line");
            desiredPosition.y = target.position.y + yOffset; // Move down to keep the target above the center line
        }

        // Apply damping
        transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref _currentVelocity, smoothTime);
        
        // Keep the Z position of the camera the same
        transform.position = new Vector3(transform.position.x, transform.position.y, -10f);
    }
}
