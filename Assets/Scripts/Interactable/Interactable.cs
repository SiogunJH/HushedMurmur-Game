using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour
{
    public UnityEvent OnInteract;
    public bool disabled = false;

    public void Interact()
    {
        if (!disabled) OnInteract?.Invoke();
    }
}
