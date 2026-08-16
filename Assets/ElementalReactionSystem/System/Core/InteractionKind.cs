using UnityEngine;

namespace ElementalReaction
{
    public enum InteractionKind
    {
        Applied,   // Applied a new seal (no existing seal)
        Refreshed, // Same element -> add units
        Reacted,   // Produced a reaction
        Resisted   // Neutral -> existing seal resisted the hit
    }
}
