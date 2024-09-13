using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FloatingEyes
{
    [RequireComponent(typeof(AudioSource))] // Attack Warning
    [RequireComponent(typeof(AudioSource))] // Footsteps
    public class Aggressive : Base
    {
        [Header("Settings (Aggressive)")]

        [Space, SerializeField] protected float forwardWalkingSpeed = 0.1f;
        [SerializeField] protected float forwardChargingSpeed = 2f;

        [Space, SerializeField] protected float attackProccingDistance = 24f;
        [SerializeField] protected float attackDelay = 15f;
        [SerializeField] protected float attackDelayOffset = 5f;
        [SerializeField] protected float attackRange = 4f;

        [SerializeField] protected AudioClip attackWarning;
        protected AudioSource attackWarningAS;

        [SerializeField] protected AudioClip attackFootstep;
        protected AudioSource attackFootstepsAS;

        protected override void Start()
        {
            var audioSources = GetComponents<AudioSource>();
            if (audioSources.Length != 2) Debug.LogError($"{gameObject.name} is expected to have exactly {2} AudioSource components, but instead has {audioSources.Length}!");
            else
            {
                attackWarningAS = audioSources[0];
                attackFootstepsAS = audioSources[1];
            }

            base.Start();
        }

        protected override IEnumerator InternalLogic(bool skipInitialInactivity = false)
        {
            if (!skipInitialInactivity) yield return new WaitForSeconds(initialInactivity + Random.Range(-initialInactivityOffset, initialInactivityOffset));
            PlayRevealAnimation();

            // Slowly approach cabin
            while (DistanceToPlayer() > attackProccingDistance)
            {
                MoveTowardsPlayer(forwardWalkingSpeed * Time.deltaTime);
                yield return null;
            }

            // Announce attack and wait
            PlayAttackWarningAudio();
            yield return new WaitForSeconds(attackDelay + Random.Range(-attackDelayOffset, attackDelayOffset));

            yield return Charge();
        }

        #region Sub-Logic

        protected IEnumerator Charge()
        {
            // Charge at the player
            PlayChargeAnimation();
            while (DistanceToPlayer() > attackRange)
            {
                MoveTowardsPlayer(forwardChargingSpeed * Time.deltaTime);
                yield return null;
            }

            // Attack
            AttackPlayer();
        }

        #endregion

        protected void PlayAttackWarningAudio()
        {
            const float defaultPitch = 1f;
            const float maxPitchOffset = 0.15f;

            attackWarningAS.clip = attackWarning;
            attackWarningAS.pitch = defaultPitch + Random.Range(-maxPitchOffset, maxPitchOffset);
            attackWarningAS.Play();
        }

        protected void PlayFootstepAudio()
        {
            const float defaultPitch = 0.85f;
            const float maxPitchOffset = 0.1f;

            attackFootstepsAS.clip = attackFootstep;
            attackFootstepsAS.pitch = defaultPitch + Random.Range(-maxPitchOffset, maxPitchOffset);
            attackFootstepsAS.Play();
        }

        protected override void OnDrawGizmosSelected()
        {
            base.OnDrawGizmosSelected();

            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(transform.position, attackProccingDistance);
        }
    }

}