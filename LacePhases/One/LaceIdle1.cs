using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using LostAndChained.LacePhases.Attacks;
using LostAndChained.Phases.Attacks;
using LostAndChained.Phases.One;
using Silksong.FsmUtil;
using System;
using System.Collections.Generic;
using System.Text;
using static LostAndChained.Constants;

namespace LostAndChained.LacePhases.One
{
    internal class LaceIdle1 : BasePhase
    {
        public LaceIdle1(PlayMakerFSM controlFSM) : base(controlFSM)
        {
        }

        public override string GetControlStateName()
        {
            return PhaseNames.Idle1;
        }

        public override void Init()
        {
            FsmState attackSelectState = _controlFSM.AddState(PhaseNames.Idle1 + " Attack");
            attackSelectState.AddMethod(_ =>
            {               
                if (UnityEngine.Random.Range(0, 2) == 0)
                {
                    _controlFSM.SendEvent("TENDRIL");
                }
                _controlFSM.SendEvent("CHARGE");
            });
            attackSelectState.AddTransition("TENDRIL", LaceAttackList.TendrilIntoDive.GetStartStateName());
            attackSelectState.AddTransition("CHARGE", LaceAttackList.ChargeIntoDive.GetStartStateName());

            //Control state
            FsmState controlState = _controlFSM.AddState(PhaseNames.Idle1);
            controlState.AddTransition("SPLASHIN", LaceAttackList.SplashIn.GetStartStateName());

            controlState.AddMethod(_ =>
            {
                _controlFSM.GetBoolVariable("Can Tendril Emerge").value = false;
                _controlFSM.GetBoolVariable("Can Hop Up").value = false;
                _controlFSM.GetBoolVariable("Did Thread Roar").value = false;

                if (!_controlFSM.GetBoolVariable("Splashed In").value)
                {
                    _controlFSM.SendEvent("SPLASHIN");
                }
            });

            controlState.AddAction(new Wait
            {
                realTime = false,
                finishEvent = FsmEvent.GetFsmEvent("ATTACK"),
                time = 2.5f
            });

            //transitions
            controlState.AddTransition("ATTACK", attackSelectState.name);
        }
    }
}
