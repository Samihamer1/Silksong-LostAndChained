using HutongGames.PlayMaker;
using LostAndChained.Components;
using LostAndChained.Phases.Attacks;
using Silksong.FsmUtil;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace LostAndChained.LacePhases.Attacks
{
    internal class LaceCircleSlashes : BaseAttack
    {
        public LaceCircleSlashes(PlayMakerFSM controlFSM, string EndStateName, string parentAttackString) : base(controlFSM, EndStateName, parentAttackString)
        {
        }

        public override string GetAttackName()
        {
            return "Circle Slashes";
        }

        public override string GetStartStateNamePure()
        {
            return "Circle Slashes Antic";
        }

        public override void Init()
        {
            FsmState Antic = CopyState("CS Antic", "Circle Slashes Antic");
            FsmState Summon = CopyState("CS Summon", "Circle Slashes Summon");
            FsmState ThwhipOut = CopyState("CS Thwip Out", "Circle Slashes Thwhip Out");
            FsmState StompAntic = CopyState("Stomp Antic", "Circle Slashes Stomp Antic");
            FsmState Stomp = CopyState("Stomp", "Circle Slashes Stomp");
            FsmState Slam = CopyState("Slam", "Circle Slashes Slam");



            //Transitions
            Antic.ChangeTransition("FINISHED", Summon.name);
            Summon.ChangeTransition("FINISHED", ThwhipOut.name);
            ThwhipOut.ChangeTransition("NEXT", StompAntic.name);
            StompAntic.ChangeTransition("FINISHED", Stomp.name);
            Stomp.ChangeTransition("LAND", Slam.name);
            Slam.AddTransition("FINISHED", _endStateName);
        }
    }
}
