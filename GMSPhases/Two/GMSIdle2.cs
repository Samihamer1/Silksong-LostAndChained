using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using LostAndChained.Components;
using LostAndChained.Phases.Attacks;
using LostAndChained.Phases.One;
using Silksong.FsmUtil;
using static LostAndChained.Constants;

namespace LostAndChained.GMSPhases.Two
{
    internal class GMSIdle2 : BasePhase
    {
        public GMSIdle2(PlayMakerFSM controlFSM) : base(controlFSM)
        {
        }

        public override string GetControlStateName()
        {
            return PhaseNames.Idle2;
        }

        public override void Init()
        {
            bool activateTransition = true;

            FsmState attackState = _controlFSM.AddState("Idle 2 Attack Choice");
            attackState.AddMethod(_ =>
            {
               if (LaceBossScene.Instance.phaseNumber != 3)
                {
                    _controlFSM.SendEvent("ATTACK");
                }             

                if (LaceBossScene.Instance.phaseNumber == 3)
                {
                    _controlFSM.SetState("Idle Choice"); //dunno why this can possibly happen but it does
                }
            });

            //attackState.AddTransition("DASH", GMSAttackList.DashSlash.GetStartStateName());
            attackState.AddTransition("ATTACK", "Set Primary Hand");


            FsmState controlState = _controlFSM.AddState(PhaseNames.Idle2);
            //controlState.RemoveTransition("ATTACK");
            //controlState.ChangeTransition("ATTACK", GMSAttackList.DashSlashAttack.GetStartStateName());

            controlState.AddAction(new Wait { time = 4f, realTime = false, finishEvent = FsmEvent.GetFsmEvent("FINISHED") });

            controlState.AddTransition("FINISHED", "Idle 2 Attack Choice");
        }
    }
}
