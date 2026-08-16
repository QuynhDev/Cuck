using UnityEngine;

namespace ElementalReaction
{
    public static class WuXing
    {
        public const int Count = 5;

        public static Element Generates(Element e)
        {
            switch (e)
            {
                case Element.Wood: return Element.Fire;
                case Element.Fire: return Element.Earth;
                case Element.Earth: return Element.Metal;
                case Element.Metal: return Element.Water;
                case Element.Water: return Element.Wood;
                default: return e;
            }
        }

        public static Element Overcomes(Element e)
        {
            switch (e)
            {
                case Element.Wood: return Element.Earth;
                case Element.Earth: return Element.Water;
                case Element.Water: return Element.Fire;
                case Element.Fire: return Element.Metal;
                case Element.Metal: return Element.Wood;
                default: return e;
            }
        }

        public static bool DoesGenerate(Element a, Element b) => Generates(a) == b;

        public static bool DoesOvercome(Element a, Element b) => Overcomes(a) == b;

        public static string DisplayName(Element e)
        {
            switch (e)
            {
                case Element.Metal: return "Metal";
                case Element.Wood: return "Wood";
                case Element.Water: return "Water";
                case Element.Fire: return "Fire";
                case Element.Earth: return "Earth";
                default: return e.ToString();
            }
        }

        public static Color ElementColor(Element e)
        {
            switch (e)
            {
                case Element.Metal: return new Color(0.85f, 0.82f, 0.55f); // metallic gold
                case Element.Wood: return new Color(0.35f, 0.75f, 0.35f); // green
                case Element.Water: return new Color(0.30f, 0.60f, 0.95f); // water blue
                case Element.Fire: return new Color(0.95f, 0.40f, 0.25f); // fire red
                case Element.Earth: return new Color(0.70f, 0.55f, 0.35f); // earth brown
                default: return Color.white;
            }
        }
    }
}
