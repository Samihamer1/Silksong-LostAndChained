using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using LostAndChained.Phases.Attacks;
using Silksong.FsmUtil;
using System;
using System.Collections.Generic;
using System.Text;

namespace LostAndChained.LacePhases.Attacks
{
    internal class LaceSingleChargeFast : BaseAttack
    {
        public LaceSingleChargeFast(PlayMakerFSM controlFSM, string EndStateName, string parentAttackString) : base(controlFSM, EndStateName, parentAttackString)
        {
        }

        public override string GetAttackName()
        {
            return "Single Charge Fast";
        }

        public override string GetStartStateNamePure()
        {
            return "Single Charge Antic Fast";
        }

        public override void Init()
        {
            //States
            FsmState ChargeAntic = CopyState("Charge Antic", "Single Charge Antic Fast");
            FsmState ChargeBreak = CopyState("Charge Break", "Single Charge Break Fast");
            FsmState Charge = CopyState("Charge", "Single Charge Fast");
            FsmState ChargeRecover = CopyState("Charge Recover", "Single Charge Recover Fast");
            FsmState ChargeStrike = CopyState("Charge Strike", "Single Charge Strike Fast");
            FsmState ComboStrikeFinisher = CopyState("Combo Strike Finisher", "Single Charge Finisher Fast");

            //Charge.GetFirstActionOfType<SetVelocityByScale>().speed = 85f;
            ChargeBreak.GetFirstActionOfType<Wait>().time = 0.25f;

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

