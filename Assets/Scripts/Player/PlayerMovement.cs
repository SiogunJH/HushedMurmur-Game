using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField]
    float walkSpeed = 12f;

    [Header("Gravity Settings")]
    [SerializeField]
    float gravity = -9.8f;
    [SerializeField]
    Transform groundCheck;
    [SerializeField]
    [Tooltip("Radius of the sphere that is generated to check if the player is grounded")]
    float groundDistance = 0.4f;
    [SerializeField]
    LayerMask groundMask;
    [SerializeField]
    [Tooltip("If vertical velocity were to be set to zero, because of collisions, it's instead set to this value, to keep player adjusted to the ground level.")]
    float minNegativeVerticalVelocity = -2;

    [Header("Other")]
    [SerializeField]
    CharacterController controller;

    Vector3 velocity;
    bool isGrounded;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        if (isGrounded && velocity.y < 0) velocity.y = minNegativeVerticalVelocity;

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;
        controller.Move(move * walkSpeed * Time.deltaTime);

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}
