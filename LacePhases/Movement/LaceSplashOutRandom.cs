using HutongGames.PlayMaker;
using LostAndChained.Phases.Attacks;
using Silksong.FsmUtil;

namespace LostAndChained.LacePhases.Movement
{
    internal class LaceSplashOutRandom : LaceBaseSplashOut
    {
        public LaceSplashOutRandom(PlayMakerFSM controlFSM, string EndStateName, string parentAttackString) : base(controlFSM, EndStateName, parentAttackString)
        {
        }

        public override string GetAttackName()
        {
            return "Splash Out Random";
        }

        public override void PostInit()
        {
            GetConfigState().AddMethod(_ =>
            {
                //land
                SetTeleportConfig(new LaceTeleportConfig
                {
                    distanceMin = 10, distanceMax = 12, distanceFailMax = 18, hopStopDistance = 11, teleY = Constants.Constraints.LaceLandY
                });

                if (UnityEngine.Random.Range(0, 2) == 0)
                {
                    SetTeleportConfig(new LaceTeleportConfig
                    {
                        distanceMin = 6,
                        distanceMax = 8,
                        distanceFailMax = 999999,
                        hopStopDistance = 11,
                        teleY = 14
                    });
                }
            });
        }
    }
}
