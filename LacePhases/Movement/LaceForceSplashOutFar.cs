using HutongGames.PlayMaker;
using LostAndChained.Components;
using LostAndChained.Phases.Attacks;
using Silksong.FsmUtil;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace LostAndChained.LacePhases.Movement
{
    //Failsafe to ensure she shows up and does the animation.
    internal class LaceForceSplashOutFar : BaseAttack
    {
        private FsmState _configState;
        public LaceForceSplashOutFar(PlayMakerFSM controlFSM, string EndStateName, string parentAttackString) : base(controlFSM, EndStateName, parentAttackString)
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

        public override string GetStartStateNamePure()
        {
            return "Set Teleport Config";
        }

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
                Log.Debug("FORCE SPLASHED OUT");
                LaceBossScene.Instance.LaceMain.DeactivateTeleportTelegraph();
            });
            EmergeTypeState.AddMethod(_ =>
            {
                _controlFSM.GetBoolVariable("Splashed In").value = false;
                _controlFSM.gameObject.GetComponent<MeshRenderer>().enabled = true;
                _controlFSM.gameObject.GetComponent<DamageHero>().enabled = true;

                if (_controlFSM.GetFloatVariable("Tele Y").value > Constants.Constraints.LaceLandY)
                {
                    _controlFSM.SendEvent("AIR");
                }
            });

            FsmState FailSafeState = AddState("Fail Safe Teleport");
            FailSafeState.AddMethod(_ =>
            {
                _controlFSM.GetFloatVariable("Tele Y").value = Constants.Constraints.LaceLandY;
                _controlFSM.GetFloatVariable("Tele X").value = Constants.Constraints.ArenaCenterX;
                _controlFSM.gameObject.transform.position = new Vector2(Constants.Constraints.ArenaCenterX, Constants.Constraints.LaceLandY);
            });

            //transition fixes
            SetTeleportConfigState.AddTransition("FINISHED", CheckTypeState.name);
            SetTeleportConfigState.AddTransition("CANCEL", "Idle Choice");
            CheckTypeState.AddTransition("FINISHED", SelfDistanceState.name);
            SelfDistanceState.ChangeTransition("FINISHED", TeleInitState.name);
            TeleInitState.ChangeTransition("FINISHED", TelePosState.name);
            TeleInitState.RemoveTransition("ABYSS WAVE");
            TelePosState.ChangeTransition("FINISHED", EmergeTypeState.name);
            TelePosState.ChangeTransition("RETRY", RetryCheckState.name);
            RetryCheckState.ChangeTransition("FINISHED", TeleInitState.name);
            RetryCheckState.ChangeTransition("RETRY", TelePosState.name);
            RetryCheckState.ChangeTransition("CANCEL", FailSafeState.name);
            EmergeTypeState.ChangeTransition("GROUND", DiveOutGState.name);
            EmergeTypeState.ChangeTransition("AIR", DiveOutAState.name);
            DiveOutGState.ChangeTransition("FINISHED", _endStateName);
            DiveOutAState.ChangeTransition("FINISHED", DiveOutA2State.name);
            DiveOutA2State.ChangeTransition("FINISHED", _endStateName);
            FailSafeState.AddTransition("FINISHED", EmergeTypeState.name);

            _configState.AddMethod(_ =>
            {
                SetTeleportConfig(new LaceTeleportConfig
                {
                    distanceMin = 6f,
                    distanceMax = 7f,
                    distanceFailMax = 15f,
                    hopStopDistance = 7f,
                    teleY = Constants.Constraints.LaceLandY
                });
            });
        }

        public override string GetAttackName()
        {
            return "Lace Force Splash Out Far";
        }
    }
}
