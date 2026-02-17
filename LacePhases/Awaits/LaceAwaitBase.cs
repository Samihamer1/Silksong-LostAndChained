using HutongGames.PlayMaker;
using LostAndChained.Components;
using LostAndChained.Phases.Attacks;
using Silksong.FsmUtil;
using System;
using System.Collections.Generic;
using System.Text;

namespace LostAndChained.LacePhases.Awaits
{
    internal abstract class LaceAwaitBase : BaseAttack
    {
        public LaceAwaitBase(PlayMakerFSM controlFSM, string EndStateName, string parentAttackString) : base(controlFSM, EndStateName, parentAttackString)
        {
        }

        public override string GetStartStateNamePure()
        {
            return "Await GMS";
        }

        public override void Init()
        {
            FsmState awaitState = AddState("Await GMS");
            awaitState.AddMethod(_ =>
            {
                LaceBossScene.Instance.laceAttemptingCombo = true;
            });

            FsmState endState = AddState("End Await GMS");
            endState.AddMethod(_ => { 
                LaceBossScene.Instance.laceAttemptingCombo = false;
                LaceBossScene.Instance.GMSMain._bossObject.LocateMyFSM("Attack Control").SendEvent(GetAttackName());
            });

            awaitState.AddTransition("READY", endState.name);
            endState.AddTransition("FINISHED", _endStateName);
        }
    }
}
