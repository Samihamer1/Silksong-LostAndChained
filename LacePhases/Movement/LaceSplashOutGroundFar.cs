using Silksong.FsmUtil;
using System;
using System.Collections.Generic;
using System.Text;

namespace LostAndChained.LacePhases.Movement
{
    internal class LaceSplashOutGroundFar : LaceBaseSplashOut
    {
        public LaceSplashOutGroundFar(PlayMakerFSM controlFSM, string EndStateName, string parentAttackString) : base(controlFSM, EndStateName, parentAttackString)
        {
        }

        public override string GetAttackName()
        {
            return "Splash Out Ground";
        }

        public override void PostInit()
        {
            GetConfigState().AddMethod(_ =>
            {
                SetTeleportConfig(new LaceTeleportConfig
                {
                    distanceMin = 6f,
                    distanceMax = 7f,
                    distanceFailMax = 15f,
                    hopStopDistance = 7f,
                    teleY = Constants.Constraints.LaceLandY
                });
            });
        }
    }
}
