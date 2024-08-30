using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FloatingEyes
{
    [RequireComponent(typeof(AudioSource))] // Attack Warning
    [RequireComponent(typeof(AudioSource))] // Footsteps
    public class Aggresive : Base
    {
        [Header("Settings (Aggresive)")]

        [Space, SerializeField] float forwardWalkingSpeed = 0.1f;
        [SerializeField] float forwardChargingSpeed = 2f;

        [Space, SerializeField] float attackProccingDistance = 24f;
        [SerializeField] float attackDelay = 15f;
        [SerializeField] float attackDelayOffset = 5f;
        [SerializeField] float attackRange = 4f;

        [SerializeField] AudioClip attackWarning;
        AudioSource attackWarningAS;

        [SerializeField] AudioClip attackFootstep;
        AudioSource attackFootstepsAS;

        protected override void Start()
        {
            base.Start();

            var audioSources = GetComponents<AudioSource>();
            if (audioSources.Length != 2) Debug.LogError($"{gameObject.name} is expected to have exactly {2} AudioSource components, but instead has {audioSources.Length}!");
            else
            {
                attackWarningAS = audioSources[0];
                attackFootstepsAS = audioSources[1];
            }
        }

        protected override IEnumerator InternalLogic(bool skipInitialInactivity = false)
        {
            yield return base.InternalLogic(skipInitialInactivity);

            // Slowly approach cabin
            while (DistanceToPlayer() > attackProccingDistance)
            {
                MoveTowardsPlayer(forwardWalkingSpeed * Time.deltaTime);
                yield return null;
            }

            // Announce attack and wait
            PlayAttackWarningAudio();
            yield return new WaitForSeconds(attackDelay + Random.Range(-attackDelayOffset, attackDelayOffset));

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

        #region Player Related Stuff

        #endregion

        protected override void OnDrawGizmosSelected()
        {
            base.OnDrawGizmosSelected();

            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(transform.position, attackProccingDistance);
        }
    }

}