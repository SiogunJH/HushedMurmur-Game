using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace FloatingEyes
{
    public class Passive : Base
    {
        [Header("Settings (Passive)")]

        [Space, SerializeField] protected float lifespan = 20f;
        [SerializeField] protected float lifespanOffset = 5f;

        protected override IEnumerator InternalLogic(bool skipInitialInactivity = false)
        {
            if (!skipInitialInactivity) yield return new WaitForSeconds(initialInactivity + Random.Range(-initialInactivityOffset, initialInactivityOffset));
            PlayRevealAnimation();

            yield return new WaitForSeconds(lifespan + Random.Range(-lifespanOffset, lifespanOffset));
            PlayHideAnimation();
        }
    }

}