using HutongGames.PlayMaker;
using LostAndChained.Phases.Attacks;
using Silksong.FsmUtil;
using System;
using System.Collections.Generic;
using System.Text;

namespace LostAndChained.LacePhases.Attacks
{
    internal class LaceCounter : BaseAttack
    {
        public LaceCounter(PlayMakerFSM controlFSM, string EndStateName, string parentAttackString) : base(controlFSM, EndStateName, parentAttackString)
        {
        }

        public override string GetAttackName()
        {
            return "Counter";
        }

        public override string GetStartStateNamePure()
        {
            return "Counter Antic 2";
        }

        public override void Init()
        {
            FsmState CounterAntic = CopyState("Counter Antic", "Counter Antic 2");
            FsmState CounterStance = CopyState("Counter Stance", "Counter Stance 2");
            FsmState CounterEnd = CopyState("Counter End", "Counter End 2");
            FsmState CounterHit = CopyState("Counter Hit", "Counter Hit 2");
            FsmState CounterType = CopyState("Counter Type", "Counter Type 2");

            FsmState RapidSlashCharge = CopyState("RapidSlash Charge", "RapidSlash Charge 2");
            FsmState RapidSlashLoop = CopyState("RapidSlash Loop", "RapidSlash Loop 2");
            FsmState RapidSlashEnd = CopyState("RapidSlash End", "RapidSlash End 2"); ;

            FsmState CollideCancel = CopyState("Collide Cancel", "Collide Cancel 2");
            FsmState CollideToMultihit = CopyState("Collide To Multihit", "Collide To MultiHit 2");
            FsmState HeroFacing = CopyState("Hero Facing", "Hero Facing 2");
            FsmState MultiHitting = CopyState("Multihitting", "Multihitting 2");
            FsmState MultihitSlash = CopyState("Multihit Slash", "Multihit Slash 2");

            FsmState CounterTeleOut = CopyState("Counter TeleOut", "Counter TeleOut 2");
            FsmState CounterTeleIn = CopyState("Counter TeleIn", "Counter TeleIn 2");

            FsmState RapidSlashAirAntic = CopyState("RapidSlashAir Antic", "RapidSlashAir Antic 2");
            FsmState RapidSlashAir = CopyState("RapidSlash Air", "RapidSlash Air 2");
            FsmState SlashEnd = CopyState("Slash End", "Slash End 2");

            FsmState HeroFacing2 = CopyState("Hero Facing 2", "Hero Facing 2-2");
            FsmState MultiHitting2 = CopyState("Multihitting 2", "Multihitting 2-2");
            FsmState MultihitSlash2 = CopyState("Multihit Slash 2", "Multihit Slash 2-2");
            FsmState MultihitSlashEnd = CopyState("Multihit Slash End", "Multihit Slash End-2");


            CounterAntic.AddMethod(_ =>
            {
                _controlFSM.GetBoolVariable("Will Counter").value = false;
            });

            //transitions
            CounterAntic.ChangeTransition("FINISHED", CounterStance.name);
            CounterAntic.ChangeTransition("BLOCKED HIT", CounterHit.name);
            CounterStance.ChangeTransition("END", CounterEnd.name);
            CounterStance.ChangeTransition("BLOCKED HIT", CounterHit.name);
            CounterEnd.ChangeTransition("FINISHED", _endStateName);
            CounterHit.ChangeTransition("FINISHED", CounterType.name);
            CounterType.ChangeTransition("GROUND", RapidSlashCharge.name);
            CounterType.ChangeTransition("AIR", CounterTeleOut.name);
            RapidSlashCharge.ChangeTransition("FINISHED", RapidSlashLoop.name);
            RapidSlashCharge.ChangeTransition("HERO COLLIDE", CollideToMultihit.name);
            CollideToMultihit.ChangeTransition("FINISHED", HeroFacing.name);
            CollideToMultihit.ChangeTransition("CANCEL", CollideCancel.name);
            CollideCancel.ChangeTransition("FINISHED", RapidSlashLoop.name);
            RapidSlashLoop.ChangeTransition("FINISHED", RapidSlashEnd.Name);
            RapidSlashLoop.ChangeTransition("MULTI HIT CONNECT", HeroFacing.name);
            RapidSlashEnd.ChangeTransition("FINISHED", _endStateName);
            HeroFacing.ChangeTransition("FINISHED", MultiHitting.name);
            MultiHitting.ChangeTransition("FINISHED", MultihitSlash.name);
            MultihitSlash.ChangeTransition("FINISHED", _endStateName);
            CounterTeleOut.ChangeTransition("FINISHED", CounterTeleIn.name);
            CounterTeleIn.ChangeTransition("FINISHED", RapidSlashAirAntic.name);
            RapidSlashAirAntic.ChangeTransition("FINISHED", RapidSlashAir.name);
            RapidSlashAir.ChangeTransition("FINISHED", SlashEnd.name);
            RapidSlashAir.ChangeTransition("MULTI HIT CONNECT", HeroFacing2.name);
            HeroFacing2.ChangeTransition("FINISHED", MultiHitting2.name);
            MultiHitting2.ChangeTransition("FINISHED", MultihitSlash2.name);
            MultihitSlash2.ChangeTransition("FINISHED", MultihitSlashEnd.name);
            SlashEnd.ChangeTransition("FINISHED", _endStateName);
            MultihitSlashEnd.ChangeTransition("FINISHED", _endStateName);

        }
    }
}
