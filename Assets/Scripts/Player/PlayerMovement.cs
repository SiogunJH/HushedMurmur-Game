using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField, Range(1, 5), Tooltip("Walking speed, in meters per second.")]
    float walkSpeed = 2.5f;
    [SerializeField]
    public float walkSpeedModifier = 1;
    [SerializeField]
    public bool allowForMovement = true;

    [Header("Gravity Settings"),]
    [SerializeField]
    float gravity = -9.8f;
    [SerializeField, Tooltip("If vertical velocity were to be set to zero, because of collisions, it's instead set to this value, to keep player adjusted to the ground level.")]
    float minNegativeVerticalVelocity = -2;

    [Header("Other")]
    [SerializeField]
    CharacterController controller;
    [SerializeField]
    PlayerInteract playerInteract;

    Vector2 movementInput;
    Vector3 velocity;

    // Update is called once per frame
    void Update()
    {
        // Reduce negative vertical velocity when grounded 
        if (controller.isGrounded && velocity.y < 0) velocity.y = minNegativeVerticalVelocity;

        // Handle gravity
        velocity.y += gravity * Time.deltaTime;

        // Handle movement input
        if (allowForMovement)
        {
            velocity.x = (transform.right * movementInput.x + transform.forward * movementInput.y).x * walkSpeed * walkSpeedModifier;
            velocity.z = (transform.right * movementInput.x + transform.forward * movementInput.y).z * walkSpeed * walkSpeedModifier;
        }
        else
        {
            velocity.x = 0;
            velocity.z = 0;
        }


        // Apply movement
        controller.Move(velocity * Time.deltaTime);

        if (velocity.x != 0 || velocity.z != 0) playerInteract.RaycastForInteractable();
    }

    // On Movement event
    public void OnMovement(InputAction.CallbackContext context)
    {
        // Read value
        movementInput = context.ReadValue<Vector2>();
    }

    public void ToggleOnOff()
    {
        allowForMovement = !allowForMovement;
    }
}
