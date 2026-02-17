using System;
using System.Collections.Generic;
using System.Text;

namespace LostAndChained
{
    internal class Constants
    {
        internal static class PhaseNames
        {
            public static readonly string Idle1 = "IDLE1";
            public static readonly string Idle2 = "IDLE2";
            public static readonly string Idle3 = "IDLE3";
            public static readonly string Idle4 = "IDLE4";
        }

        internal static class PhaseValues
        {
            public static readonly int Phase1HP = 500;
            public static readonly int Phase2HP = 600;
            public static readonly int Phase3HP = 800;
            public static readonly int Phase4HP = 1000;
        }

        internal static class Constraints
        {
            public static float GMSMinX = 7f;
            public static float GMSMaxX = 52f;
            public static float ArenaCenterX = 33.5f;
            public static float ArenaCenterY = 10.5f;
            public static float GroundY = 4.5f;
            public static float LaceLandY = 6.4f; 
        }
    }
}
