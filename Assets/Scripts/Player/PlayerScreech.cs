using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerScreech : MonoBehaviour
{
    [Header("Screech Settings")]
    [SerializeField]
    float screechDuration = 0.6f;
    [SerializeField]
    float screechCooldownDuration = 0.5f;
    float screechEffectAppearanceSpeed = 0.2f;
    float screechEffectDisappearanceSpeed = 1f;
    Vector3 screechEffectDisappearancePos = new Vector3(0, -0.5f, 0);

    [HideInInspector]
    public ScreechStatus currentScreechStatus = ScreechStatus.Available;

    [Header("Other")]
    [SerializeField]
    GameObject screechEffectVolume;
    [SerializeField]
    GameObject playerCamera;

    void Start()
    {
        if (screechEffectVolume == null) Debug.LogWarning($"Missing [Screech Effect Volume] reference in {gameObject.name}");
        if (playerCamera == null) Debug.LogWarning($"Missing [Player Camera] reference in {gameObject.name}");
    }


    public void OnScreech(InputAction.CallbackContext context)
    {
        // Skip multiple callouts
        if (!context.performed) return;

        // Do not allow for overlapping screeching
        if (currentScreechStatus != ScreechStatus.Available)
        {
            // Debug.Log("Screech is not available");
            return;
        }

        // Begin screeching
        OnScreechStart();
    }

    void OnScreechStart()
    {
        // Update screeching status
        currentScreechStatus = ScreechStatus.Performing;

        // Apply Screech Effect Volume effects
        MoveTowardsPoint mtp = screechEffectVolume.AddComponent<MoveTowardsPoint>();
        mtp.SetDestination(playerCamera.transform.localPosition, screechEffectAppearanceSpeed);

        // End screech after specified amount of time
        Invoke("OnScreechEnd", screechDuration);
    }

    void OnScreechEnd()
    {
        // Update screeching status
        currentScreechStatus = ScreechStatus.OnCooldown;

        // Remove Screech Effect Volume effects
        MoveTowardsPoint mtp = screechEffectVolume.AddComponent<MoveTowardsPoint>();
        mtp.SetDestination(screechEffectDisappearancePos, screechEffectDisappearanceSpeed);

        // End screech cooldown after specified amount of time
        Invoke("OnScreechCooldownEnd", screechCooldownDuration);
    }

    void OnScreechCooldownEnd()
    {
        // Update screeching status
        currentScreechStatus = ScreechStatus.Available;
    }

    public enum ScreechStatus
    {
        Forbidden,
        Available,
        Performing,
        OnCooldown,
    }
}
