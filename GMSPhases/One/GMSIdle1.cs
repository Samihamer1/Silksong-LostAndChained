using HutongGames.PlayMaker;
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
            attackState.AddMethod(_ =>
            {
                if (UnityEngine.Random.Range(0, 2) == 0)
                {
                    _controlFSM.SendEvent("DASH");
                }
                _controlFSM.SendEvent("HAND");
            });

            controlState.ChangeTransition("ATTACK", "Idle 1 Attack Choice");
            attackState.AddTransition("DASH", GMSAttackList.DashSlash.GetStartStateName());
            attackState.AddTransition("HAND", "Set Primary Hand");
        }
    }
}
