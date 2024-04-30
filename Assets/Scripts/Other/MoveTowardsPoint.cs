using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveTowardsPoint : MonoBehaviour
{
    Vector3 targetPoint; // The point to move towards
    float duration = 2.0f; // Time in seconds to reach the target point

    Vector3 startPoint; // The starting point of the movement
    float elapsedTime = 0.0f; // Time elapsed since movement started

    void Start()
    {
        RemoveMultipleInstancesOfSelf();

        // Initialize the starting point
        startPoint = transform.localPosition;
    }

    public void SetDestination(Vector3 targetPoint, float duration)
    {
        this.targetPoint = targetPoint;
        this.duration = duration;
    }

    public void RemoveMultipleInstancesOfSelf()
    {
        // Get all components of the same type in the GameObject
        Component[] duplicateComponents = GetComponents(GetType());

        // Loop through all components of the same type
        for (int i = 0; i < duplicateComponents.Length; i++)
            if (duplicateComponents[i] != this) DestroyImmediate(duplicateComponents[i]);
    }

    void Update()
    {
        // Increment elapsed time
        elapsedTime += Time.deltaTime;

        // Calculate the normalized time between 0 and 1
        float t = elapsedTime / duration;

        // Apply damping using Mathf.SmoothStep for smooth movement
        //float smoothTime = Mathf.Lerp(0, 1, Mathf.Pow(t, 1 / damping));
        float smoothTime = Mathf.SmoothStep(0, 1, t);

        // Move the GameObject towards the target point
        transform.localPosition = Vector3.Lerp(startPoint, targetPoint, smoothTime);

        // Check if the GameObject has reached the target point, and if so - delete this component
        if (t >= 1.0f) Destroy(this);
    }
}
