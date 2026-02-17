using LostAndChained.GMSPhases.Attacks;
using LostAndChained.GMSPhases.Two;
using LostAndChained.LacePhases.Attacks;
using LostAndChained.LacePhases.Movement;
using System;
using System.Collections.Generic;
using System.Text;

namespace LostAndChained.Phases.Attacks
{
    internal static class GMSAttackList
    {
        public static BaseAttackString DashSlash;
        public static BaseAttackString DoubleClaw;
        public static BaseAttackString RubblePull;
        public static BaseAttackString SpikeLift;
        public static void InitGMSAttackStrings(PlayMakerFSM GMSAttackControl)
        {
            DashSlash = new BaseAttackString(GMSAttackControl, "DASHSLASH", new List<Type> { typeof(GMSDashSlash) });

            DoubleClaw = new BaseAttackString(GMSAttackControl, "DOUBLECLAW", new List<Type> { typeof(GMSDoubleClaw) });

            RubblePull = new BaseAttackString(GMSAttackControl, "RUBBLEPULL", new List<Type> { typeof(GMSRubblePull) });

            SpikeLift = new BaseAttackString(GMSAttackControl, "SPIKELIFT", new List<Type> { typeof(GMSSpikeLift) });
        }
    }
}
