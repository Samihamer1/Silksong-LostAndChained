using HutongGames.PlayMaker;
using LostAndChained.Phases.Attacks;
using Silksong.FsmUtil;
using System;
using System.Collections.Generic;
using System.Text;

namespace LostAndChained.LacePhases.Attacks
{
    internal class LaceBulletCastAir : BaseAttack
    {
        public LaceBulletCastAir(PlayMakerFSM controlFSM, string EndStateName, string parentAttackString) : base(controlFSM, EndStateName, parentAttackString)
        {
        }

        public override string GetAttackName()
        {
            return "Bullet Cast";
        }

        public override string GetStartStateNamePure()
        {
            return "Bullet Type Air";
        }

        public override void Init()
        {
            FsmState BulletSType = CopyState("Bullet S Type", "Bullet Type Air");
            FsmState BulletSAir = CopyState("Bullet S Air", "Bullet Cast Air");
            FsmState BulletSAir2 = CopyState("Bullet S Air 2", "Bullet Cast Air 2");
            FsmState CastToAir = CopyState("Cast To Air", "Cast To Air 2");

            BulletSType.RemoveTransition("AIR");
            BulletSType.ChangeTransition("FINISHED", BulletSAir.name);
            BulletSAir.ChangeTransition("FINISHED", BulletSAir2.name);
            BulletSAir2.ChangeTransition("FINISHED", CastToAir.name);
            CastToAir.ChangeTransition("FINISHED", _endStateName);
        }
    }
}
