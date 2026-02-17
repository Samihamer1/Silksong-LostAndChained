using HutongGames.PlayMaker;
using LostAndChained.Phases.Attacks;
using Silksong.FsmUtil;
using System;
using System.Collections.Generic;
using System.Text;

namespace LostAndChained.LacePhases.Attacks
{
    internal class LaceVomitAir : BaseAttack
    {
        public LaceVomitAir(PlayMakerFSM controlFSM, string EndStateName, string parentAttackString) : base(controlFSM, EndStateName, parentAttackString)
        {
        }

        public override string GetAttackName()
        {
            return "Vomit Air";
        }

        public override string GetStartStateNamePure()
        {
            return "Vomit Air 2";
        }

        public override void Init()
        {
            FsmState VomitAir = CopyState("Vomit Air", "Vomit Air 2");
            FsmState VomitAntic = CopyState("Vomit Antic", "Vomit Antic 2");
            FsmState VomitStart = CopyState("Vomit Start", "Vomit Start 2");
            FsmState VomitFire = CopyState("Vomit Fire", "Vomit Fire 2");
            FsmState VomitRepeat = CopyState("Vomit Repeat?", "Vomit Repeat 2");
            FsmState VomitCooldown = CopyState("Vomit Cooldown", "Vomit Cooldown 2");
            FsmState VomitEnd = CopyState("Vomit End", "Vomit End 2");
            FsmState CastToAir = CopyState("Cast To Air", "Cast To Air 2");


            VomitAir.ChangeTransition("FINISHED", VomitAntic.name);
            VomitAntic.ChangeTransition("FINISHED", VomitStart.name);
            VomitStart.ChangeTransition("FINISHED", VomitFire.name);
            VomitFire.ChangeTransition("FINISHED", VomitRepeat.name);
            VomitRepeat.ChangeTransition("REPEAT", VomitFire.name);
            VomitRepeat.ChangeTransition("END", VomitCooldown.name);
            VomitCooldown.ChangeTransition("FINISHED", VomitEnd.name);
            VomitEnd.ChangeTransition("FINISHED", CastToAir.name);
            VomitEnd.RemoveTransition("AIR");
            CastToAir.ChangeTransition("FINISHED", _endStateName);
        }
    }
}
