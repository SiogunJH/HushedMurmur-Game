using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerLook : MonoBehaviour
{
    [Header("Sensitivity Settings")]
    [SerializeField]
    float mouseXSensitivity = 100;
    [SerializeField]
    float mouseYSensitivity = 100;

    [Header("Other")]
    [SerializeField]
    Transform playerRoot;

    float xRotation = 0;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    // On LookAround event
    public void OnLookAround(InputAction.CallbackContext context)
    {
        // Read value
        Vector2 value = context.ReadValue<Vector2>();

        float mouseX = value.x * mouseXSensitivity * Time.deltaTime;
        float mouseY = value.y * mouseYSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90, 90);

        playerRoot.Rotate(Vector3.up * mouseX);
        transform.localRotation = Quaternion.Euler(xRotation, 0, 0);
    }
}
