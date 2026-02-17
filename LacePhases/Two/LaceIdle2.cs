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

namespace LostAndChained.LacePhases.Two
{
    internal class LaceIdle2 : BasePhase
    {
        public LaceIdle2(PlayMakerFSM controlFSM) : base(controlFSM)
        {
        }

        public override string GetControlStateName()
        {
            return Constants.PhaseNames.Idle2;
        }

        public override void Init()
        {
            bool activateTransition = true;

            //Attack select
            FsmState attackSelectState = _controlFSM.AddState(PhaseNames.Idle2 + " Attack");
            attackSelectState.AddAction(new SendRandomEventV4
            {
                events = new[] {
                    FsmEvent.GetFsmEvent("JUMPSLASHCOMBO"),
                    FsmEvent.GetFsmEvent("PURSUER"),
                    FsmEvent.GetFsmEvent("SLASHCOMBO")
                },
                weights = new[]
                {
                    new FsmFloat(1f),
                    new FsmFloat(1f),
                    new FsmFloat(1f)
                },
                eventMax = new[]
                {
                    new FsmInt(2),
                    new FsmInt(2),
                    new FsmInt(2)
                },
                missedMax = new[]
                {
                    new FsmInt(4),
                    new FsmInt(4),
                    new FsmInt(4)
                },
                activeBool = true
            });
            attackSelectState.AddTransition("JUMPSLASHCOMBO", LaceAttackList.JumpslashTendrilDive.GetStartStateName());
            attackSelectState.AddTransition("PURSUER", LaceAttackList.PursuerChargeDive.GetStartStateName());
            attackSelectState.AddTransition("SLASHCOMBO", LaceAttackList.SlashChargeTendrilDive.GetStartStateName());

            //Control state
            FsmState controlState = _controlFSM.CopyState("Idle", PhaseNames.Idle2);
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
                    _controlFSM.gameObject.GetComponent<HealthManager>().hp = Constants.PhaseValues.Phase2HP;
                    _controlFSM.SendEvent("PHASE2");
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
                time = 0.7f
            });

            //transitions
            controlState.ChangeTransition("ATTACK", attackSelectState.name);
            controlState.AddTransition("PHASE2", LaceAttackList.Phase2Shift.GetStartStateName());
            controlState.ChangeTransition("COUNTER", LaceAttackList.Counter.GetStartStateName());
            controlState.ChangeTransition("TOOK DAMAGE", attackSelectState.name);
            controlState.RemoveTransition("EVADE");
        }
    }
}
