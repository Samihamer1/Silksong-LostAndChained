using HutongGames.PlayMaker;
using LostAndChained.Phases.Attacks;
using Silksong.FsmUtil;
using System;
using System.Collections.Generic;
using System.Text;

namespace LostAndChained.LacePhases.Two
{
    internal class LacePhase2Shift : BaseAttack
    {
        public LacePhase2Shift(PlayMakerFSM controlFSM, string EndStateName, string parentAttackString) : base(controlFSM, EndStateName, parentAttackString)
        {
        }

        public override string GetAttackName()
        {
            return "Phase 2 Shift";
        }

        public override string GetStartStateNamePure()
        {
            return "P2 Shift 1-2";
        }

        public override void Init()
        {
            FsmState Shift1 = CopyState("P2 Shift 1", "P2 Shift 1-2");
            FsmState Shift2 = CopyState("P2 Shift 2", "P2 Shift 2-2");

            Shift1.ChangeTransition("FINISHED", Shift2.name);
            Shift2.ChangeTransition("FINISHED", _endStateName);
        }
    }
}
