using System;
using UnityEngine;

namespace ElementalReaction
{
    // Elemental seal currently present on the target.
    [Serializable]
    public class ElementalSeal
    {
        public Element element;
        public float units;

        public ElementalSeal(Element element, float units)
        {
            this.element = element;
            this.units = units;
        }

        public bool IsActive => units > 0.001f;
    }
}
