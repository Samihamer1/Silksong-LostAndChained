using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using LostAndChained.Phases.Attacks;
using Silksong.FsmUtil;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace LostAndChained.LacePhases.Movement
{
    internal class LaceCounterTele : BaseAttack
    {
        public LaceCounterTele(PlayMakerFSM controlFSM, string EndStateName, string parentAttackString) : base(controlFSM, EndStateName, parentAttackString)
        {
        }

        public override string GetAttackName()
        {
            return "Counter Tele";
        }

        public override string GetStartStateNamePure()
        {
            return "Counter TeleOut 2";
        }

        public override void Init()
        {
            FsmState CounterTeleOut = CopyState("Counter TeleOut", "Counter TeleOut 2");
            FsmState CounterTeleIn = CopyState("Counter TeleIn", "Counter TeleIn 2");
            FsmState PostState = AddState("Counter Tele Post");

            CounterTeleIn.InsertMethod(0, _ =>
            {
                CounterTeleIn.GetFirstActionOfType<SetPosition>().y = HeroController.instance.transform.position.y + 2.5f;
            });

            PostState.AddMethod(_=>
            {
                _controlFSM.gameObject.layer = 11;
                _controlFSM.gameObject.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
            });

            CounterTeleOut.ChangeTransition("FINISHED", CounterTeleIn.name);
            CounterTeleIn.ChangeTransition("FINISHED", _endStateName);
        }
    }
}
