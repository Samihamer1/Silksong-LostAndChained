using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using LostAndChained.Components;
using LostAndChained.Phases.Attacks;
using Silksong.FsmUtil;
using System;
using System.Collections.Generic;
using System.Text;

namespace LostAndChained.LacePhases.Extras
{
    internal class LaceP4Lunge : BaseAttack
    {
        public LaceP4Lunge(PlayMakerFSM controlFSM, string EndStateName, string parentAttackString) : base(controlFSM, EndStateName, parentAttackString)
        {
        }

        public override string GetAttackName()
        {
            return "P4 Lunge";
        }

        public override string GetStartStateNamePure()
        {
            return "P4 Charge Antic";
        }

        public override void Init()
        {
            FsmState ChargeAntic = CopyState("Charge Antic", "P4 Charge Antic");
            FsmState ChargeBreak = CopyState("Charge Break", "P4 Charge Break");
            FsmState Charge = CopyState("Charge", "P4 Charge");

            Charge.GetFirstActionOfType<PlayParticleEmitterInState>().enabled = false;
            Charge.GetAction<ActivateGameObject>(5).enabled = false;
            Charge.GetAction<SetDamageHero>(13).enabled = false;
            Charge.GetAction<ActivateGameObject>(14).enabled = false;
            Charge.GetAction<SpawnObjectFromGlobalPoolOverTime>(16).enabled = false;

            Charge.AddMethod(_ =>
            {
                LaceBossScene.Instance.GMSMain.PromptParry();
            });



            ChargeAntic.ChangeTransition("NEXT", ChargeBreak.name);
            ChargeBreak.ChangeTransition("NEXT", Charge.name);
            Charge.RemoveTransition("J SLASH");
            Charge.RemoveTransition("MULTI HIT CONNECT");
            Charge.RemoveTransition("NEXT");

            Charge.AddTransition("CONTINUE", _endStateName);
        }
    }
}
