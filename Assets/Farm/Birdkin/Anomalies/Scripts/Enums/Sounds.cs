using UnityEditor;
using UnityEngine;

namespace Bird
{
    public enum Sounds
    {
        // Bird Related (0-99)

        // Breath
        LoudBreath = 0,
        QuietBreath = 1,

        // Mouth Sound
        Growl = 10,
        Sniff = 11,
        Hiss = 12,
        Caw = 13,

        // Body Sound
        BeakClack = 20,
        FeatherFlap = 21,

        // Footsteps
        QuietFootsteps = 30,
        LoudFootsteps = 31,
        WetFootsteps = 32,

        // Environment Related (100-199)

        // Forest
        CrackingBranches = 100,
        LeavesRustleOnWind = 101,
        BushRustle = 102,
        GustOfWind = 103,

        // Other (200-299)


        // Misc
        Miscellaneous = 300
    }
}