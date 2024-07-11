using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerLook : MonoBehaviour
{
    [Header("Sensitivity Settings")]
    public static float mouseXSensitivity = 4;
    public static float mouseYSensitivity = 4;
    public static float mouseSensitivityModifier = 1;

    Vector3 originalPosition;

    [Header("Object Looked At Settings")]
    [SerializeField, Range(0.5f, 3.5f), Tooltip("In meters, maximum distance from which player can detect objects and interact with them.")]
    float maxObjectDetectionDistance = 1.5f;

    [Header("Other"), SerializeField] Transform playerRoot;
    float xRotation = 0;

    #region Singleton

    public static PlayerLook Instance { get; private set; }

    void Awake()
    {
        // Destroy self, if object of this class already exists
        if (Instance != null) Destroy(gameObject);

        //One time setup of a Singleton
        else Instance = this;
    }

    #endregion

    void OnEnable()
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

    public GameObject GetObjectLookedAt()
    {
        var rayOrigin = transform.position;
        var rayDirection = transform.forward;

        RaycastHit hit;
        if (Physics.Raycast(rayOrigin, rayDirection, out hit, maxObjectDetectionDistance)) return hit.collider.gameObject;
        else return null;
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
    }
}
