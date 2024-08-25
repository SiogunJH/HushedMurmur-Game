using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.Events;

public class FarmBirdkin : MonoBehaviour, IShriekable
{
    [Header("Settings")]

    [SerializeField] float forwardWalkingSpeed = 0.1f;
    [SerializeField] float forwardChargingSpeed = 2f;

    [Space, SerializeField] float attackDistance = 4f;
    [SerializeField] float attackDelay = 20f;
    [SerializeField] float attackDelayOffset = 2f;

    [Space, SerializeField] float stunDuration = 10f;
    bool isWithinChargeDistance = false;

    Coroutine internalLogic;

    [Header("References")]
    [SerializeField] BoxCollider chargingDistanceCollider;

    [Header("Children References")]
    [SerializeField] Animator animator;
    const string ANIMATOR_TRIGGER_SHRIEKED_AT = "ShriekedAt";
    const string ANIMATOR_TRIGGER_START_RUNNING = "StartRunning";
    const string ANIMATOR_TRIGGER_WAKE_UP = "WakeUp";

    [Header("Debug")]
    [SerializeField] bool logDebugInfo = false;

    void Start()
    {
        OnShriekedAt.AddListener(ShriekReaction);
        internalLogic = StartCoroutine(InternalLogic());
    }

    void Update()
    {
        LookAtPlayer();
    }

    IEnumerator InternalLogic()
    {
        // Init
        yield return new WaitForSeconds(stunDuration);

        if (spawnPoints == null || spawnPoints.Length == 0) { Debug.LogError("No spawn points available"); yield break; }
        transform.position = spawnPoints[Random.Range(0, spawnPoints.Length)].position;
        animator.SetTrigger(ANIMATOR_TRIGGER_WAKE_UP);

        // Slowly approach cabin
        while (!isWithinChargeDistance)
        {
            MoveTowardsPlayer(forwardWalkingSpeed * Time.deltaTime);
            yield return null;
        }

        // Wait for a short duration
        yield return new WaitForSeconds(attackDelay + Random.Range(-attackDelayOffset, attackDelay));

        // Charge at the player
        animator.SetTrigger(ANIMATOR_TRIGGER_START_RUNNING);
        while (DistanceToPlayer() > attackDistance)
        {
            MoveTowardsPlayer(forwardChargingSpeed * Time.deltaTime);
            yield return null;
        }

        // Attack
        AttackPlayer();
    }

    #region IShriekable

    [SerializeField] Transform[] spawnPoints;
    public UnityEvent OnShriekedAt { get; private set; } = new();

    public void ShriekReaction()
    {
        if (logDebugInfo) Debug.Log($"{gameObject.name} was shrieked at!");

        animator.SetTrigger(ANIMATOR_TRIGGER_SHRIEKED_AT);

        StopCoroutine(internalLogic);

        internalLogic = StartCoroutine(InternalLogic());
    }

    #endregion

    #region Collisions & Triggers

    void OnTriggerEnter(Collider other)
    {
        if (other.transform == chargingDistanceCollider.transform)
        {
            isWithinChargeDistance = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.transform == chargingDistanceCollider.transform)
        {
            isWithinChargeDistance = false;
        }
    }

    #endregion

    #region Player Related Stuff

    float DistanceToPlayer() => (PlayerLook.Instance.gameObject.transform.position - gameObject.transform.position).magnitude;

    void LookAtPlayer()
    {
        // Get the direction from the head to the player, but ignore the y-axis
        Vector3 direction = PlayerLook.Instance.gameObject.transform.position - gameObject.transform.position;
        direction.y = 0; // Ignore the y-axis

        // Check if the direction is not zero to avoid errors
        if (direction != Vector3.zero)
        {
            // Create the rotation to look in that direction
            Quaternion rotation = Quaternion.LookRotation(direction);

            // Apply the rotation to the head, maintaining the original X rotation
            gameObject.transform.rotation = Quaternion.Euler(gameObject.transform.rotation.eulerAngles.x, rotation.eulerAngles.y, gameObject.transform.rotation.eulerAngles.z);
        }
    }

    void MoveTowardsPlayer(float distance)
    {
        // Get the direction from the current position to the player's position
        Vector3 direction = PlayerMovement.Instance.gameObject.transform.position - transform.position;
        direction.y = 0; // avoid moving down
        direction.Normalize();

        transform.position = transform.position + direction * distance;
    }

    void AttackPlayer()
    {
        Debug.Log($"{gameObject.name} is attacking Player!");
    }

    #endregion

}
