using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using LostAndChained.Phases.Attacks;
using Silksong.FsmUtil;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace LostAndChained.GMSPhases.Attacks
{
    internal class GMSDoubleClaw : BaseAttack
    {
        public GMSDoubleClaw(PlayMakerFSM controlFSM, string EndStateName, string parentAttackString) : base(controlFSM, EndStateName, parentAttackString)
        {
        }

        public override string GetAttackName()
        {
            return "Double Claw";
        }

        public override string GetStartStateNamePure()
        {
            return "Set Primary Hand 2";
        }

        public override void Init()
        {
            FsmState SetPrimaryHand2 = CopyState("Set Primary Hand", "Set Primary Hand 2");
            FsmState Claw = AddState("Claw");
            FsmState WaitState = CopyState("Wait For Hands Ready", "Wait For Claws End");

            //Gonna hack this one in since i don't like adding global transitions. they usually are weird
            Claw.AddMethod(_ =>
            {
                GameManager.instance.StartCoroutine(ClawAttack(Claw.name));
            });
            Claw.AddAction(new Wait() { time = 1.5f, realTime = false, finishEvent = FsmEvent.GetFsmEvent("FINISHED") });

            SetPrimaryHand2.ChangeTransition("FINISHED", Claw.name);
            Claw.AddTransition("FINISHED", WaitState.name);
            WaitState.ChangeTransition("FINISHED", _endStateName);
        }

        private IEnumerator ClawAttack(string stateName)
        {
            _controlFSM.GetGameObjectVariable("Primary Hand").value.LocateMyFSM("Hand Control").SetState("Claw Dir");
            yield return new WaitForSeconds(0.75f);
            if (_controlFSM.ActiveStateName != stateName)
            {
                yield break;
            }
            _controlFSM.GetGameObjectVariable("Secondary Hand").value.LocateMyFSM("Hand Control").SetState("Claw Dir");
        }
    }
}
