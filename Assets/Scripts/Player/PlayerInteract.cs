using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteract : MonoBehaviour
{
    [SerializeField]
    GameObject playerCamera;

    Vector3 rayOrigin;
    Vector3 rayDirection;
    [SerializeField]
    LayerMask rayMask;
    [SerializeField]
    [Range(0.5f, 3.5f)]
    [Tooltip("In meters, maximum distance from which player can interact with objects.")]
    float rayLength = 1.5f;

    public GameObject RaycastForInteractable()
    {
        rayOrigin = playerCamera.transform.position;
        rayDirection = playerCamera.transform.forward;

        RaycastHit hit;
        if (Physics.Raycast(rayOrigin, rayDirection, out hit, rayLength)) return hit.collider.gameObject;
        else return null;
    }

    public void OnTriggerInteraction(InputAction.CallbackContext context)
    {
        if (!context.started) return;

        GameObject focusedObject = RaycastForInteractable();
        if (focusedObject == null) return;

        Interactable interaction = focusedObject.GetComponent<Interactable>();
        if (interaction == null) return;

        interaction.Interact();
    }
}
