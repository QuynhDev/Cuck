using UnityEngine;

namespace ElementalReaction
{
    public enum ElementalReactionType
    {
        None = 0,         // Neutral - reduce damage, consume units by ratio
        Steam = 1,        // fire + water   - burst damage
        Melt = 2,         // metal + fire   - amplify + burst
        Wildfire = 3,     // wood + fire    - strong spreading DoT
        Mud = 4,          // water + earth  - slow
        Reflect = 5,      // earth + metal  - reflect damage
        Overgrowth = 6    // wood + water   - growing DoT
    }
}
