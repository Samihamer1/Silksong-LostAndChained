using Silksong.FsmUtil;
using System;
using System.Collections.Generic;
using System.Text;

namespace LostAndChained.LacePhases.Movement
{
    internal class LaceSplashOutAirCloseFollow : LaceBaseSplashOut
    {
        public LaceSplashOutAirCloseFollow(PlayMakerFSM controlFSM, string EndStateName, string parentAttackString) : base(controlFSM, EndStateName, parentAttackString)
        {
        }

        public override string GetAttackName()
        {
            return "Splash Out Air Close";
        }

        public override void PostInit()
        {
            GetConfigState().AddMethod(_ =>
            {
                SetTeleportConfig(new LaceTeleportConfig
                {
                    distanceMin = 5f,
                    distanceMax = 8f,
                    distanceFailMax = 10f,
                    hopStopDistance = 5f,
                    teleY = HeroController.instance.transform.position.y + 3f
                });
            });
        }
    }
}
