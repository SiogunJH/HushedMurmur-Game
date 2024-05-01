using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerLook : MonoBehaviour
{
    [Header("Sensitivity Settings")]
    [SerializeField]
    float mouseXSensitivity = 20;
    [SerializeField]
    float mouseYSensitivity = 20;
    [SerializeField]
    public float mouseSensitivityModifier = 1;

    [Header("Shake Settings")]
    [SerializeField]
    float shakeIntensity = 0.1f;
    float shakeTimer = 0f;
    Vector3 originalPosition;

    [Header("Other")]
    [SerializeField]
    Transform playerRoot;

    float xRotation = 0;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        originalPosition = transform.localPosition;
    }

    void Update()
    {
        // Check if the shakeTimer is greater than 0 (shaking is active)
        if (shakeTimer > 0)
        {
            // Shake the camera by applying random values to the position
            Vector3 shakeOffset = (Vector3)Random.insideUnitCircle * shakeIntensity;
            shakeOffset = shakeOffset * shakeIntensity;
            transform.localPosition = originalPosition + shakeOffset;

            // Decrease the shake timer based on the elapsed time
            shakeTimer -= Time.deltaTime;

            // If the timer is less than or equal to 0, stop shaking
            if (shakeTimer <= 0) transform.localPosition = originalPosition;
        }
    }

    // Method to start the shake
    public void StartShake(float shakeDuration)
    {
        if (shakeDuration <= 0) Debug.LogWarning($"Shake duration must be a positive value! Value provided: {shakeDuration}");

        // Reset the shake timer
        shakeTimer = shakeDuration;
    }

    // On LookAround event
    public void OnLookAround(InputAction.CallbackContext context)
    {
        // Read value
        Vector2 value = context.ReadValue<Vector2>();

        float mouseX = value.x * mouseXSensitivity * mouseSensitivityModifier * Time.deltaTime;
        float mouseY = value.y * mouseYSensitivity * mouseSensitivityModifier * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90, 90);

        playerRoot.Rotate(Vector3.up * mouseX);
        transform.localRotation = Quaternion.Euler(xRotation, 0, 0);
    }
}
