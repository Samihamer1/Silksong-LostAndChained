using HutongGames.PlayMaker;
using LostAndChained.LacePhases.Attacks;
using LostAndChained.Phases.Attacks;
using Silksong.FsmUtil;
using System;
using System.Collections.Generic;
using System.Text;

namespace LostAndChained.LacePhases.Extras
{
    internal class LaceThreadCheck : BaseAttack
    {
        public LaceThreadCheck(PlayMakerFSM controlFSM, string EndStateName, string parentAttackString) : base(controlFSM, EndStateName, parentAttackString)
        {
        }

        public override string GetAttackName()
        {
            return "Thread Check";
        }

        public override string GetStartStateNamePure()
        {
            return "Thread Check";
        }

        public override void Init()
        {
            FsmState ThreadCheck = AddState("Thread Check");

            ThreadCheck.AddMethod(_ =>
            {
                if (LaceBlackThreads.PreventCircles())
                {
                    _controlFSM.SendEvent("CANCEL");
                }
            });

            ThreadCheck.AddTransition("FINISHED", _endStateName);
            ThreadCheck.AddTransition("CANCEL", "Idle Choice");
        }
    }
}
