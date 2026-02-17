using System;
using System.Collections.Generic;
using System.Text;

namespace LostAndChained.LacePhases.Awaits
{
    internal class LaceAwaitWeb : LaceAwaitBase
    {
        public LaceAwaitWeb(PlayMakerFSM controlFSM, string EndStateName, string parentAttackString) : base(controlFSM, EndStateName, parentAttackString)
        {
        }

        public override string GetAttackName()
        {
            return "WEB";
        }
    }
}
