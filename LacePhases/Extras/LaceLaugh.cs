using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using LostAndChained.Phases.Attacks;
using Silksong.FsmUtil;
using System;
using System.Collections.Generic;
using System.Text;

namespace LostAndChained.LacePhases.Extras
{
    internal class LaceLaugh : BaseAttack
    {
        public LaceLaugh(PlayMakerFSM controlFSM, string EndStateName, string parentAttackString) : base(controlFSM, EndStateName, parentAttackString)
        {
        }

        public override string GetAttackName()
        {
            return "Lace Laugh";
        }

        public override string GetStartStateNamePure()
        {
            return "Laugh";
        }

        public override void Init()
        {
            FsmState Laugh = CopyState("Death Pose", "Laugh");
            Laugh.AddAction(new Wait { time = 3f, realTime = false, finishEvent = FsmEvent.GetFsmEvent("FINISHED") });

            Laugh.AddTransition("FINISHED", _endStateName);
        }
    }
}
