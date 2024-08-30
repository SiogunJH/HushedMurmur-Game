using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace FloatingEyes
{
    public class Anomaly : Base
    {
        [Header("Settings (Anomaly)")]

        [Space, SerializeField] float attackDelay = 15f;
        [SerializeField] float attackDelayOffset = 5f;
        [SerializeField] float attackRange = 4f;

        protected override IEnumerator InternalLogic(bool skipInitialInactivity = false)
        {
            yield return base.InternalLogic(skipInitialInactivity);
        }
    }

}