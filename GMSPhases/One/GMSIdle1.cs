using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using LostAndChained.Phases.Attacks;
using Silksong.FsmUtil;
using System;
using System.Collections.Generic;
using System.Text;
using static LostAndChained.Constants;

namespace LostAndChained.Phases.One
{
    internal class GMSIdle1 : BasePhase
    {
        public GMSIdle1(PlayMakerFSM controlFSM) : base(controlFSM)
        {
        }

        public override string GetControlStateName()
        {
            return PhaseNames.Idle1;
        }

        public override void Init()
        {
            FsmState controlState = _controlFSM.CopyState("Idle", PhaseNames.Idle1);
            //controlState.RemoveTransition("ATTACK");
            //controlState.ChangeTransition("ATTACK", GMSAttackList.DashSlashAttack.GetStartStateName());

            FsmState attackState = _controlFSM.AddState("Idle 1 Attack Choice");
            attackState.AddAction(new SendRandomEventV4
            {
                events = new[] {
                    FsmEvent.GetFsmEvent("DASH"),
                    FsmEvent.GetFsmEvent("HAND"),
                },
                weights = new[]
               {
                    new FsmFloat(1f),
                    new FsmFloat(1f),
                },
                eventMax = new[]
               {
                    new FsmInt(2),
                    new FsmInt(2),
                },
                missedMax = new[]
               {
                    new FsmInt(4),
                    new FsmInt(4),
                },
                activeBool = true
            });

            controlState.ChangeTransition("ATTACK", "Idle 1 Attack Choice");
            attackState.AddTransition("DASH", GMSAttackList.DashSlash.GetStartStateName());
            attackState.AddTransition("HAND", "Set Primary Hand");
        }
    }
}
