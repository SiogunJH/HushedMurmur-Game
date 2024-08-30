using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.Events;

namespace FloatingEyes
{
    [RequireComponent(typeof(Animator))]
    public abstract class Base : MonoBehaviour, IShriekable
    {
        [Header("Debug")]
        [SerializeField] bool logDebugInfo = false;

        [Header("Settings (General)")]
        [Space, SerializeField] protected float maxOffsetXZ = 4f;
        [SerializeField] protected float maxOffsetY = 0.5f;
        protected Vector3 initialPosition;

        [Space, SerializeField] protected float minScaleModifier = 0.6f;
        [SerializeField] protected float maxScaleModifier = 1.8f;
        protected Vector3 initialScale;

        [Space, SerializeField] protected float initialInactivity = 15f;
        [SerializeField] protected float initialInactivityOffset = 5f;

        //[Header("Children References")]
        protected Animator animator;
        protected const string ANIMATOR_TRIGGER_HIDE = "FadeOut";
        protected const string ANIMATOR_TRIGGER_CHARGE = "Charge";
        protected const string ANIMATOR_TRIGGER_REVEAL = "FadeIn";

        protected virtual void Start()
        {
            if (animator == null) animator = GetComponent<Animator>();

            OnShriekedAt.AddListener(ShriekReaction);
            internalLogic = StartCoroutine(InternalLogic(true));
            initialPosition = transform.position;
            initialScale = transform.localScale;
        }

        void Update()
        {
            LookAtPlayer();
        }

        #region AI

        protected Coroutine internalLogic;

        protected virtual IEnumerator InternalLogic(bool skipInitialInactivity = false)
        {
            if (!skipInitialInactivity) yield return new WaitForSeconds(initialInactivity + Random.Range(-initialInactivityOffset, initialInactivityOffset));
            PlayRevealAnimation();
        }

        protected void ResetInternalLogic()
        {
            StopCoroutine(internalLogic);
            internalLogic = StartCoroutine(InternalLogic());
        }

        #endregion

        #region Relocation Logic

        protected Vector3 GetNewPosition()
        {
            Vector2 randomOffset = Random.insideUnitCircle * maxOffsetXZ;
            return new Vector3(initialPosition.x + randomOffset.x, initialPosition.y + Random.Range(-maxOffsetY, maxOffsetY), initialPosition.z + randomOffset.y);
        }

        protected Vector3 GetNewScale()
        {
            return initialScale * Random.Range(minScaleModifier, maxScaleModifier);
        }

        protected void Relocate()
        {
            transform.position = GetNewPosition();
            transform.localScale = GetNewScale();
        }

        #endregion

        #region Animations

        protected void PlayChargeAnimation()
        {
            animator.SetTrigger(ANIMATOR_TRIGGER_CHARGE);
        }

        protected void PlayHideAnimation()
        {
            animator.ResetTrigger(ANIMATOR_TRIGGER_REVEAL);
            animator.SetTrigger(ANIMATOR_TRIGGER_HIDE);
        }

        protected virtual void OnFullyHidden()
        {
            Relocate();
            ResetInternalLogic();
        }

        protected void PlayRevealAnimation()
        {
            animator.SetTrigger(ANIMATOR_TRIGGER_REVEAL);
        }

        #endregion

        #region IShriekable

        public UnityEvent OnShriekedAt { get; private set; } = new();

        protected virtual void ShriekReaction()
        {
            ResetInternalLogic();
            PlayHideAnimation();
        }

        #endregion

        #region Player Related Stuff

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

        protected float DistanceToPlayer() => (PlayerLook.Instance.gameObject.transform.position - gameObject.transform.position).magnitude;

        protected void MoveTowardsPlayer(float distance)
        {
            // Get the direction from the current position to the player's position
            Vector3 direction = PlayerMovement.Instance.gameObject.transform.position - transform.position;
            direction.y = 0; // avoid moving down
            direction.Normalize();

            transform.position = transform.position + direction * distance;
        }

        protected virtual void AttackPlayer()
        {
            if (logDebugInfo) Debug.Log($"{gameObject.name} is attacking Player!");
        }

        #endregion

        protected virtual void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;

            if (Application.isPlaying) Gizmos.DrawWireSphere(initialPosition, maxOffsetXZ);
            else Gizmos.DrawWireSphere(transform.position, maxOffsetXZ);
        }
    }
}
