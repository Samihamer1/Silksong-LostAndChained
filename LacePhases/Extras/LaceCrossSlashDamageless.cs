using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using LostAndChained.Components;
using LostAndChained.Phases.Attacks;
using Silksong.FsmUtil;
using System;
using System.Collections.Generic;
using System.Text;

namespace LostAndChained.LacePhases.Extras
{
    internal class LaceCrossSlashDamageless : BaseAttack
    {
        public LaceCrossSlashDamageless(PlayMakerFSM controlFSM, string EndStateName, string parentAttackString) : base(controlFSM, EndStateName, parentAttackString)
        {
        }

        public override string GetAttackName()
        {
            return "CS Damageless";
        }

        public override string GetStartStateNamePure()
        {
            return "CS Damageless Do Cross Slash";
        }

        public override void Init()
        {
            FsmState DoCrossSlash = CopyState("Do CrossSlash", "CS Damageless Do Cross Slash");
            FsmState CSSlam = CopyState("CS Slam", "CS Damageless Slam");
            FsmState CSFlipBack = CopyState("CS Flip Back", "CS Damageless Flip Back");

            DoCrossSlash.RemoveActionsOfType<ActivateGameObject>();
            DoCrossSlash.RemoveActionsOfType<SendMessageDelay>();
            DoCrossSlash.AddMethod(_ =>
            {
                LaceBossScene.Instance.LaceMain.ActivateAltCrossSlash();
            });

            //CSSlam.RemoveActionsOfType<DamageHeroDirectly>();

            //transitions
            DoCrossSlash.ChangeTransition("FINISHED", CSSlam.name);
            CSSlam.ChangeTransition("FINISHED", CSFlipBack.name);
            CSFlipBack.ChangeTransition("LAND", _endStateName);
        }
    }
}
