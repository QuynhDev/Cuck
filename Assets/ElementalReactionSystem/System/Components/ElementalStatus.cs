using System;
using System.Collections.Generic;
using UnityEngine;

namespace ElementalReaction
{
    /// <summary>
    /// Attach to an enemy. Manages the elemental seal, unit-consuming DoT, resolves reactions when a new
    /// seal is applied, and the side effects (Mud = slow, Reflect, Wildfire/Overgrowth = extra DoT).
    /// Requires any component implementing <see cref="IDamageable"/> on the same GameObject.
    /// </summary>
    public class ElementalStatus : MonoBehaviour
    {
        [Tooltip("Shared elemental system configuration.")]
        [SerializeField] private ElementSystemConfig config;

        [Header("State (read-only)")]
        [SerializeField] private Element currentElement;
        [SerializeField] private float currentUnits;

        private ElementalSeal _seal;
        private IDamageable _damageable;
        private float _tickTimer;

        // Side effects produced by reactions.
        private readonly List<ReactionDot> _reactionDots = new List<ReactionDot>();
        private float _slowRemaining, _slowMagnitude;
        private float _reflectRemaining, _reflectMagnitude;

        // ---- Events for VFX / UI ----
        public event Action<ElementalReactionResult> OnHitResolved;
        public event Action<ElementalReactionType, ElementalReactionResult> OnReaction;
        public event Action<ElementalSeal> OnSealChanged; // seal may be null (cleared)
        public event Action<Element, float> OnDotTick;

        // ---- Queries for other gameplay systems ----
        public bool HasSeal => _seal != null && _seal.IsActive;
        public Element CurrentElement => _seal != null ? _seal.element : currentElement;
        public float CurrentUnits => _seal != null ? _seal.units : 0f;
        public ElementSystemConfig Config { get => config; set => config = value; }

        private void Awake()
        {
            _damageable = GetComponent<IDamageable>();
            if (_damageable == null)
                Debug.LogWarning($"[ElementalStatus] No IDamageable found on {name}; damage will be ignored.", this);
        }

        /// <summary>Main entry point: deal an elemental hit to this target.</summary>
        public void ApplyHit(ElementalHit hit)
        {
            if (config == null)
            {
                Debug.LogWarning($"[ElementalStatus] ElementSystemConfig is not assigned on {name}.", this);
                return;
            }

            var result = ElementalReactionResolver.Resolve(config, _seal, hit);

            // Reflect: send a portion of the direct damage back to the source.
            if (_reflectRemaining > 0f && hit.source != null && result.directDamage > 0f)
            {
                var srcDmg = hit.source.GetComponent<IDamageable>();
                if (srcDmg != null)
                    srcDmg.TakeDamage(result.directDamage * _reflectMagnitude, gameObject);
            }

            // Apply direct damage + reaction damage.
            if (result.TotalDamage > 0f)
                _damageable?.TakeDamage(result.TotalDamage, hit.source);

            // Update the seal.
            SetSeal(result.resultingSeal);

            // Side effects per reaction type.
            if (result.kind == InteractionKind.Reacted)
            {
                ApplyReactionEffect(result);
                OnReaction?.Invoke(result.reaction, result);
            }

            OnHitResolved?.Invoke(result);
        }

        /// <summary>Convenience overload to build a hit quickly.</summary>
        public void ApplyHit(Element element, float directDamage, float units, GameObject source = null)
            => ApplyHit(new ElementalHit(element, directDamage, units, source));

        private void ApplyReactionEffect(ElementalReactionResult r)
        {
            switch (r.reaction)
            {
                case ElementalReactionType.Mud: // slow
                    _slowRemaining = Mathf.Max(_slowRemaining, r.effectDuration);
                    _slowMagnitude = Mathf.Max(_slowMagnitude, r.effectMagnitude);
                    break;

                case ElementalReactionType.Reflect: // reflect
                    _reflectRemaining = Mathf.Max(_reflectRemaining, r.effectDuration);
                    _reflectMagnitude = Mathf.Max(_reflectMagnitude, r.effectMagnitude);
                    break;

                case ElementalReactionType.Wildfire: // spreading fire DoT, fixed intensity
                    _reactionDots.Add(new ReactionDot(Element.Fire, r.effectMagnitude, 0.5f, r.effectDuration, 0f));
                    break;

                case ElementalReactionType.Overgrowth: // DoT that grows over time
                    _reactionDots.Add(new ReactionDot(Element.Wood, r.effectMagnitude * 0.4f, 0.5f, r.effectDuration, r.effectMagnitude * 0.15f));
                    break;

                // Steam & Melt: burst damage only (already in reactionDamage).
            }
        }

        private void Update()
        {
            float dt = Time.deltaTime;
            TickSealDot(dt);
            TickReactionDots(dt);

            if (_slowRemaining > 0f) _slowRemaining -= dt;
            if (_reflectRemaining > 0f) _reflectRemaining -= dt;
        }

        private void TickSealDot(float dt)
        {
            if (_seal == null || !_seal.IsActive) return;

            var profile = config.GetProfile(_seal.element);
            _tickTimer += dt;

            while (_tickTimer >= profile.tickInterval && _seal.IsActive)
            {
                _tickTimer -= profile.tickInterval;
                float consume = Mathf.Min(profile.consumePerTick, _seal.units);
                _seal.units -= consume;
                float dmg = consume * profile.damagePerUnit;

                _damageable?.TakeDamage(dmg, gameObject);
                OnDotTick?.Invoke(_seal.element, dmg);
            }

            currentUnits = _seal.units;
            if (!_seal.IsActive)
                SetSeal(null);
        }

        private void TickReactionDots(float dt)
        {
            for (int i = _reactionDots.Count - 1; i >= 0; i--)
            {
                var dot = _reactionDots[i];
                dot.timeRemaining -= dt;
                dot.tickTimer += dt;

                while (dot.tickTimer >= dot.tickInterval && dot.timeRemaining > -dot.tickInterval)
                {
                    dot.tickTimer -= dot.tickInterval;
                    dot.damagePerTick += dot.growthPerTick; // Overgrowth grows over time
                    _damageable?.TakeDamage(dot.damagePerTick, gameObject);
                    OnDotTick?.Invoke(dot.flavor, dot.damagePerTick);
                }

                if (dot.timeRemaining <= 0f)
                    _reactionDots.RemoveAt(i);
                else
                    _reactionDots[i] = dot;
            }
        }

        private void SetSeal(ElementalSeal seal)
        {
            _seal = seal;
            if (_seal != null)
            {
                currentElement = _seal.element;
                currentUnits = _seal.units;
            }
            else
            {
                currentUnits = 0f;
                _tickTimer = 0f;
            }
            OnSealChanged?.Invoke(_seal);
        }

        /// <summary>Secondary DoT produced by a reaction (not the main seal).</summary>
        private struct ReactionDot
        {
            public Element flavor;
            public float damagePerTick;
            public float tickInterval;
            public float timeRemaining;
            public float growthPerTick;
            public float tickTimer;

            public ReactionDot(Element flavor, float damagePerTick, float tickInterval, float duration, float growthPerTick)
            {
                this.flavor = flavor;
                this.damagePerTick = damagePerTick;
                this.tickInterval = tickInterval;
                this.timeRemaining = duration;
                this.growthPerTick = growthPerTick;
                this.tickTimer = 0f;
            }
        }
    }
}
