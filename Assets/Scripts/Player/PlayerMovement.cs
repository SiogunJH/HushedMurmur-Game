using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMove : MonoBehaviour
{
    [Header("Movement Settings")]
    [Range(1, 10)]
    [Tooltip("Walking speed, in meters per second.")]
    [SerializeField]
    float walkSpeed = 1.2f;

    [Header("Gravity Settings")]
    [SerializeField]
    float gravity = -9.8f;
    [SerializeField]
    [Tooltip("If vertical velocity were to be set to zero, because of collisions, it's instead set to this value, to keep player adjusted to the ground level.")]
    float minNegativeVerticalVelocity = -2;

    [Header("Other")]
    [SerializeField]
    CharacterController controller;

    Vector2 movementInput;
    Vector3 velocity;
    CollisionFlags collisionInfo;

    public bool IsGrounded
    {
        get => (collisionInfo & CollisionFlags.Below) != 0;
    }

    // Update is called once per frame
    void Update()
    {
        // Reduce negative vertical velocity when grounded 
        if (IsGrounded && velocity.y < 0) velocity.y = minNegativeVerticalVelocity;

        // Handle gravity
        velocity.y += gravity * Time.deltaTime;

        // Handle movement input
        velocity.x = (transform.right * movementInput.x + transform.forward * movementInput.y).x * walkSpeed;
        velocity.z = (transform.right * movementInput.x + transform.forward * movementInput.y).z * walkSpeed;

        // Apply movement and get CollisionFlags
        collisionInfo = controller.Move(velocity * Time.deltaTime);
    }

    // On Movement event
    public void OnMovement(InputAction.CallbackContext context)
    {
        // Read value
        movementInput = context.ReadValue<Vector2>();
    }
}
