using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using LostAndChained.Phases.Attacks;
using Silksong.FsmUtil;
using System;
using System.Collections.Generic;
using System.Text;

namespace LostAndChained.LacePhases.Attacks
{
    internal class LaceDeflect : BaseAttack
    {
        public LaceDeflect(PlayMakerFSM controlFSM, string EndStateName, string parentAttackString) : base(controlFSM, EndStateName, parentAttackString)
        {
        }

        public override string GetAttackName()
        {
            return "Deflect";
        }

        public override string GetStartStateNamePure()
        {
            return "Deflect Antic";
        }

        public override void Init()
        {
            FsmState DeflectAntic = CopyState("Counter Antic", "Deflect Antic");
            FsmState DeflectStance = CopyState("Counter Stance", "Deflect Stance");
            FsmState DeflectEnd = CopyState("Counter End", "Deflect End");
            FsmState DeflectHit = CopyState("Counter Hit", "Deflect Hit");

            FsmState Evade = CopyState("Evade", "Evade 2");
            FsmState EvadeRecover = CopyState("Evade Recover", "Evade Recover 2");

            DeflectHit.GetFirstActionOfType<Tk2dPlayAnimationWithEvents>().clipName = "Antic";
            DeflectHit.GetFirstActionOfType<Tk2dPlayAnimationWithEvents>().enabled = false;


            DeflectAntic.ChangeTransition("FINISHED", DeflectStance.name);
            DeflectAntic.ChangeTransition("BLOCKED HIT", DeflectHit.name);
            DeflectStance.ChangeTransition("END", DeflectEnd.name);
            DeflectStance.ChangeTransition("BLOCKED HIT", DeflectHit.name);
            DeflectEnd.ChangeTransition("FINISHED", "Idle Choice");
            DeflectHit.ChangeTransition("FINISHED", Evade.name);
            Evade.ChangeTransition("FINISHED", EvadeRecover.name);
            EvadeRecover.ChangeTransition("FINISHED", _endStateName);

        }
    }
}
