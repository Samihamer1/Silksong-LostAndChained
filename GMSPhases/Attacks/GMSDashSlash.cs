using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using Silksong.FsmUtil;
using System;
using System.Collections.Generic;
using System.Text;
using static LostAndChained.Constants;

namespace LostAndChained.Phases.Attacks
{
    internal class GMSDashSlash : BaseAttack
    {
        public GMSDashSlash(PlayMakerFSM controlFSM, string EndStateName, string parentAttackString) : base(controlFSM, EndStateName, parentAttackString)
        {
        }

        public override string GetAttackName()
        {
            return "Dash Slash";
        }

        public override string GetStartStateNamePure()
        {
            return "DashSlash Prepare";
        }

        public override void Init()
        {
            FsmState PrepareState = CopyState("DashAttack Prepare", "DashSlash Prepare");
            FsmState DirState = CopyState("DashAttack Dir", "DashSlash Dir");
            FsmState DashingLState = CopyState("Dashing L Antic", "DashSlash Antic L");
            FsmState DashingRState = CopyState("Dashing R Antic", "DashSlash Antic R");
            FsmState SlashAnticState = CopyState("Dash Attack Antic", "DashSlash Antic");
            FsmState SlashState = CopyState("Dash Attack", "DashSlash");
            FsmState SlashEndState = CopyState("Dash Attack End", "DashSlash End");
            FsmState MoveRestartState = CopyState("Move Restart 3", "DashSlash Move Restart");

            //transition fixes
            PrepareState.ChangeTransition("ATTACK PREPARED", DirState.Name);
            DirState.ChangeTransition("HERO L", DashingLState.Name);
            DirState.ChangeTransition("HERO R", DashingRState.Name);
            DashingLState.ChangeTransition("FINISHED", SlashAnticState.Name);
            DashingRState.ChangeTransition("FINISHED", SlashAnticState.Name);
            SlashAnticState.ChangeTransition("FINISHED", SlashState.Name);
            SlashState.ChangeTransition("END", SlashEndState.Name);
            SlashEndState.ChangeTransition("FINISHED", MoveRestartState.Name);
            MoveRestartState.ChangeTransition("FINISHED", _endStateName);

            //modifications
            SlashAnticState.GetAction<FloatClamp>(3).minValue = Constraints.GMSMinX;
            SlashAnticState.GetAction<FloatClamp>(3).maxValue = Constraints.GMSMaxX;
            SlashAnticState.GetFirstActionOfType<AnimateYPositionTo>().ToValue = Constraints.ArenaCenterY + 2f;

            SlashState.GetFirstActionOfType<AnimateYPositionTo>().ToValue = Constraints.ArenaCenterY -1f;
            SlashState.GetAction<CheckXPosition>(7).compareTo = Constraints.GMSMinX;
            SlashState.GetAction<CheckXPosition>(8).compareTo = Constraints.GMSMaxX;

            SlashEndState.GetAction<SetPosition2d>(0).y = Constraints.ArenaCenterY - 1f;
            SlashEndState.GetFirstActionOfType<AnimateYPositionTo>().ToValue = Constraints.ArenaCenterY;
        }
    }
}
