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
    internal class LaceLand : BaseAttack
    {
        public LaceLand(PlayMakerFSM controlFSM, string EndStateName, string parentAttackString) : base(controlFSM, EndStateName, parentAttackString)
        {
        }

        public override string GetAttackName()
        {
            return "Land";
        }

        public override string GetStartStateNamePure()
        {
            return "Bounce Back Land";
        }

        public override void Init()
        {
            //Dive in states
            FsmState BounceBackState = CopyState("Bounce Back", "Bounce Back Land");
            FsmState LandState = CopyState("Land", "Land 2");

            BounceBackState.AddMethod(_ =>
            {
                _controlFSM.gameObject.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
            });

            //Transition fixes
            BounceBackState.RemoveTransition("TELE");
            BounceBackState.ChangeTransition("LAND", LandState.name);
            LandState.RemoveTransition("TELE");
            LandState.ChangeTransition("FINISHED", _endStateName);
        }
    }
}
