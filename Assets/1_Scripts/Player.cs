using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [Header("Move")]
    public float speed;
    public Vector2 moveInput;

    [Header("Headbob Settings")] 
    public Transform cameraTransform;
    
    private float bobTimer = 0f;
    private float defaultCameraY = 0f;
    
    void Update()
    {
        Vector3 moveDirection = new Vector3(moveInput.x, 0f, moveInput.y);
        transform.Translate(speed * Time.deltaTime * moveDirection);
        
        HeadBob();
    }
    
    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    private void HeadBob()
    {
        if (cameraTransform == null) return;

        if (moveInput.sqrMagnitude > 0f)
        {
            bobTimer += Time.deltaTime * 11f;

            float newY = defaultCameraY + Mathf.Sin(bobTimer) * 0.05f;

            cameraTransform.localPosition = new Vector3(cameraTransform.localPosition.x, newY, cameraTransform.localPosition.z);
        }
        else
        {
            bobTimer = 0f;
            Vector3 targetPosition = new Vector3(cameraTransform.localPosition.x, defaultCameraY, cameraTransform.localPosition.z);

            cameraTransform.localPosition = Vector3.Lerp(cameraTransform.localPosition, targetPosition, Time.deltaTime * 10f);
        }
    }
}