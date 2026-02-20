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
    internal class LaceBlackThreads : BaseAttack
    {
        private static float lastUsedTime = 0;
        private static float useCooldown = 8;

        public static bool OnCooldown()
        {
            return Time.time - lastUsedTime < useCooldown;
        }

        public static bool PreventCircles()
        {
            return Time.time - lastUsedTime <= 1.5f;
        }

        public LaceBlackThreads(PlayMakerFSM controlFSM, string EndStateName, string parentAttackString) : base(controlFSM, EndStateName, parentAttackString)
        {
        }

        public override string GetAttackName()
        {
            return "Black Threads";
        }

        public override string GetStartStateNamePure()
        {
            return "Threads Summon";
        }

        public override void Init()
        {
            FsmState summonState = AddState("Threads Summon");
            summonState.AddMethod(_ =>
            {
                if (!OnCooldown())
                {
                    lastUsedTime = Time.time;
                    LaceBossScene.Instance.LaceMain.ActivateBlackThreads();
                }
            });

            summonState.AddTransition("FINISHED", _endStateName);
        }
    }
}
