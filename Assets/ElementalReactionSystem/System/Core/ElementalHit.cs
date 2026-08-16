using UnityEngine;

namespace ElementalReaction
{
    public struct ElementalHit
    {
        public Element element;
        public float units;
        public float directDamage;
        public GameObject source;

        public ElementalHit(Element element, float directDamage, float units, GameObject source = null)
        {
            this.element = element;
            this.directDamage = directDamage;
            this.units = units;
            this.source = source;
        }
    }
}

