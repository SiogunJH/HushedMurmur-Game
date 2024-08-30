using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace FloatingEyes
{
    public class Passive : Base
    {
        [Header("Settings (Passive)")]

        [Space, SerializeField] float lifespan = 20f;
        [SerializeField] float lifespanOffset = 5f;

        protected override IEnumerator InternalLogic(bool skipInitialInactivity = false)
        {
            yield return base.InternalLogic(skipInitialInactivity);

            yield return new WaitForSeconds(lifespan + Random.Range(-lifespanOffset, lifespanOffset));
            PlayHideAnimation();
        }
    }

}