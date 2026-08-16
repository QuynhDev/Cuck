using DG.Tweening;
using UnityEngine;

namespace UI.Effects
{
    /// <summary>
    /// Reusable DOTween-based "pop up" feedback (punch-scale bounce),
    /// in the same spirit as the Balatro card hover/select juice.
    /// </summary>
    public static class PopUpEffect
    {
        public static Tween Play(
            Transform target,
            float punchScale = 0.25f,
            float duration = 0.35f,
            int vibrato = 8,
            float elasticity = 0.9f)
        {
            if (target == null) return null;

            DOTween.Kill(target, true);
            target.localScale = Vector3.one;

            return target
                .DOPunchScale(Vector3.one * punchScale, duration, vibrato, elasticity)
                .SetId(target)
                .SetLink(target.gameObject);
        }
    }
}
