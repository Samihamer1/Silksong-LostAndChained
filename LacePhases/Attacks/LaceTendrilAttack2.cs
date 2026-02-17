using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using LostAndChained.Phases.Attacks;
using Silksong.FsmUtil;
using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Text;
using static LostAndChained.Constants;

namespace LostAndChained.LacePhases.Attacks
{
    internal class LaceTendrilAttack2 : BaseAttack
    {
        public LaceTendrilAttack2(PlayMakerFSM controlFSM, string EndStateName, string parentAttackString) : base(controlFSM, EndStateName, parentAttackString)
        {
        }

        public override string GetAttackName()
        {
            return "Tendril Attack";
        }

        public override string GetStartStateNamePure()
        {
            return "Tendril Type 2";
        }

        public override void Init()
        {
            //Tendril Type
            FsmState TendrilTypeState = CopyState("Tendril Type", "Tendril Type 2");

            //Ground attack
            FsmState GAnticState = CopyState("Tendril G Antic", "Tendril G Antic 2");
            FsmState GDashState = CopyState("Tendril G Dash", "Tendril G Dash 2");
            FsmState GWhipState =  CopyState("Tendril G Whip", "Tendril G Whip 2");
            FsmState GWhipEndState = CopyState("Tendril G Whip End", "Tendril G Whip End 2");

            //Air attack
            FsmState AAnticState =  CopyState("Tendril A Antic", "Tendril A Antic 2");
            FsmState AWhipState = CopyState("Tendril A Whip", "Tendril A Whip 2");
            FsmState AEndState = CopyState("Tendril A End", "Tendril A End 2");


            //transition fixes
            TendrilTypeState.ChangeTransition("FINISHED", GAnticState.name);
            TendrilTypeState.ChangeTransition("AIR", AAnticState.name);
            GAnticState.ChangeTransition("FINISHED", GDashState.name);
            GDashState.ChangeTransition("FINISHED", GWhipState.name);
            GWhipState.ChangeTransition("FINISHED", GWhipEndState.name);
            GWhipEndState.ChangeTransition("FINISHED", _endStateName);
            AAnticState.ChangeTransition("FINISHED", AWhipState.name);
            AWhipState.ChangeTransition("FINISHED", AEndState.name);
            AEndState.ChangeTransition("FINISHED", _endStateName);
            

            

        }
    }
}
