using Silksong.FsmUtil;
using System;
using System.Collections.Generic;
using System.Text;

namespace LostAndChained.LacePhases.Movement
{
    internal class LaceSplashOutGroundClose : LaceBaseSplashOut
    {
        public LaceSplashOutGroundClose(PlayMakerFSM controlFSM, string EndStateName, string parentAttackString) : base(controlFSM, EndStateName, parentAttackString)
        {
        }

        public override string GetAttackName()
        {
            return "Splash Out Ground Close";
        }

        public override void PostInit()
        {
            GetConfigState().AddMethod(_ =>
            {
                SetTeleportConfig(new LaceTeleportConfig
                {
                    distanceMin = 0f,
                    distanceMax = 6f,
                    distanceFailMax = 10f,
                    hopStopDistance = 5f,
                    teleY = Constants.Constraints.LaceLandY
                });
            });
        }
    }
}
