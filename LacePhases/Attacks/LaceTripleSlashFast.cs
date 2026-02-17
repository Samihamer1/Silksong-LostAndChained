using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using LostAndChained.Phases.Attacks;
using Silksong.FsmUtil;
using System;
using System.Collections.Generic;
using System.Text;

namespace LostAndChained.LacePhases.Attacks
{
    internal class LaceTripleSlashFast : BaseAttack
    {
        public LaceTripleSlashFast(PlayMakerFSM controlFSM, string EndStateName, string parentAttackString) : base(controlFSM, EndStateName, parentAttackString)
        {
        }

        public override string GetAttackName()
        {
            return "Triple Slash Fast";
        }

        public override string GetStartStateNamePure()
        {
            return "TripleSlash 1";
        }

        public override void Init()
        {
            FsmState TripleSlash1 = CopyState("ComboSlash 1", "TripleSlash 1");
            FsmState TripleSlash2 = CopyState("ComboSlash 2", "TripleSlash 2");
            FsmState TripleSlash3 = CopyState("ComboSlash 3", "TripleSlash 3");
            FsmState TripleSlash4 = CopyState("ComboSlash 4", "TripleSlash 4");
            FsmState TripleSlash5 = CopyState("ComboSlash 5", "TripleSlash 5");
            FsmState TripleSlash6 = CopyState("ComboSlash 6", "TripleSlash 6");
            FsmState TripleSlash7 = CopyState("ComboSlash 7", "TripleSlash 7");

            FsmState ComboStrike1 = CopyState("Combo Strike 1", "Combo Strike 1-2");
            FsmState ComboStrike2 = CopyState("Combo Strike 2", "Combo Strike 2-2");
            FsmState ComboStrike = CopyState("Combo Strike", "Combo Strike 2");
            FsmState ComboStrikeFinisher = CopyState("Combo Strike Finisher", "Combo Strike Finisher 2");

            TripleSlash1.GetFirstActionOfType<Tk2dPlayAnimationWithEvents>().clipName = "Combo Slash Triple Fast";

            TripleSlash1.ChangeTransition("FINISHED", TripleSlash2.name);
            TripleSlash2.ChangeTransition("FINISHED", TripleSlash3.name);
            TripleSlash2.ChangeTransition("MULTI HIT CONNECT", ComboStrike1.name);
            TripleSlash3.ChangeTransition("FINISHED", TripleSlash4.name);
            TripleSlash3.ChangeTransition("MULTI HIT CONNECT", ComboStrike2.name);
            TripleSlash4.ChangeTransition("FINISHED", TripleSlash5.name);
            TripleSlash4.ChangeTransition("MULTI HIT CONNECT", ComboStrike2.name);
            TripleSlash5.ChangeTransition("FINISHED", TripleSlash6.name);
            TripleSlash5.ChangeTransition("MULTI HIT CONNECT", ComboStrike1.name);
            TripleSlash6.ChangeTransition("FINISHED", TripleSlash7.name);
            TripleSlash6.ChangeTransition("MULTI HIT CONNECT", ComboStrike1.name);
            TripleSlash7.ChangeTransition("FINISHED", _endStateName);

            ComboStrike1.ChangeTransition("FINISHED", ComboStrike.name);
            ComboStrike2.ChangeTransition("FINISHED", ComboStrike.name);
            ComboStrike.ChangeTransition("FINISHED", ComboStrikeFinisher.name);
            ComboStrikeFinisher.ChangeTransition("FINISHED", "Idle Choice");

        }
    }
}
