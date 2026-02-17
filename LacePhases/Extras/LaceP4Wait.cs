using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using LostAndChained.Phases.Attacks;
using Silksong.FsmUtil;
using System;
using System.Collections.Generic;
using System.Text;

namespace LostAndChained.LacePhases.Extras
{
    internal class LaceP4Wait : BaseAttack
    {
        public LaceP4Wait(PlayMakerFSM controlFSM, string EndStateName, string parentAttackString) : base(controlFSM, EndStateName, parentAttackString)
        {
        }

        public override string GetAttackName()
        {
            return "Lace P4 Wait";
        }

        public override string GetStartStateNamePure()
        {
            return "P4 Wait";
        }

        public override void Init()
        {
            FsmState waitState = AddState("P4 Wait");
            waitState.AddAction(new Wait { time = 0.75f, realTime = false, finishEvent = FsmEvent.GetFsmEvent("FINISHED") });
            waitState.AddMethod(_ =>
            {
                _controlFSM.gameObject.GetComponent<tk2dSpriteAnimator>().Play("Idle");
            });

            waitState.AddTransition("FINISHED", _endStateName);
        }
    }
}
