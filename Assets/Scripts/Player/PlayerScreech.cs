using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerScreech : MonoBehaviour
{
    [SerializeField]
    float screechDuration = 0.6f;

    [SerializeField]
    GameObject screechEffectVP;

    void Start()
    {
        if (screechEffectVP == null) Debug.LogWarning($"Missing [Screech Effect VP] reference in {gameObject.name}");
    }


    public void OnScreech(InputAction.CallbackContext context)
    {
        if (context.started) OnScreechStart();
    }

    void OnScreechStart()
    {
        MoveTowardsPoint mtp = screechEffectVP.AddComponent<MoveTowardsPoint>();

        Vector3 destination = new Vector3(0, 0.5f, 0);
        float duration = 0.15f;

        mtp.SetDestination(destination, duration);

        Debug.Log("Screech started");
        Invoke("OnScreechEnd", screechDuration);
    }

    void OnScreechEnd()
    {
        MoveTowardsPoint mtp = screechEffectVP.AddComponent<MoveTowardsPoint>();

        Vector3 destination = new Vector3(0, -0.5f, 0);
        float duration = 1f;

        mtp.SetDestination(destination, duration);

        Debug.Log("Screech ended");
    }
}
