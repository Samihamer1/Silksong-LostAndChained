using System;
using System.Collections.Generic;
using System.Text;

namespace LostAndChained.LacePhases.Awaits
{
    internal class LaceAwaitClaw : LaceAwaitBase
    {
        public LaceAwaitClaw(PlayMakerFSM controlFSM, string EndStateName, string parentAttackString) : base(controlFSM, EndStateName, parentAttackString)
        {
        }

        public override string GetAttackName()
        {
            return "CLAW";
        }
    }
}
