using HutongGames.PlayMaker;
using LostAndChained.Phases.Attacks;
using Silksong.FsmUtil;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace LostAndChained.LacePhases.Extras
{
    internal class LaceSplashInPerma : BaseAttack
    {
        public LaceSplashInPerma(PlayMakerFSM controlFSM, string EndStateName, string parentAttackString) : base(controlFSM, EndStateName, parentAttackString)
        {
        }

        public override string GetAttackName()
        {
            return "Splash In Perma";
        }

        public override string GetStartStateNamePure()
        {
            return "Bounce Back 2";
        }

        public override void Init()
        {
            //Dive in states
            FsmState BounceBackState = CopyState("Bounce Back", "Bounce Back 2");
            FsmState DiveIn1State = CopyState("Dive In 1", "Dive In 1 2");
            FsmState DiveIn2State = CopyState("Dive In 2", "Dive In 2 2");
            DiveIn2State.AddMethod(_ =>
            {
                _controlFSM.GetBoolVariable("Force Tele").value = true;
                _controlFSM.GetBoolVariable("Splashed In").value = true;

                Log.Debug("SPLASHED IN");
            });

            BounceBackState.InsertMethod(0, _ =>
            {
                _controlFSM.gameObject.GetComponent<Rigidbody2D>().SetVelocity(0, 0);
                _controlFSM.gameObject.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
                if (_controlFSM.GetBoolVariable("Splashed In").value)
                {
                    _controlFSM.SendEvent("CANCEL");
                }
            });

            FsmState permaWait = AddState("Perma Wait");


            //Transition fixes
            BounceBackState.RemoveTransition("TELE");
            BounceBackState.ChangeTransition("LAND", DiveIn1State.name);
            BounceBackState.AddTransition("CANCEL", permaWait.name);
            DiveIn1State.ChangeTransition("FINISHED", DiveIn2State.name);
            DiveIn2State.ChangeTransition("FINISHED", permaWait.name);
        }
    }
}

