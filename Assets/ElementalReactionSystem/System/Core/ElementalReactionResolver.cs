using UnityEngine;

namespace ElementalReaction
{
    public static class ElementalReactionResolver
    {
        private const float EPSILON = 0.001f;

        public static ElementalReactionResult Resolve(ElementSystemConfig config, ElementalSeal currentSeal, ElementalHit hit)
        {
            var result = new ElementalReactionResult
            {
                elementHit = hit.element,
                reaction = ElementalReactionType.None,
                directDamage = hit.directDamage,
            };

            float eHitUnits = Mathf.Max(0f, hit.units); 
            var eHitProfile = config.GetProfile(hit.element); 

            // 1) No existing seal -> apply a new seal.
            if (currentSeal == null || !currentSeal.IsActive)
            {
                result.kind = InteractionKind.Applied;
                result.resultingSeal = new ElementalSeal(hit.element, Mathf.Min(eHitUnits, eHitProfile.maxUnits));
                result.consumedElementHitUnit = 0f;
                return result;
            }

            result.elementSeal = currentSeal.element;

            // 2) Same element -> stack (refresh).
            if (currentSeal.element == hit.element)
            {
                result.kind = InteractionKind.Refreshed;
                float newUnits = Mathf.Min(currentSeal.units + eHitUnits, eHitProfile.maxUnits);
                result.resultingSeal = new ElementalSeal(hit.element, newUnits);
                return result;
            }

            // 3) Different element -> look up the interaction table.
            Element eHit = hit.element;         // elementHit
            Element eSeal = currentSeal.element; // elementSeal
            float eSealUnits = currentSeal.units;

            var interaction = config.GetInteraction(eHit, eSeal);

            if (interaction != null && interaction.reaction != ElementalReactionType.None)
                return ResolveReaction(config, interaction, eHit, eSeal, eHitUnits, eSealUnits, ref result);

            return ResolveNeutral(config, interaction, eHit, eSeal, eHitUnits, eSealUnits, ref result);
        }

        private static ElementalReactionResult ResolveReaction(
            ElementSystemConfig config, ElementInteraction interaction,
            Element eHit, Element eSeal, float eHitUnits, float eSealUnits, ref ElementalReactionResult result)
        {
            float costEHit = Mathf.Max(EPSILON, interaction.CostOf(eHit));
            float costESeal = Mathf.Max(EPSILON, interaction.CostOf(eSeal));

            // Number of "reaction-units" is limited by the scarcer side per the ratio.
            float reactionUnits = Mathf.Min(eHitUnits / costEHit, eSealUnits / costESeal);
            reactionUnits = Mathf.Max(0f, reactionUnits);

            float consumedEHit = reactionUnits * costEHit;
            float consumedESeal = reactionUnits * costESeal;

            result.kind = InteractionKind.Reacted;
            result.reaction = interaction.reaction;
            result.reactionDamage = reactionUnits * interaction.reactionDamagePerUnit;
            result.effectDuration = interaction.effectDuration;
            result.effectMagnitude = interaction.effectMagnitude;
            result.consumedElementHitUnit = consumedEHit;
            result.consumedElementSealUnit = consumedESeal;

            float remainingESeal = eSealUnits - consumedESeal;
            float remainingEHit = eHitUnits - consumedEHit;

            if (remainingESeal > EPSILON)
            {
                // Existing seal survives -> keep it; excess incoming is absorbed by the reaction.
                result.resultingSeal = new ElementalSeal(eSeal, remainingESeal);
            }
            else if (remainingEHit > EPSILON)
            {
                // Existing seal depleted -> leftover incoming forms a new seal.
                var inProfile = config.GetProfile(eHit);
                result.resultingSeal = new ElementalSeal(eHit, Mathf.Min(remainingEHit, inProfile.maxUnits));
            }
            else
            {
                result.resultingSeal = null; // no seal left
            }

            return result;
        }

        private static ElementalReactionResult ResolveNeutral(
            ElementSystemConfig config, ElementInteraction interaction,
            Element eHit, Element eSeal, float eHitUnits, float eSealUnits, ref ElementalReactionResult result)
        {
            result.kind = InteractionKind.Resisted;
            result.reaction = ElementalReactionType.None;

            // Safe defaults if the neutral pair was not configured.
            float blockRatio = interaction != null ? Mathf.Max(EPSILON, interaction.blockRatio) : 1f;
            float damageReduction = interaction != null ? interaction.damageReduction : 0.5f;

            // How many incoming units the existing seal can block at most.
            float blockableEHit = eSealUnits / blockRatio;
            float blockedEHit = Mathf.Min(eHitUnits, blockableEHit);
            float consumedESeal = blockedEHit * blockRatio;
            float passThroughIn = eHitUnits - blockedEHit;

            float blockFraction = eHitUnits > EPSILON ? blockedEHit / eHitUnits : 0f;
            result.directDamage = result.directDamage * (1f - damageReduction * blockFraction);
            result.consumedElementSealUnit = consumedESeal;
            result.consumedElementHitUnit = 0f;

            float remainingESeal = eSealUnits - consumedESeal;

            if (remainingESeal > EPSILON)
            {
                // Existing seal survives -> keep it; the hit is blocked and does not stick.
                result.resultingSeal = new ElementalSeal(eSeal, remainingESeal);
            }
            else if (passThroughIn > EPSILON)
            {
                // Existing seal broken -> the leftover part of the hit applies as a new seal.
                var inProfile = config.GetProfile(eHit);
                result.resultingSeal = new ElementalSeal(eHit, Mathf.Min(passThroughIn, inProfile.maxUnits));
                result.consumedElementHitUnit = blockedEHit;
            }
            else
            {
                result.resultingSeal = null;
                result.consumedElementHitUnit = blockedEHit;
            }

            return result;
        }
    }
}
