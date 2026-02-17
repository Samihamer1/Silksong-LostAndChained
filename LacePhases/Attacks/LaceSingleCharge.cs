using HutongGames.PlayMaker;
using LostAndChained.Phases.Attacks;
using Silksong.FsmUtil;
using System;
using System.Collections.Generic;
using System.Text;

namespace LostAndChained.LacePhases.Attacks
{
    internal class LaceSingleCharge : BaseAttack
    {
        public LaceSingleCharge(PlayMakerFSM controlFSM, string EndStateName, string parentAttackString) : base(controlFSM, EndStateName, parentAttackString)
        {
        }

        public override string GetAttackName()
        {
            return "Single Charge";
        }

        public override string GetStartStateNamePure()
        {
            return "Single Charge Antic";
        }

        public override void Init()
        {
            //States
            FsmState ChargeAntic = CopyState("Charge Antic", "Single Charge Antic");
            FsmState ChargeBreak = CopyState("Charge Break", "Single Charge Break");
            FsmState Charge = CopyState("Charge", "Single Charge");
            FsmState ChargeRecover = CopyState("Charge Recover", "Single Charge Recover");
            FsmState ChargeStrike = CopyState("Charge Strike", "Single Charge Strike");
            FsmState ComboStrikeFinisher = CopyState("Combo Strike Finisher", "Single Charge Finisher");

            //transitions
            ChargeAntic.ChangeTransition("NEXT", ChargeBreak.name);
            ChargeBreak.ChangeTransition("NEXT", Charge.name);
            Charge.RemoveTransition("J SLASH");
            Charge.ChangeTransition("MULTI HIT CONNECT", ChargeStrike.name);
            Charge.ChangeTransition("NEXT", ChargeRecover.name);
            ChargeStrike.ChangeTransition("FINISHED", ComboStrikeFinisher.name);
            ComboStrikeFinisher.ChangeTransition("FINISHED", "Idle Choice");
            ChargeRecover.ChangeTransition("FINISHED", _endStateName);

        }
    }
}
