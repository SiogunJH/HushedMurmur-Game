using System.Collections;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PlayerScreech : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] ScreechStatus status = ScreechStatus.Available;

    [Space, SerializeField] float chargeDuration = 0.8f;
    [SerializeField] float chargeMovementModifier = 0.2f;
    [SerializeField] float chargeMouseSensitivityModifier = 0.2f;
    [SerializeField] AudioClip[] chargeAudio;
    [SerializeField] AudioClip[] chargeCancelAudio;
    Coroutine chargeCoroutine;

    [Space, SerializeField] float screechOnsetDuration = 0.1f; // should be lesser than screechDuration
    [Space, SerializeField] float screechDuration = 0.6f;
    [SerializeField] float screechShakeMagnitude = 0.15f;
    [SerializeField] AudioClip[] screechAudio;

    [Space, SerializeField] float fullRechargeDuration = 1f;
    [SerializeField] float shortRechargeDuration = 0.4f;

    // [Header("Visuals")]
    (float Old, float Default, float High) chromaticAberrationIntensity = (Old: 0f, Default: 0f, High: 0.3f);
    (float Old, float Default, float Low, float High) lensDistortionIntensity = (Old: 0f, Default: 0f, Low: 0.2f, High: 0.4f);

    // [Header("Repellant")]
    public static Bird.Type? birdRepelled = null;

    [Header("References")]
    [SerializeField] AudioSource audioSource;
    [SerializeField] VolumeProfile screechEffectVolumeProfile;

    void Start()
    {
        // Check if all references are present
        if (screechEffectVolumeProfile == null) Debug.LogWarning($"Missing [Screech Effect Volume] reference in {gameObject.name}");
        if (audioSource == null) Debug.LogWarning($"Missing [Audio Source] reference in {gameObject.name}");

        // Reset Volume Profile overrides
        screechEffectVolumeProfile.TryGet(out LensDistortion lensDistortion);
        screechEffectVolumeProfile.TryGet(out ChromaticAberration chromaticAberration);

        lensDistortion.intensity.value = lensDistortionIntensity.Default;
        chromaticAberration.intensity.value = chromaticAberrationIntensity.Default;
    }


    public void OnScreech(InputAction.CallbackContext context)
    {
        // Start screeching
        if (context.started)
        {
            if (status != ScreechStatus.Available) return;

            // Begin charging the screech
            ScreechInputStart();
        }
        else if (context.canceled)
        {
            ScreechInputEnd();
        }
    }

    void ScreechInputStart()
    {
        status = ScreechStatus.Charging;
        chargeCoroutine = StartCoroutine(ScreechCharge());
    }

    IEnumerator ScreechCharge()
    {
        // Play charging audio
        audioSource.SetClipFromPool(chargeAudio);
        audioSource.Play();

        // Get screech volume effects references
        screechEffectVolumeProfile.TryGet(out LensDistortion lensDistortion);

        // Await end of charge
        float chargeProgressInPercent;
        float chargeDurationRemaining = chargeDuration;
        while (chargeDurationRemaining > 0)
        {
            // Update progress variables
            chargeDurationRemaining -= Time.deltaTime;
            chargeProgressInPercent = Mathf.Clamp(1 - (chargeDurationRemaining / chargeDuration), 0, 1);

            // Update visuals
            lensDistortion.intensity.value = lensDistortionIntensity.Low * chargeProgressInPercent;

            // Slow down player movement and mouse sensitivity over time
            PlayerMovement.walkSpeedModifier = 1 - (1 - chargeMovementModifier) * chargeProgressInPercent;
            PlayerLook.mouseSensitivityModifier = 1 - (1 - chargeMouseSensitivityModifier) * chargeProgressInPercent;

            // Execute once per frame
            yield return null;
        }

        // Adjust

        // Update status
        status = ScreechStatus.Prepared;
    }

    void ScreechInputEnd()
    {
        if (status == ScreechStatus.Charging) ScreechCancel();
        else if (status == ScreechStatus.Prepared) StartCoroutine(ScreechPerform());
    }

    void ScreechCancel()
    {
        // Stop charging
        StopCoroutine(chargeCoroutine);

        // Play charging audio
        audioSource.Stop();
        audioSource.SetClipFromPool(chargeCancelAudio);
        audioSource.Play();

        // Go on cooldown
        StartCoroutine(ScreechRecharge(shortRechargeDuration));
    }

    IEnumerator ScreechPerform()
    {
        // Update status
        status = ScreechStatus.Performing;

        // Play screech audio
        audioSource.SetClipFromPool(screechAudio);
        audioSource.Play();

        // Attack
        ScreechAttack();

        // Shake camera
        StartCoroutine(PlayerLook.Instance.ShakeCamera(screechDuration, screechShakeMagnitude));

        // Apply screech volume effects
        screechEffectVolumeProfile.TryGet(out LensDistortion lensDistortion);
        screechEffectVolumeProfile.TryGet(out ChromaticAberration chromaticAberration);

        // Onset screech effects
        float screechProgressInPercent;
        float screechDurationRemaining = screechOnsetDuration;
        while (screechDurationRemaining > 0)
        {
            // Update progress variables
            screechDurationRemaining -= Time.deltaTime;
            screechProgressInPercent = Mathf.Clamp(1 - (screechDurationRemaining / screechOnsetDuration), 0, 1);

            // Update visuals
            lensDistortion.intensity.value = lensDistortionIntensity.Low + (lensDistortionIntensity.High - lensDistortionIntensity.Low) * screechProgressInPercent;
            chromaticAberration.intensity.value = chromaticAberrationIntensity.High * screechProgressInPercent;

            // Execute once per frame
            yield return null;
        }

        screechDurationRemaining = screechDuration - screechOnsetDuration;
        while (screechDurationRemaining > 0)
        {
            // Update progress variables
            screechDurationRemaining -= Time.deltaTime;

            // Execute once per frame
            yield return null;
        }

        // Go on cooldown
        StartCoroutine(ScreechRecharge(fullRechargeDuration));
    }

    IEnumerator ScreechRecharge(float rechargeDuration)
    {
        // Revert player movement speed and mouse sensitivity
        PlayerMovement.walkSpeedModifier = 1;
        PlayerLook.mouseSensitivityModifier = 1;

        // Apply screech volume effects
        screechEffectVolumeProfile.TryGet(out LensDistortion lensDistortion);
        screechEffectVolumeProfile.TryGet(out ChromaticAberration chromaticAberration);

        // Backup visuals
        lensDistortionIntensity.Old = lensDistortion.intensity.value;
        chromaticAberrationIntensity.Old = chromaticAberration.intensity.value;

        // Await end of recharge
        float rechargeProgressInPercent;
        float rechargeDurationRemaining = rechargeDuration;
        while (rechargeDurationRemaining > 0)
        {
            // Update progress variables
            rechargeDurationRemaining -= Time.deltaTime;
            rechargeProgressInPercent = Mathf.Clamp(1 - (rechargeDurationRemaining / rechargeDuration), 0, 1);

            // Update visuals
            lensDistortion.intensity.value = lensDistortionIntensity.Old + (lensDistortionIntensity.Default - lensDistortionIntensity.Old) * rechargeProgressInPercent;
            chromaticAberration.intensity.value = chromaticAberrationIntensity.Old + (chromaticAberrationIntensity.Default - chromaticAberrationIntensity.Old) * rechargeProgressInPercent;

            // Execute once per frame
            yield return null;
        }

        // Update status
        status = ScreechStatus.Available;
    }

    void ScreechAttack()
    {
        if (birdRepelled == null) return;

        Ray ray = new Ray(PlayerLook.Instance.transform.position, PlayerLook.Instance.transform.forward);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 10))
        {
            var corridorEntrance = hit.collider.gameObject.GetComponent<CorridorEntrance>();
            if (corridorEntrance == null) return;

            var vulnerableOccupants = corridorEntrance.connectedRoom.GetOccupants().Where(bird => bird.birdType == birdRepelled).ToList();
            if (vulnerableOccupants.Count != 0)
                foreach (var occupant in vulnerableOccupants)
                    occupant.ScareAway();
        }
        birdRepelled = null;
    }

    enum ScreechStatus
    {
        Unavailable,
        Available,
        Charging,
        Prepared,
        Performing,
        Recharging,
    }
}
