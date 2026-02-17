using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using LostAndChained.Components;
using LostAndChained.Phases.Attacks;
using LostAndChained.Phases.One;
using Silksong.FsmUtil;
using static LostAndChained.Constants;

namespace LostAndChained.GMSPhases.Three
{
    internal class GMSIdle3 : BasePhase
    {
        public GMSIdle3(PlayMakerFSM controlFSM) : base(controlFSM)
        {
        }

        public override string GetControlStateName()
        {
            return PhaseNames.Idle3;
        }

        public override void Init()
        {
            bool activateTransition = true;

            FsmState attackState = _controlFSM.AddState("Idle 3 Attack Choice");
            FsmState comboAttackState = _controlFSM.AddState("Idle 3 Attack Combo Await");
            FsmState controlState = _controlFSM.AddState(PhaseNames.Idle3);

            //attack state

            attackState.AddAction(new SendRandomEventV4
            {
                events = new[] {
                    FsmEvent.GetFsmEvent("DASHSLASH"),
                    FsmEvent.GetFsmEvent("NEEDLESRANDOM"),
                    FsmEvent.GetFsmEvent("SPIKELIFT"),
                    FsmEvent.GetFsmEvent("RUBBLE")
                },
                weights = new[]
               {
                    new FsmFloat(1f),
                    new FsmFloat(1f),
                    new FsmFloat(1f),
                    new FsmFloat(1f),
                },
                eventMax = new[]
               {
                    new FsmInt(2),
                    new FsmInt(2),
                    new FsmInt(1),
                    new FsmInt(1),
                },
                missedMax = new[]
               {
                    new FsmInt(4),
                    new FsmInt(4),
                    new FsmInt(4),
                    new FsmInt(4)
                },
                activeBool = true
            });

            attackState.AddTransition("DASHSLASH", GMSAttackList.DashSlash.GetStartStateName());
            attackState.AddTransition("NEEDLESRANDOM", "Set Primary Hand");
            attackState.AddTransition("SPIKELIFT", GMSAttackList.SpikeLift.GetStartStateName());
            attackState.AddTransition("RUBBLE", GMSAttackList.RubblePull.GetStartStateName());


            // control state
            controlState.AddMethod(_ =>
            {
                if (LaceBossScene.Instance.laceAttemptingCombo)
                {
                    _controlFSM.SendEvent("COMBO");
                }
            });
            controlState.AddAction(new Wait { time = 1.5f, realTime = false, finishEvent = FsmEvent.GetFsmEvent("FINISHED") });
            controlState.AddTransition("FINISHED", "Idle 3 Attack Choice");
            controlState.AddTransition("COMBO", comboAttackState.name);

            //combo attack state
            comboAttackState.AddMethod(_ =>
            {
                LaceBossScene.Instance.LaceMain.ReadyCombo();
            });


            comboAttackState.AddTransition("WEB", "Web Prepare");
            comboAttackState.AddTransition("CLAW", GMSAttackList.DoubleClaw.GetStartStateName());
            comboAttackState.AddTransition("CANCEL", PhaseNames.Idle3);
        }
    }
}
