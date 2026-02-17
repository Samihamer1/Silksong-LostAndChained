using HutongGames.PlayMaker;
using LostAndChained.Phases.Attacks;
using Silksong.FsmUtil;
using System;
using System.Collections.Generic;
using System.Text;

namespace LostAndChained.LacePhases.Movement
{
    internal class LaceEvade : BaseAttack
    {
        public LaceEvade(PlayMakerFSM controlFSM, string EndStateName, string parentAttackString) : base(controlFSM, EndStateName, parentAttackString)
        {
        }

        public override string GetAttackName()
        {
            return "Evade";
        }

        public override string GetStartStateNamePure()
        {
            return "Evade 2";
        }

        public override void Init()
        {
            FsmState Evade = CopyState("Evade", "Evade 2");
            FsmState EvadeRecover = CopyState("Evade Recover", "Evade Recover 2");

            Evade.ChangeTransition("FINISHED", EvadeRecover.name);
            EvadeRecover.ChangeTransition("FINISHED", _endStateName);
        }
    }
}
