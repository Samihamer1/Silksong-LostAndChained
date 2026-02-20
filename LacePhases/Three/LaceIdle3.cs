using GlobalEnums;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using LostAndChained.Components;
using LostAndChained.Phases.Attacks;
using LostAndChained.Phases.One;
using Silksong.FsmUtil;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using static LostAndChained.Constants;

namespace LostAndChained.LacePhases.Three
{
    internal class LaceIdle3 : BasePhase
    {
        public LaceIdle3(PlayMakerFSM controlFSM) : base(controlFSM)
        {
        }

        public override string GetControlStateName()
        {
            return Constants.PhaseNames.Idle3;
        }

        public override void Init()
        {
            bool activateTransition = true;

            FsmState attackSelectState = _controlFSM.AddState(PhaseNames.Idle3 + " Attack");
            FsmState controlState = _controlFSM.CopyState("Idle", PhaseNames.Idle3);
            FsmState comboAttackState = _controlFSM.AddState(PhaseNames.Idle3 + " Combo Attack Choice");


            //Attack select
            //height snipe
            attackSelectState.AddMethod(_ =>
            {
                if (HeroController.instance.transform.position.y >= 16f)
                {
                    _controlFSM.SendEvent("HEIGHTSNIPE");
                }
            });

            attackSelectState.AddAction(new SendRandomEventV4
            {
                events = new[] {
                    FsmEvent.GetFsmEvent("JUMPSLASHCOMBO"),
                    FsmEvent.GetFsmEvent("PURSUER"),
                    FsmEvent.GetFsmEvent("SLASHCOMBO"),
                    FsmEvent.GetFsmEvent("COMBOATTACK"),
                    FsmEvent.GetFsmEvent("PULLUPCOUNTER")
                },
                weights = new[]
                {
                    new FsmFloat(1f),
                    new FsmFloat(1f),
                    new FsmFloat(1f),
                    new FsmFloat(2f),
                    new FsmFloat(1f),
                },
                eventMax = new[]
                {
                    new FsmInt(2),
                    new FsmInt(2),
                    new FsmInt(2),
                    new FsmInt(1),
                    new FsmInt(1),
                },
                missedMax = new[]
                {
                    new FsmInt(4),
                    new FsmInt(4),
                    new FsmInt(4),
                    new FsmInt(4),
                    new FsmInt(4)
                },
                activeBool = true
            });
            attackSelectState.AddTransition("JUMPSLASHCOMBO", LaceAttackList.JumpslashTendrilDive.GetStartStateName());
            attackSelectState.AddTransition("PURSUER", LaceAttackList.ChargeJumpslashHold.GetStartStateName());
            attackSelectState.AddTransition("SLASHCOMBO", LaceAttackList.SlashChargeTendrilDive.GetStartStateName());
            attackSelectState.AddTransition("COMBOATTACK", comboAttackState.name);
            attackSelectState.AddTransition("PULLUPCOUNTER", LaceAttackList.PullUpCounter.GetStartStateName());
            attackSelectState.AddTransition("HEIGHTSNIPE", LaceAttackList.PullUpRapidStab.GetStartStateName());

            //Control state           
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
                    _controlFSM.gameObject.GetComponent<HealthManager>().hp = Constants.PhaseValues.Phase3HP;
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
                time = 0.85f
            });

            //transitions
            controlState.ChangeTransition("ATTACK", attackSelectState.name);
            controlState.ChangeTransition("COUNTER", LaceAttackList.Counter.GetStartStateName());
            controlState.ChangeTransition("TOOK DAMAGE", attackSelectState.name);
            controlState.RemoveTransition("EVADE");
            


            //combo attack state
            comboAttackState.AddAction(new SendRandomEventV4
            {
                events = new[] {
                    FsmEvent.GetFsmEvent("WEB"),
                    FsmEvent.GetFsmEvent("CLAW"),
                },
                weights = new[]
                {
                    new FsmFloat(1f),
                    new FsmFloat(1f),
                },
                eventMax = new[]
                {
                    new FsmInt(1),
                    new FsmInt(1),
                },
                missedMax = new[]
                {
                    new FsmInt(3),
                    new FsmInt(3),
                },
                activeBool = true
            });

            comboAttackState.AddTransition("WEB", LaceAttackList.ComboWeb.GetStartStateName());
            comboAttackState.AddTransition("CLAW", LaceAttackList.ComboClawShot.GetStartStateName());
        }
    }
}
