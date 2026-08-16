using System.Collections.Generic;
using UnityEngine;

namespace ElementalReaction
{
    /// <summary>
    /// Central configuration of the elemental system: per-element DoT + the 10-pair interaction table.
    /// Create asset: Create > Mayker > Elemental > System Config, then press "Populate Defaults" in the Inspector.
    /// </summary>
    [CreateAssetMenu(menuName = "ElementalReaction/System Config")]
    public class ElementSystemConfig : ScriptableObject
    {
        [Tooltip("DoT configuration for each element (should contain all 5).")]
        public List<ElementProfile> profiles = new List<ElementProfile>();

        [Tooltip("Interaction table for the 10 element pairs.")]
        public List<ElementInteraction> interactions = new List<ElementInteraction>();

        private Dictionary<Element, ElementProfile> _profileLookup;

        /// <summary>Get the DoT profile of an element (returns a default if not configured).</summary>
        public ElementProfile GetProfile(Element e)
        {
            if (_profileLookup == null || _profileLookup.Count != profiles.Count)
                RebuildLookup();

            if (_profileLookup.TryGetValue(e, out var p))
                return p;

            // safe fallback
            var fallback = new ElementProfile { element = e };
            _profileLookup[e] = fallback;
            return fallback;
        }

        /// <summary>Get the interaction for pair {a,b} (null if not found).</summary>
        public ElementInteraction GetInteraction(Element a, Element b)
        {
            for (int i = 0; i < interactions.Count; i++)
            {
                if (interactions[i].Matches(a, b))
                    return interactions[i];
            }
            return null;
        }

        private void RebuildLookup()
        {
            _profileLookup = new Dictionary<Element, ElementProfile>();
            foreach (var p in profiles)
                _profileLookup[p.element] = p;
        }

        private void OnEnable() => _profileLookup = null;

        [ContextMenu("Populate Defaults")]
        public void PopulateDefaults()
        {
            profiles = new List<ElementProfile>
            {
                new ElementProfile { element = Element.Metal,  maxUnits = 10, damagePerUnit = 5f, tickInterval = 0.5f, consumePerTick = 1f },
                new ElementProfile { element = Element.Wood,  maxUnits = 12, damagePerUnit = 3f, tickInterval = 0.4f, consumePerTick = 1f },
                new ElementProfile { element = Element.Water, maxUnits = 10, damagePerUnit = 2f, tickInterval = 0.5f, consumePerTick = 1f },
                new ElementProfile { element = Element.Fire,  maxUnits = 10, damagePerUnit = 6f, tickInterval = 0.4f, consumePerTick = 1f },
                new ElementProfile { element = Element.Earth,  maxUnits = 12, damagePerUnit = 3f, tickInterval = 0.6f, consumePerTick = 1f },
            };

            interactions = new List<ElementInteraction>
            {
                // ---- 6 REACTIONS ----
                // Steam 
                new ElementInteraction {
                    elementA = Element.Fire, elementB = Element.Water, reaction = ElementalReactionType.Steam,
                    costA = 2f, costB = 1f, reactionDamagePerUnit = 18f, effectDuration = 0f, effectMagnitude = 0f },

                // Melt 
                new ElementInteraction {
                    elementA = Element.Metal, elementB = Element.Fire, reaction = ElementalReactionType.Melt,
                    costA = 2f, costB = 1f, reactionDamagePerUnit = 22f, effectDuration = 0f, effectMagnitude = 0f },

                // Wildfire 
                new ElementInteraction {
                    elementA = Element.Wood, elementB = Element.Fire, reaction = ElementalReactionType.Wildfire,
                    costA = 1f, costB = 1f, reactionDamagePerUnit = 8f, effectDuration = 4f, effectMagnitude = 6f },

                // Mud 
                new ElementInteraction {
                    elementA = Element.Water, elementB = Element.Earth, reaction = ElementalReactionType.Mud,
                    costA = 1f, costB = 2f, reactionDamagePerUnit = 6f, effectDuration = 3f, effectMagnitude = 0.5f },

                // Reflect 
                new ElementInteraction {
                    elementA = Element.Earth, elementB = Element.Metal, reaction = ElementalReactionType.Reflect,
                    costA = 1f, costB = 1f, reactionDamagePerUnit = 4f, effectDuration = 4f, effectMagnitude = 0.5f },

                // Overgrowth
                new ElementInteraction {
                    elementA = Element.Wood, elementB = Element.Water, reaction = ElementalReactionType.Overgrowth,
                    costA = 1f, costB = 1f, reactionDamagePerUnit = 5f, effectDuration = 5f, effectMagnitude = 8f },

                // ---- 4 NEUTRAL PAIRS (resist) ----
                // Metal + Wood -> big reduction.
                new ElementInteraction {
                    elementA = Element.Metal, elementB = Element.Wood, reaction = ElementalReactionType.None,
                    blockRatio = 0.5f, damageReduction = 0.6f },

                // Metal + Water -> light resist.
                new ElementInteraction {
                    elementA = Element.Metal, elementB = Element.Water, reaction = ElementalReactionType.None,
                    blockRatio = 1f, damageReduction = 0.4f },

                // Wood + Earth -> medium resist.
                new ElementInteraction {
                    elementA = Element.Wood, elementB = Element.Earth, reaction = ElementalReactionType.None,
                    blockRatio = 1f, damageReduction = 0.5f },

                // Fire + Earth -> light resist.
                new ElementInteraction {
                    elementA = Element.Fire, elementB = Element.Earth, reaction = ElementalReactionType.None,
                    blockRatio = 1f, damageReduction = 0.4f },
            };

            _profileLookup = null;
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
#endif
            Debug.Log($"[ElementSystemConfig] Populated {profiles.Count} profiles, {interactions.Count} interactions.");
        }
    }
}
