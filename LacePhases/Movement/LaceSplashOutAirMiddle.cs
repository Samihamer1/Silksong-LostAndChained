using Silksong.FsmUtil;
using System;
using System.Collections.Generic;
using System.Text;

namespace LostAndChained.LacePhases.Movement
{
    internal class LaceSplashOutAirMiddle : LaceBaseSplashOut
    {
        public LaceSplashOutAirMiddle(PlayMakerFSM controlFSM, string EndStateName, string parentAttackString) : base(controlFSM, EndStateName, parentAttackString)
        {
        }

        public override string GetAttackName()
        {
            return "Splash Out Air Middle";
        }

        public override void PostInit()
        {
            GetOverrideState().AddMethod(_ =>
            {
                _controlFSM.GetFloatVariable("Tele X").value = Constants.Constraints.ArenaCenterX;
                _controlFSM.GetFloatVariable("Tele Y").value = 14f;
            }
            );
        }
    }
}
