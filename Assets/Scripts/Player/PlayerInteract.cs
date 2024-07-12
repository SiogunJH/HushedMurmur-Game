using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteract : MonoBehaviour
{
    public void OnTriggerInteraction(InputAction.CallbackContext context)
    {
        if (!context.started) return;

        GameObject focusedObject = PlayerLook.Instance.GetObjectLookedAt();
        if (focusedObject == null) return;

        Interactable interaction = focusedObject.GetComponent<Interactable>();
        if (interaction == null) return;

        interaction.Interact();
    }
}
