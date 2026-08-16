using UnityEngine;

namespace ElementalReaction
{
    /// <summary>Anything that can take damage. The elemental system depends only on this,
    /// not on any concrete health implementation.</summary>
    public interface IDamageable
    {
        void TakeDamage(float amount, GameObject source = null);
        bool IsAlive { get; }
    }
}
