using HutongGames.PlayMaker;
using LostAndChained.Components;
using LostAndChained.Phases.Attacks;
using Silksong.FsmUtil;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace LostAndChained.LacePhases.Movement
{
    internal class LaceSplashIn : BaseAttack
    {
        public LaceSplashIn(PlayMakerFSM controlFSM, string EndStateName, string parentAttackString) : base(controlFSM, EndStateName, parentAttackString)
        {
        }

        public override string GetAttackName()
        {
            return "Splash In";
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
            FsmState InvisibleState = AddState("Dive In 3");

            DiveIn2State.AddMethod(_ =>
            {
                _controlFSM.GetBoolVariable("Force Tele").value = true;
                _controlFSM.GetBoolVariable("Splashed In").value = true;
                _controlFSM.gameObject.GetComponent<DamageHero>().enabled = false;

                Log.Debug("SPLASHED IN");
            });

            BounceBackState.InsertMethod(0,_ =>
            {
                if (_controlFSM.GetBoolVariable("Splashed In").value)
                {
                    _controlFSM.SendEvent("CANCEL");
                }
                _controlFSM.gameObject.GetComponent<Rigidbody2D>().SetVelocity(0, 0);
                _controlFSM.gameObject.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
            });

            InvisibleState.AddMethod(_ =>
            {
                _controlFSM.gameObject.GetComponent<MeshRenderer>().enabled = false;
            });

            //Transition fixes
            BounceBackState.RemoveTransition("TELE");
            BounceBackState.ChangeTransition("LAND", DiveIn1State.name);
            BounceBackState.AddTransition("CANCEL", _endStateName);
            DiveIn1State.ChangeTransition("FINISHED", DiveIn2State.name);
            DiveIn2State.ChangeTransition("FINISHED", InvisibleState.name);
            InvisibleState.AddTransition("FINISHED", _endStateName);
        }
    }
}
