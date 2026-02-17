using HutongGames.PlayMaker;
using LostAndChained.Phases.Attacks;
using Silksong.FsmUtil;
using System;
using System.Collections.Generic;
using System.Text;

namespace LostAndChained.LacePhases.Attacks
{
    internal class LaceDownstabFast : BaseAttack
    {
        public LaceDownstabFast(PlayMakerFSM controlFSM, string EndStateName, string parentAttackString) : base(controlFSM, EndStateName, parentAttackString)
        {
        }

        public override string GetAttackName()
        {
            return "Downstab Fast";
        }

        public override string GetStartStateNamePure()
        {
            return "Downstab Antic Fast";
        }

        public override void Init()
        {
            FsmState DownstabAntic = CopyState("Downstab Antic", "Downstab Antic Fast");
            FsmState DstabAngle = CopyState("Dstab Angle", "Dstab Angle Fast");
            FsmState Downstab = CopyState("Downstab", "Downstab Fast");
            FsmState DownstabLand = CopyState("Downstab Land", "Downstab Land Fast");
            FsmState DstabMultihit = CopyState("Dstab Multihit", "Dstab Multihit Fast");
            FsmState DstabStrike = CopyState("Dstab Strike", "Dstab Strike Fast");
            FsmState MultihitSlashEnd = CopyState("Multihit Slash End", "Dstab Multihit End");

            DownstabAntic.ChangeTransition("FINISHED", DstabAngle.name);
            DstabAngle.ChangeTransition("FINISHED", Downstab.name);
            Downstab.RemoveTransition("WALL");
            Downstab.ChangeTransition("LAND", DownstabLand.name);
            Downstab.ChangeTransition("MULTI HIT CONNECT", DstabMultihit.name);
            DownstabLand.ChangeTransition("FINISHED", _endStateName);
            DstabMultihit.ChangeTransition("FINISHED", DstabStrike.name);
            DstabStrike.ChangeTransition("FINISHED", MultihitSlashEnd.name);
            MultihitSlashEnd.ChangeTransition("FINISHED", _endStateName);
        }
    }
}
