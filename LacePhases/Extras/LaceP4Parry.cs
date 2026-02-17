using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using LostAndChained.Phases.Attacks;
using Silksong.FsmUtil;
using System;
using System.Collections.Generic;
using System.Text;

namespace LostAndChained.LacePhases.Extras
{
    internal class LaceP4Parry : BaseAttack
    {
        public LaceP4Parry(PlayMakerFSM controlFSM, string EndStateName, string parentAttackString) : base(controlFSM, EndStateName, parentAttackString)
        {
        }

        public override string GetAttackName()
        {
            return "P4 Parry";
        }

        public override string GetStartStateNamePure()
        {
            return "P4 Counter Antic";
        }

        public override void Init()
        {
            FsmState CounterAntic = CopyState("Counter Antic", "P4 Counter Antic");
            FsmState CounterStance = CopyState("Counter Stance", "P4 Counter Stance");
            FsmState CounterEnd = CopyState("Counter End", "P4 Counter End");

            CounterStance.GetFirstActionOfType<ActivateGameObject>().enabled = false;
            CounterStance.GetFirstActionOfType<SendMessage>().enabled = false;
            CounterStance.GetFirstActionOfType<Wait>().time = 3;

            CounterAntic.ChangeTransition("FINISHED", CounterStance.name);
            CounterAntic.RemoveTransition("BLOCKED HIT");
            CounterStance.ChangeTransition("END", CounterEnd.name);
            CounterStance.RemoveTransition("BLOCKED HIT");
            CounterEnd.ChangeTransition("FINISHED", _endStateName);
        }
    }
}
