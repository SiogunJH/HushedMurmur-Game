using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteract : MonoBehaviour
{
    [SerializeField]
    GameObject playerCamera;

    GameObject focusedObject = null;

    Vector3 rayOrigin;
    Vector3 rayDirection;
    [SerializeField]
    LayerMask rayMask;
    [SerializeField]
    [Range(0.5f, 3.5f)]
    [Tooltip("In meters, maximum distance from which player can interact with objects.")]
    float rayLength = 2.0f;

    public void RaycastForInteractable()
    {
        rayOrigin = playerCamera.transform.position;
        rayDirection = playerCamera.transform.forward;

        RaycastHit hit;
        if (Physics.Raycast(rayOrigin, rayDirection, out hit, rayLength, rayMask)) // raycast hit
        {
            if (hit.collider.gameObject != focusedObject) // hit new object
            {
                // going strainght from one interactible to another
                if (focusedObject != null) UnfocusObject();

                // focus an object that was hit
                focusedObject = hit.collider.gameObject;
                FocusObject();
            }
        }
        else if (focusedObject != null) // raycast miss, but an old object is still focused
        {
            UnfocusObject();
            focusedObject = null;
        }
    }

    void FocusObject()
    {
        InteractableBase interactable = focusedObject.GetComponent<InteractableBase>();
        if (interactable != null) interactable.Focus();
        else Debug.LogError($"[{focusedObject.name}] is a member of Interactable layer but is missing an [InteractableBase] script");
    }

    void UnfocusObject()
    {
        InteractableBase interactable = focusedObject.GetComponent<InteractableBase>();
        if (interactable != null) interactable.Unfocus();
        else Debug.LogError($"[{focusedObject.name}] is a member of Interactable layer but is missing an [InteractableBase] script");
    }

    public void OnTriggerInteraction(InputAction.CallbackContext context)
    {
        if (!context.started) return;

        if (focusedObject == null) return;

        InteractableBase interaction = focusedObject.GetComponent<InteractableBase>();
        if (interaction == null)
        {
            Debug.LogError($"[{focusedObject.name}] was called to trigger its interaction, but is missing [InteractableBase] script");
            return;
        }

        interaction.Interact();
    }
}
