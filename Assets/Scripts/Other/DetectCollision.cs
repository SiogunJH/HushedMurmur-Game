
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(BoxCollider))]
public class DetectCollision : MonoBehaviour
{
    public UnityEvent<Collider> OnTriggerEnterEvent;
    public UnityEvent<Collider> OnTriggerStayEvent;
    public UnityEvent<Collider> OnTriggerExitEvent;

    public HashSet<Transform> currentTriggers = new();

    void OnTriggerEnter(Collider other)
    {
        currentTriggers.Add(other.gameObject.transform);
        OnTriggerEnterEvent?.Invoke(other);
    }

    void OnTriggerStay(Collider other) => OnTriggerStayEvent?.Invoke(other);

    void OnTriggerExit(Collider other)
    {
        currentTriggers.Remove(other.gameObject.transform);
        OnTriggerExitEvent?.Invoke(other);
    }
}
