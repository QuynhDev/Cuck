using ElementalReaction;
using System;
using UnityEngine;

namespace ElementalReaction
{
    // Interaction between two elements (order-independent)
    // Reaction == None means a neutral pair (resist)
    [Serializable]
    public class ElementInteraction
    {
        public Element elementA;
        public Element elementB;
        public ElementalReactionType reaction = ElementalReactionType.None;

        [Header("Consumption ratio (units / 1 reaction-unit)")]
        [Tooltip("Units of elementA consumed per 'reaction-unit'.")]
        public float costA = 1f;
        [Tooltip("Units of elementB consumed per 'reaction-unit'.")]
        public float costB = 1f;

        [Header("Reaction (when reaction != None)")]
        [Tooltip("Reaction damage per 'reaction-unit'.")]
        public float reactionDamagePerUnit = 12f;
        [Tooltip("Effect duration (slow/reflect/secondary DoT...).")]
        public float effectDuration = 3f;
        [Tooltip("Effect magnitude (e.g. slow 0.5 = -50% speed, reflect 0.5 = 50% dmg).")]
        public float effectMagnitude = 0.5f;

        [Header("Neutral (when reaction == None) - resist")]
        [Tooltip("Existing-seal units consumed to block 1 incoming elemental unit.")]
        public float blockRatio = 1f;
        [Tooltip("Max direct-damage reduction (0..1) when the existing seal blocks fully.")]
        [Range(0f, 1f)] public float damageReduction = 0.5f;

        // True if the pair {a,b} matches this interaction (in any order)
        public bool Matches(Element a, Element b)
        {
            return (elementA == a && elementB == b) || (elementA == b && elementB == a);
        }

        // Consumption cost for a specific element within the pair
        public float CostOf(Element e)
        {
            if (e == elementA) return costA;
            if (e == elementB) return costB;
            return 1f;
        }
    }
}
