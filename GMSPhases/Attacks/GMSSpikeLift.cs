using HutongGames.PlayMaker;
using LostAndChained.Phases.Attacks;
using Silksong.FsmUtil;
using System;
using System.Collections.Generic;
using System.Text;

namespace LostAndChained.GMSPhases.Attacks
{
    internal class GMSSpikeLift : BaseAttack
    {
        public GMSSpikeLift(PlayMakerFSM controlFSM, string EndStateName, string parentAttackString) : base(controlFSM, EndStateName, parentAttackString)
        {
        }

        public override string GetAttackName()
        {
            return "Spike Lift";
        }

        public override string GetStartStateNamePure()
        {
            return "Spike Lift Type 2";
        }

        public override void Init()
        {
            FsmState SpikeLiftType = CopyState("Spike Lift Type", "Spike Lift Type 2");
            FsmState SpikeLiftAim = CopyState("Spike Lift Aim", "Spike Lift Aim 2-1");
            FsmState SpikeLiftAim2 = CopyState("Spike Lift Aim 2", "Spike Lift Aim 2-2");
            FsmState SpikeLiftAim3 = CopyState("Spike Lift Aim 2", "Spike Lift Aim 2-3");   

            SpikeLiftType.RemoveTransition("AIM");
            SpikeLiftType.RemoveTransition("INTRO");
            SpikeLiftType.AddTransition("FINISHED", SpikeLiftAim.name);
            SpikeLiftAim.ChangeTransition("FINISHED", SpikeLiftAim2.name);
            SpikeLiftAim2.ChangeTransition("FINISHED", SpikeLiftAim3.name);
            SpikeLiftAim3.ChangeTransition("FINISHED", _endStateName);
        }
    }
}
