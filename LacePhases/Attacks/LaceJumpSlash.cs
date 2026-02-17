using HutongGames.PlayMaker;
using LostAndChained.Phases.Attacks;
using Silksong.FsmUtil;
using System;
using System.Collections.Generic;
using System.Text;

namespace LostAndChained.LacePhases.Attacks
{
    internal class LaceJumpSlash : BaseAttack
    {
        public LaceJumpSlash(PlayMakerFSM controlFSM, string EndStateName, string parentAttackString) : base(controlFSM, EndStateName, parentAttackString)
        {
        }

        public override string GetAttackName()
        {
            return "Jump Slash";
        }

        public override string GetStartStateNamePure()
        {
            return "Jump Slash Antic";
        }

        public override void Init()
        {
            FsmState JSlashAntic = CopyState("J Slash M Antic", "Jump Slash Antic");
            FsmState JSlashMulti = CopyState("J Slash Multi", "Jump Slash Multi");
            FsmState JSlashFollowUp = CopyState("J Slash Followup", "Jump Slash Followup");

            FsmState JSlashMultiHit = CopyState("J Slash Multihit", "Jump Slash Multihit");
            FsmState JSlashStrike = CopyState("J Slash Strike", "Jump Slash Strike");
            FsmState MultiHitEnd = CopyState("Multihit Slash End", "Jump Slash Multihit End");

            FsmState BounceBackState = CopyState("Bounce Back", "Bounce Back Land");
            FsmState LandState = CopyState("Land", "Land 2");

            JSlashAntic.ChangeTransition("NEXT", JSlashMulti.name);
            JSlashMulti.ChangeTransition("FINISHED", JSlashFollowUp.name);
            JSlashMulti.ChangeTransition("MULTI HIT CONNECT", JSlashMultiHit.name);
            JSlashMultiHit.ChangeTransition("FINISHED", JSlashStrike.name);
            JSlashStrike.ChangeTransition("FINISHED", MultiHitEnd.name);
            MultiHitEnd.ChangeTransition("FINISHED", BounceBackState.name);
            JSlashFollowUp.ChangeTransition("FINISHED", _endStateName);
            JSlashFollowUp.RemoveTransition("TENDRIL");

            BounceBackState.RemoveTransition("TELE");
            BounceBackState.ChangeTransition("LAND", LandState.name);
            LandState.RemoveTransition("TELE");
            LandState.ChangeTransition("FINISHED", "Idle Choice");

        }
    }
}
