using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using LostAndChained.Components;
using LostAndChained.Phases.Attacks;
using Silksong.FsmUtil;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace LostAndChained.LacePhases.Movement
{
    internal abstract class LaceBaseSplashOut : BaseAttack
    {
        private FsmState _configState;
        private FsmState _overrideState;
        public LaceBaseSplashOut(PlayMakerFSM controlFSM, string EndStateName, string parentAttackString) : base(controlFSM, EndStateName, parentAttackString)
        {
        }

        public void SetTeleportConfig(LaceTeleportConfig config)
        {
            _controlFSM.GetFloatVariable("Distance Min").value = config.distanceMin;
            _controlFSM.GetFloatVariable("Distance Max").value = config.distanceMax;
            _controlFSM.GetFloatVariable("Distance Fail Max").value = config.distanceFailMax;
            _controlFSM.GetFloatVariable("Hop Stop Distance").value = config.hopStopDistance;
            _controlFSM.GetFloatVariable("Tele Y").value = config.teleY;
        }

        public FsmState GetConfigState()
        {
            return _configState;
        }

        public FsmState GetOverrideState()
        {
            return _overrideState;
        }

        public override string GetStartStateNamePure()
        {
            return "Set Teleport Config";
        }

        public abstract void PostInit();

        public override void Init()
        {
            //Types
            FsmState SetTeleportConfigState = AddState("Set Teleport Config");
            _configState = SetTeleportConfigState;

            FsmState CheckTypeState = AddState("Check Teleport Type");

            //Dive out states
            FsmState SelfDistanceState = CopyState("Set Self Distance", "Self Distance-2");
            FsmState TeleInitState = CopyState("Tele Init", "Tele Init-2");
            FsmState TelePosState = CopyState("Tele Pos", "Tele Pos-2");
            FsmState RetryCheckState = CopyState("Retry Check", "Retry Check-2");
            FsmState OverrideState = AddState("Override Check");
            _overrideState = OverrideState;
            FsmState TelegraphState = AddState("Tele Telegraph");
            FsmState EmergeTypeState = CopyState("Emerge Type", "Emerge Type-2");
            FsmState DiveOutGState = CopyState("Dive Out G", "Dive Out G-2");
            FsmState DiveOutAState = CopyState("Dive Out A", "Dive Out A-2");
            FsmState DiveOutA2State = CopyState("Dive Out A 2", "Dive Out A2-2");

            SetTeleportConfigState.AddMethod(_ => {
				if (!_controlFSM.GetBoolVariable("Splashed In").value)
				{
					_controlFSM.SendEvent("CANCEL");
				}
			});

            //height
            EmergeTypeState.InsertMethod(0, _ =>
            {
                _controlFSM.GetBoolVariable("Splashed In").value = false;
                _controlFSM.gameObject.GetComponent<MeshRenderer>().enabled = true;
                _controlFSM.gameObject.GetComponent<DamageHero>().enabled = true;
                Log.Debug("SPLASHED OUT");
                LaceBossScene.Instance.LaceMain.DeactivateTeleportTelegraph();
            });

            TelegraphState.AddMethod(_ =>
            {
                LaceBossScene.Instance.LaceMain.ActivateTeleportTelegraph(new Vector3(_controlFSM.GetFloatVariable("Tele X").value, Constants.Constraints.LaceLandY,0));
            });
            TelegraphState.AddAction(new Wait() { time = 0.5f, realTime = false, finishEvent = FsmEvent.GetFsmEvent("FINISHED") });

            //remove early setting of splash in
            TeleInitState.RemoveAction(0); //SetBoolValue (Splashed In) false


            //transition fixes
            SetTeleportConfigState.AddTransition("FINISHED", CheckTypeState.name);
            SetTeleportConfigState.AddTransition("CANCEL", "Idle Choice");
            CheckTypeState.AddTransition("FINISHED", SelfDistanceState.name);
            SelfDistanceState.ChangeTransition("FINISHED", TeleInitState.name);
            TeleInitState.ChangeTransition("FINISHED", TelePosState.name);
            TeleInitState.RemoveTransition("ABYSS WAVE");
            TelePosState.ChangeTransition("FINISHED", OverrideState.name);
            TelePosState.ChangeTransition("RETRY", RetryCheckState.name);
            OverrideState.AddTransition("FINISHED", TelegraphState.name);
            TelegraphState.AddTransition("FINISHED", EmergeTypeState.name);
            RetryCheckState.ChangeTransition("FINISHED", TeleInitState.name);
            RetryCheckState.ChangeTransition("RETRY", TelePosState.name);
            RetryCheckState.ChangeTransition("CANCEL", "Idle Choice");
            EmergeTypeState.ChangeTransition("GROUND", DiveOutGState.name);
            EmergeTypeState.ChangeTransition("AIR", DiveOutAState.name);
            DiveOutGState.ChangeTransition("FINISHED", _endStateName);
            DiveOutAState.ChangeTransition("FINISHED", DiveOutA2State.name);
            DiveOutA2State.ChangeTransition("FINISHED", _endStateName);

            PostInit();
        }

    }
}
