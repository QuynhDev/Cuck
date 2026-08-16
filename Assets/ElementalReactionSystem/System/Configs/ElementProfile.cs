using System;
using UnityEngine;

namespace ElementalReaction
{
    [Serializable]
    public class ElementProfile
    {
        public Element element;
        [Tooltip("Maximum elemental units that can stack on the target.")]
        public float maxUnits = 10f;
        [Tooltip("Damage dealt per 1 unit consumed by the DoT.")]
        public float damagePerUnit = 4f; // Remove
        [Tooltip("Seconds between each DoT tick.")]
        public float tickInterval = 0.5f;
        [Tooltip("Units consumed on each DoT tick.")]
        public float consumePerTick = 1f;
    }
}
