using LostAndChained.LacePhases.Attacks;
using LostAndChained.LacePhases.Awaits;
using LostAndChained.LacePhases.Extras;
using LostAndChained.LacePhases.Movement;
using LostAndChained.LacePhases.Two;
using System;
using System.Collections.Generic;
using System.Text;

namespace LostAndChained.Phases.Attacks
{
    internal static class LaceAttackList
    {
        public static BaseAttackString TendrilIntoDive;
        public static BaseAttackString ChargeIntoDive;
        public static BaseAttackString SplashIn;
        public static BaseAttackString Phase2Shift;

        public static BaseAttackString Counter;
        public static BaseAttackString PullUpCounter;
        public static BaseAttackString SlashChargeTendrilDive;
        public static BaseAttackString PursuerChargeDive;
        public static BaseAttackString JumpslashTendrilDive;
        public static BaseAttackString PullUpRapidStab;
        public static BaseAttackString ChargeJumpslashHold;

        public static BaseAttackString DeflectCharge;
        public static BaseAttackString PullUpDeflectCharge;

        public static BaseAttackString ComboWeb;
        public static BaseAttackString ComboClawShot;
        public static BaseAttackString ComboRubble;

        public static BaseAttackString CrossSlashCutscene;


        public static BaseAttackString SplashInPerma;
        public static BaseAttackString Phase4Shift;

        public static BaseAttackString FastCharge;
        public static BaseAttackString PullUpJumpslashHold;

        public static BaseAttackString FastTripleSlashJumpSlashHold;
        public static BaseAttackString FastWhipHarass;
        public static BaseAttackString CircleSlashes;
        public static BaseAttackString BulletDive;
        public static BaseAttackString JumpSlashRapidStab;

        public static void InitLaceAttackStrings(PlayMakerFSM LaceAttackControl)
        {
            TendrilIntoDive = new BaseAttackString(LaceAttackControl, "HARASS1", new List<Type>
                {
                    typeof(LaceSplashIn),typeof(LaceSplashOutRandom), typeof(LaceTendrilAttack2), typeof(LaceSplashIn)
                });

            ChargeIntoDive = new BaseAttackString(LaceAttackControl, "HARASS2", new List<Type>
            {
                typeof(LaceSplashIn), typeof(LaceSplashOutGroundFar), typeof(LaceSingleCharge), typeof(LaceSplashIn)
            });

            SplashIn = new BaseAttackString(LaceAttackControl, "SPLASHIN", new List<Type> { typeof(LaceSplashIn) });

            Phase2Shift = new BaseAttackString(LaceAttackControl, "PHASE2SHIFT", new List<Type> { typeof(LaceSplashIn), typeof(LaceForceSplashOutFar), typeof(LacePhase2Shift), typeof(LaceSplashIn) });

            SlashChargeTendrilDive = new BaseAttackString(LaceAttackControl, "SLASHCHARGETENDRILDIVE", new List<Type>
            {
                typeof(LaceSplashIn), typeof(LaceSplashOutGroundClose), typeof(LaceTripleSlash), typeof(LaceTendrilAttack2), typeof(LaceLand)
            });

            PursuerChargeDive = new BaseAttackString(LaceAttackControl, "PURSUERCHARGE", new List<Type>
            {
                typeof(LaceSplashIn), typeof(LaceSplashOutGroundFar), typeof(LaceSingleCharge), typeof(LaceSplashIn), typeof(LaceSplashOutGroundClose), typeof(LaceSingleCharge), typeof(LaceSplashIn)
            });

            JumpslashTendrilDive = new BaseAttackString(LaceAttackControl, "JUMPSLASHTENDRILDIVE", new List<Type>
            {
                typeof(LaceSplashIn), typeof(LaceSplashOutGroundClose), typeof(LaceJumpSlash), typeof(LaceTendrilAttack2), typeof(LaceLand)
            });

            Counter = new BaseAttackString(LaceAttackControl, "COUNTER", new List<Type>
            {
                typeof(LaceCounter), typeof(LaceLand)
            });

            DeflectCharge = new BaseAttackString(LaceAttackControl, "DEFLECT", new List<Type>
            {
                typeof(LaceDeflect), typeof(LaceSingleChargeFast)
            });

            PullUpDeflectCharge = new BaseAttackString(LaceAttackControl, "PULLUPDEFLECT", new List<Type>
            {
                typeof(LaceSplashIn), typeof(LaceSplashOutGroundClose), typeof(LaceDeflect), typeof(LaceSingleChargeFast)
            });

            ComboWeb = new BaseAttackString(LaceAttackControl, "COMBOWEB", new List<Type>
            {
                typeof(LaceSplashIn), typeof(LaceAwaitWeb), typeof(LaceForceSplashOutFar), typeof(LaceCircleSlashes), typeof(LaceSplashIn)
            });

            CrossSlashCutscene = new BaseAttackString(LaceAttackControl, "CROSSSLASHCUTSCENE", new List<Type>
            {
                typeof(LaceCrossSlashDamageless), typeof(LaceForceSplashIn)
            });

            PullUpCounter = new BaseAttackString(LaceAttackControl, "PULLUPCOUNTER", new List<Type>
            {
                typeof(LaceSplashIn), typeof(LaceSplashOutGroundClose), typeof(LaceCounter), typeof(LaceLand)
            });

            ComboClawShot = new BaseAttackString(LaceAttackControl, "COMBOCLAWSHOT", new List<Type>
            {
                typeof(LaceSplashIn), typeof(LaceAwaitClaw), typeof(LaceSplashOutAirMiddle), typeof(LaceVomitAir), typeof(LaceSplashIn)
            });

            ComboRubble = new BaseAttackString(LaceAttackControl, "COMBORUBBLE", new List<Type>
            {
                typeof(LaceSplashIn), typeof(LaceAwaitRubble), typeof(LaceSplashOutGroundFar), typeof(LaceSingleCharge), typeof(LaceSplashIn), typeof(LaceSplashOutGroundClose), typeof(LaceJumpSlash), typeof(LaceSplashIn)
            });

            ChargeJumpslashHold = new BaseAttackString(LaceAttackControl, "CHARGEJUMPSLASHHOLD", new List<Type>
            {
                typeof(LaceSplashIn), typeof(LaceSplashOutGroundFar), typeof(LaceSingleCharge), typeof(LaceSplashIn), typeof(LaceSplashOutGroundClose), typeof(LaceJumpSlash), typeof(LaceSplashIn)
            });

            PullUpRapidStab = new BaseAttackString(LaceAttackControl, "PULLUPRAPIDSTAB", new List<Type>
            {
                typeof(LaceSplashIn), typeof(LaceSplashOutGroundFar), typeof(LaceCounterTele), typeof(LaceTendrilAttack2), typeof(LaceLand)
            });

            Phase4Shift = new BaseAttackString(LaceAttackControl, "PHASE4SHIFT", new List<Type>
            {
                typeof(LaceSplashOutP4Shift), typeof(LaceP4Wait), typeof(LaceLaugh),typeof(LaceP4Lunge), typeof(LaceEvade), typeof(LaceP4Parry)
            });

            SplashInPerma = new BaseAttackString(LaceAttackControl, "SPLASHINPERMA", new List<Type>
            {
                typeof(LaceSplashInPerma)
            });

            FastCharge = new BaseAttackString(LaceAttackControl, "FASTCHARGE", new List<Type>
            {
                typeof(LaceSplashIn), typeof(LaceSplashOutGroundClose), typeof(LaceSingleChargeFast), typeof(LaceSplashIn)
            });

            PullUpJumpslashHold = new BaseAttackString(LaceAttackControl, "PULLUPJUMPSLASHHOLD", new List<Type>
            {
                typeof(LaceSplashIn), typeof(LaceSplashOutGroundClose), typeof(LaceJumpSlash), typeof(LaceSplashIn)
            });

            FastTripleSlashJumpSlashHold = new BaseAttackString(LaceAttackControl, "FASTTRIPLESLASHJUMPSLASHHOLD", new List<Type>
            {
                typeof(LaceSplashIn), typeof(LaceSplashOutGroundClose), typeof(LaceTripleSlashFast), typeof(LaceJumpSlash), typeof(LaceSplashIn)
            });

            FastWhipHarass = new BaseAttackString(LaceAttackControl, "FASTWHIPHARASS", new List<Type>
            {
                typeof(LaceSplashIn), typeof(LaceSplashOutRandom), typeof(LaceTendrilAttackFast), typeof(LaceSplashIn)
            });

            CircleSlashes = new BaseAttackString(LaceAttackControl, "CIRCLESLASHES", new List<Type>
            {
                typeof(LaceSplashIn), typeof(LaceSplashOutGroundFar), typeof(LaceCircleSlashes), typeof(LaceSplashIn)
            });

            BulletDive = new BaseAttackString(LaceAttackControl, "BULLETDIVE", new List<Type>
            {
                typeof(LaceSplashIn), typeof(LaceSplashOutAirMiddle), typeof(LaceBulletCastAir), typeof(LaceDownstabFast), typeof(LaceSplashIn)
            });

            JumpSlashRapidStab = new BaseAttackString(LaceAttackControl, "JUMPSLASHRAPIDSTAB", new List<Type>
            {
                typeof(LaceSplashIn), typeof(LaceSplashOutGroundClose), typeof(LaceJumpSlash), typeof(LaceRapidStabAir), typeof(LaceSplashIn)
            });
        }
    }
}
