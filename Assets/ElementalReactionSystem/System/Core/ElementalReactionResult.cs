using UnityEngine;

namespace ElementalReaction
{
    public struct ElementalReactionResult
    {
        public InteractionKind kind;
        public Element elementHit;
        public Element elementSeal;
        public ElementalReactionType reaction;

        // Direct damage (after resist, if any)
        public float directDamage;
        // Damage dealt by the reaction 
        public float reactionDamage;
        // ElementHit units that were consumed
        public float consumedElementHitUnit;
        // ElementSeal units that were consumed
        public float consumedElementSealUnit;

        public float effectDuration;
        public float effectMagnitude;

        // remaining after resolving (null if no seal left)
        public ElementalSeal resultingSeal;

        public float TotalDamage => directDamage + reactionDamage;
    }
}
