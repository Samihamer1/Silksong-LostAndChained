using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using LostAndChained.Components;
using LostAndChained.Phases.Attacks;
using Silksong.FsmUtil;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace LostAndChained.LacePhases.Attacks
{
    internal class LaceRapidStabAir : BaseAttack
    {
        public LaceRapidStabAir(PlayMakerFSM controlFSM, string EndStateName, string parentAttackString) : base(controlFSM, EndStateName, parentAttackString)
        {
        }

        public override string GetAttackName()
        {
            return "Rapid Stab Air";
        }

        public override string GetStartStateNamePure()
        {
            return "Counter TeleOut 2";
        }

        public override void Init()
        {
            FsmState CounterTeleOut = CopyState("Counter TeleOut", "Counter TeleOut 2");
            FsmState CounterTeleIn = CopyState("Counter TeleIn", "Counter TeleIn 2");

            FsmState RapidSlashAirAntic = CopyState("RapidSlashAir Antic", "RapidSlashAir Antic 2");
            FsmState RapidSlashAir = CopyState("RapidSlash Air", "RapidSlash Air 2");
            FsmState SlashEnd = CopyState("Slash End", "Slash End 2");

            FsmState HeroFacing2 = CopyState("Hero Facing 2", "Hero Facing 2-2");
            FsmState MultiHitting2 = CopyState("Multihitting 2", "Multihitting 2-2");
            FsmState MultihitSlash2 = CopyState("Multihit Slash 2", "Multihit Slash 2-2");
            FsmState MultihitSlashEnd = CopyState("Multihit Slash End", "Multihit Slash End-2");

            CounterTeleIn.InsertMethod(0,_ =>
            {
                CounterTeleIn.GetFirstActionOfType<SetPosition>().y = HeroController.instance.transform.position.y + 2.5f;
            });

            RapidSlashAirAntic.AddMethod(_ =>
            {
                LaceBossScene.Instance.LaceMain._bossObject.GetComponent<Rigidbody2D>().SetVelocity(0, 0);
            });

            CounterTeleOut.ChangeTransition("FINISHED", CounterTeleIn.name);
            CounterTeleIn.ChangeTransition("FINISHED", RapidSlashAirAntic.name);
            RapidSlashAirAntic.ChangeTransition("FINISHED", RapidSlashAir.name);
            RapidSlashAir.ChangeTransition("FINISHED", SlashEnd.name);
            RapidSlashAir.ChangeTransition("MULTI HIT CONNECT", HeroFacing2.name);
            HeroFacing2.ChangeTransition("FINISHED", MultiHitting2.name);
            MultiHitting2.ChangeTransition("FINISHED", MultihitSlash2.name);
            MultihitSlash2.ChangeTransition("FINISHED", MultihitSlashEnd.name);
            SlashEnd.ChangeTransition("FINISHED", _endStateName);
            MultihitSlashEnd.ChangeTransition("FINISHED", "Idle Choice");
        }
    }
}
