using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace FloatingEyes
{
    public class Anomaly : Aggressive
    {
        [Header("Settings (Anomaly)")]
        [SerializeField] public Bird.Controller birdController;

        protected override IEnumerator InternalLogic(bool skipInitialInactivity = false)
        {
            if (!skipInitialInactivity) yield return new WaitForSeconds(initialInactivity + Random.Range(-initialInactivityOffset, initialInactivityOffset));
            PlayRevealAnimation();

            // Announce attack and wait
            PlayAttackWarningAudio();
            yield return new WaitForSeconds(attackDelay + Random.Range(-attackDelayOffset, attackDelayOffset));

            yield return Charge();
        }

        protected override void OnFullyHidden()
        {
            birdController.ScareAway();
            Destroy(gameObject);
        }

        protected override void ShriekReaction()
        {
            StopCoroutine(internalLogic);

            if (birdController.birdType == PlayerScreech.birdRepelled) PlayHideAnimation();
            else StartCoroutine(Charge());
        }
    }

}