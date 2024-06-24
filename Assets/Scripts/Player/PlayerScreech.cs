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
    [SerializeField]
    float screechMovementModifier = 0.2f;
    [SerializeField]
    float screechMouseSensitivityModifier = 0.2f;
    [SerializeField]
    float screechShakeMagnitude = 0.1f;

    float screechEffectAppearanceSpeed = 0.2f;
    float screechEffectDisappearanceSpeed = 1f;
    Vector3 screechEffectDisappearancePos = new(0, -0.5f, 0);
    Coroutine screechShakeCoroutine;

    [SerializeField]
    public ScreechStatus currentScreechStatus = ScreechStatus.Available;

    [Header("Other")]
    [SerializeField]
    GameObject screechEffectVolume;
    [SerializeField]
    AudioSource screechAudioSource;
    [SerializeField]
    AudioClip[] screechAudio;

    [Space]
    [SerializeField]
    PlayerLook playerLook;
    [SerializeField]
    Transform playerCameraHolder;

    [Space]
    [SerializeField]
    PlayerMovement playerMovement;

    void Start()
    {
        if (screechEffectVolume == null) Debug.LogWarning($"Missing [Screech Effect Volume] reference in {gameObject.name}");
        if (screechAudioSource == null) Debug.LogWarning($"Missing [Screech Audio Source] reference in {gameObject.name}");
        if (playerLook == null) Debug.LogWarning($"Missing [Player Look] reference in {gameObject.name}");
        if (playerMovement == null) Debug.LogWarning($"Missing [Player Movement] reference in {gameObject.name}");
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

        // Slow down player movement and mouse sensitivity
        playerMovement.walkSpeedModifier = screechMovementModifier;
        playerLook.mouseSensitivityModifier = screechMouseSensitivityModifier;

        // Apply Screech Effect Volume effects
        MoveTowardsPoint mtp = screechEffectVolume.AddComponent<MoveTowardsPoint>();
        mtp.SetDestination(playerCameraHolder.localPosition, screechEffectAppearanceSpeed);
        screechShakeCoroutine = StartCoroutine(playerLook.ShakeCamera(screechDuration, screechShakeMagnitude));

        // Play screech sound
        screechAudioSource.SetClipFromPool(screechAudio);
        screechAudioSource.Play();

        // End screech after specified amount of time
        Invoke("OnScreechEnd", screechDuration);
    }

    void OnScreechEnd()
    {
        // Update screeching status
        currentScreechStatus = ScreechStatus.OnCooldown;

        // Revert player movement speed and mouse sensitivity
        playerMovement.walkSpeedModifier = 1;
        playerLook.mouseSensitivityModifier = 1;

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
