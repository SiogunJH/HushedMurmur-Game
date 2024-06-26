using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerLook : MonoBehaviour
{
    [Header("Sensitivity Settings")]
    [SerializeField] float mouseXSensitivity = 20;
    [SerializeField] float mouseYSensitivity = 20;
    [SerializeField] public float mouseSensitivityModifier = 1;

    Vector3 originalPosition;

    [Header("Other"), SerializeField] Transform playerRoot;
    [SerializeField] PlayerInteract playerInteract;
    float xRotation = 0;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        originalPosition = transform.localPosition;

    }

    // Shake coroutine
    public IEnumerator ShakeCamera(float shakeDuration, float shakeMagnitude)
    {
        while (shakeDuration > 0)
        {
            // Shake the camera by applying random values to the position
            Vector3 shakeOffset = (Vector3)Random.insideUnitCircle * shakeMagnitude;
            shakeOffset = shakeOffset * shakeMagnitude;
            transform.localPosition = originalPosition + shakeOffset;

            // Decrease the shake timer based on the elapsed time
            shakeDuration -= Time.deltaTime;

            // If the timer is less than or equal to 0, stop shaking
            if (shakeDuration <= 0) transform.localPosition = originalPosition;

            // Execute once per frame
            yield return null;
        }
    }

    // On LookAround event
    public void OnLookAround(InputAction.CallbackContext context)
    {
        // Read value
        Vector2 value = context.ReadValue<Vector2>();

        float mouseX = value.x * mouseXSensitivity * mouseSensitivityModifier * Time.deltaTime;
        float mouseY = value.y * mouseYSensitivity * mouseSensitivityModifier * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -80, 80);

        playerRoot.Rotate(Vector3.up * mouseX);
        transform.localRotation = Quaternion.Euler(xRotation, 0, 0);

        // Search for interactibles
        playerInteract.RaycastForInteractable();
    }
}
