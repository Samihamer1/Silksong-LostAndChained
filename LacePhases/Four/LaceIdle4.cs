using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using LostAndChained.Components;
using LostAndChained.Phases.Attacks;
using LostAndChained.Phases.One;
using Silksong.FsmUtil;
using System;
using System.Collections.Generic;
using System.Text;
using static LostAndChained.Constants;

namespace LostAndChained.LacePhases._4
{
    internal class LaceIdle4 : BasePhase
    {
        public LaceIdle4(PlayMakerFSM controlFSM) : base(controlFSM)
        {
        }

        public override string GetControlStateName()
        {
            return Constants.PhaseNames.Idle4;
        }

        public override void Init()
        {
            bool activateTransition = true;

            //Attack select
            FsmState attackSelectState = _controlFSM.AddState(PhaseNames.Idle4 + " Attack");
            attackSelectState.AddAction(new SendRandomEventV4
            {
                events = new[] {
                    FsmEvent.GetFsmEvent("JUMPSLASHHOLD"),
                    FsmEvent.GetFsmEvent("CHARGE"),
                    FsmEvent.GetFsmEvent("PULLUPDEFLECT"),
                    FsmEvent.GetFsmEvent("SLASHCOMBO"),
                    FsmEvent.GetFsmEvent("TENDRILHARASS"),
                    FsmEvent.GetFsmEvent("CIRCLESLASHES"),
                    FsmEvent.GetFsmEvent("BULLETDIVE"),
                    FsmEvent.GetFsmEvent("JUMPSLASHSTAB")
                },
                weights = new[]
                {
                    new FsmFloat(1f),
                    new FsmFloat(1f),
                    new FsmFloat(1f),
                    new FsmFloat(1f),
                    new FsmFloat(1f),
                    new FsmFloat(1f),
                    new FsmFloat(1f),
                    new FsmFloat(1f)
                },
                eventMax = new[]
                {
                    new FsmInt(2),
                    new FsmInt(2),
                    new FsmInt(2),
                    new FsmInt(1),
                    new FsmInt(2),
                    new FsmInt(1),
                    new FsmInt(2),
                    new FsmInt(2)
                },
                missedMax = new[]
                {
                    new FsmInt(8),
                    new FsmInt(8),
                    new FsmInt(8),
                    new FsmInt(8),
                    new FsmInt(8),
                    new FsmInt(8),
                    new FsmInt(8),
                    new FsmInt(8)
                },
                activeBool = true
            });
            attackSelectState.AddTransition("JUMPSLASHHOLD", LaceAttackList.PullUpJumpslashHold.GetStartStateName());
            attackSelectState.AddTransition("CHARGE", LaceAttackList.FastCharge.GetStartStateName());
            attackSelectState.AddTransition("PULLUPDEFLECT", LaceAttackList.PullUpDeflectCharge.GetStartStateName());
            attackSelectState.AddTransition("SLASHCOMBO", LaceAttackList.FastTripleSlashJumpSlashHold.GetStartStateName());
            attackSelectState.AddTransition("TENDRILHARASS", LaceAttackList.FastWhipHarass.GetStartStateName());
            attackSelectState.AddTransition("CIRCLESLASHES", LaceAttackList.CircleSlashes.GetStartStateName());
            attackSelectState.AddTransition("BULLETDIVE", LaceAttackList.BulletDive.GetStartStateName());
            attackSelectState.AddTransition("JUMPSLASHSTAB", LaceAttackList.JumpSlashRapidStab.GetStartStateName());

            //Control state
            FsmState controlState = _controlFSM.CopyState("Idle", PhaseNames.Idle4);
            controlState.RemoveAction(0); //IMPORTANT HOTFIX (that i probably wont make cleaner) since we're copying idle which already had an inserted method

            controlState.RemoveFirstActionOfType<Tk2dPlayAnimation>();

            controlState.InsertMethod(0, _ =>
            {
                _controlFSM.GetBoolVariable("Will Counter").value = false;
                //initial
                if (activateTransition)
                {
                    activateTransition = false;
                    LaceBossScene.Instance.phaseActive = true;
                    _controlFSM.gameObject.GetComponent<HealthManager>().hp = Constants.PhaseValues.Phase4HP;
                }

                if (!_controlFSM.GetBoolVariable("Splashed In").value)
                {
                    _controlFSM.gameObject.GetComponent<tk2dSpriteAnimator>().Play("Idle");

                    if (UnityEngine.Random.Range(0, 3) == 0)
                    {
                        _controlFSM.GetBoolVariable("Will Counter").value = true;
                    }
                }
            });

            controlState.AddAction(new Wait
            {
                realTime = false,
                finishEvent = FsmEvent.GetFsmEvent("ATTACK"),
                time = 0.2f
            });

            //transitions
            controlState.ChangeTransition("ATTACK", attackSelectState.name);
            controlState.ChangeTransition("COUNTER", LaceAttackList.DeflectCharge.GetStartStateName());
            controlState.ChangeTransition("TOOK DAMAGE", attackSelectState.name);
            controlState.RemoveTransition("EVADE");
        }
    }
}

