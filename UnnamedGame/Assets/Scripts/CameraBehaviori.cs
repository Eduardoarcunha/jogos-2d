using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Camera))]
public class CameraBehavior : MonoBehaviour
{
    public Transform target; // The character or target the camera follows
    public float xOffsetForward = 2f; // Offset when moving forward
    public float xOffsetBackward = -1f; // Offset when moving backward
    public float smoothTime = 0.3f; // Damping time for smooth movement

    private Camera _cam;
    private Vector3 _currentVelocity;
    public bool isGrounded; // This should probably be private and set via some other collision logic
    private float previousGroundedY;
    private bool wasGrounded;
    private float yGroundedDifference;
    private bool isShaking = false;
    public Image redPanel; // Drag your red UI panel here in the inspector
    public float blinkDuration = 0.7f; // Duration of one blink (fade in and out)


    void Start()
    {
        PlayerController.OnChangeGroundedState += HandleChangeGroundedState;
        _cam = GetComponent<Camera>();
        if (isGrounded)
        {
            previousGroundedY = target.position.y;
        }
        wasGrounded = isGrounded;
    }

    private void HandleChangeGroundedState(bool isGroundedState)
    {
        isGrounded = isGroundedState;
    }

    public IEnumerator Shake(float duration, float magnitude)
    {

        Debug.Log("Shake");
        Vector3 originalPos = transform.localPosition;
        float elapsed = 0.0f;
        while (elapsed < duration)
        {
            float x = UnityEngine.Random.Range(-1f, 1f) * magnitude;
            float y = UnityEngine.Random.Range(-1f, 1f) * magnitude;
            transform.localPosition = originalPos + new Vector3(x, y, originalPos.z);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.localPosition = originalPos;
    }

public void ResetPanelColor()
{
    redPanel.color = new Color(0.2824f, 0, 0, 0); // Reset to transparent color
}

public IEnumerator BlinkRoutine()
{
    Color startColor = new Color(0.2824f, 0, 0, 0.2118f/2);      // Start with 0 alpha
    Color targetColor = new Color(0.2824f, 0, 0, 0.2118f); // Target alpha of 0.2118

    while (true) // Loop indefinitely
    {
        float elapsed = 0;

        // Fade in
        while (elapsed < blinkDuration / 2)
        {
            elapsed += Time.deltaTime;
            redPanel.color = Color.Lerp(startColor, targetColor, elapsed / (blinkDuration / 2));
            yield return null;
        }

        elapsed = 0;

        // Fade out
        while (elapsed < blinkDuration / 2)
        {
            elapsed += Time.deltaTime;
            redPanel.color = Color.Lerp(targetColor, startColor, elapsed / (blinkDuration / 2));
            yield return null;
        }
    }
}



    void Update()
    {

        
        // Check if the player was not grounded in the previous frame but is now
        if (!wasGrounded && isGrounded)
        {
            float currentGroundedY = target.position.y;
            yGroundedDifference = currentGroundedY - previousGroundedY;

            // Update the previous grounded position
            previousGroundedY = currentGroundedY;
        }

        // Update wasGrounded for the next frame
        wasGrounded = isGrounded;
    }

    void LateUpdate()
    {
        if(!isShaking)
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

            if(yGroundedDifference > 0 || target.position.x < 30 || target.position.y < -25)
            {

                if (target.position.y > transform.position.y + yOffset ) // If above the center line
                {
                    // Debug.Log("Above center line");
                    desiredPosition.y = target.position.y + yOffset; // Move up to keep the target below the center line
                }
                else if (target.position.y < transform.position.y - yOffset) // If below the center line
                {
                    // Debug.Log("Below center line");
                    desiredPosition.y = target.position.y - yOffset; // Move down to keep the target above the center line
                }
            }
            else
            {

                if (target.position.y > transform.position.y + 2*yOffset) 
                {
                    // Debug.Log("Above center line");
                    desiredPosition.y = target.position.y + yOffset; 
                }
                else if (target.position.y < transform.position.y+ yOffset) 
                {
                    // Debug.Log("Below center line");
                    desiredPosition.y = target.position.y - yOffset;
                }
            }
            

            // Apply damping
            transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref _currentVelocity, smoothTime);
            
            // Keep the Z position of the camera the same
            transform.position = new Vector3(transform.position.x, transform.position.y, -10f);
                
        }
        
    }
}
