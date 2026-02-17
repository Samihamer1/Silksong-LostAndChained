using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using LostAndChained.Phases.Attacks;
using Silksong.FsmUtil;
using System;
using System.Collections.Generic;
using System.Text;

namespace LostAndChained.GMSPhases.Attacks
{
    internal class GMSRubblePull : BaseAttack
    {
        public GMSRubblePull(PlayMakerFSM controlFSM, string EndStateName, string parentAttackString) : base(controlFSM, EndStateName, parentAttackString)
        {
        }

        public override string GetAttackName()
        {
            return "Rubble Pull";
        }

        public override string GetStartStateNamePure()
        {
            return "Rubble Choice State";
        }

        public override void Init()
        {
            FsmState ChoiceState = AddState("Rubble Choice State");
            FsmState RubbleP = CopyState("Rubble P", "Rubble P 2");
            FsmState RubbleM = CopyState("Rubble M", "Rubble M 2");
            FsmState Recover = CopyState("Rubble Pull Recover", "Rubble Recover 2");

            foreach (SendEventByName ac in RubbleP.GetActionsOfType<SendEventByName>())
            {
                ac.sendEvent = "ATTACK LONG";
            }
            foreach (SendEventByName ac in RubbleM.GetActionsOfType<SendEventByName>())
            {
                ac.sendEvent = "ATTACK LONG";
            }

            Recover.GetFirstActionOfType<Wait>().time = 3.5f;

            ChoiceState.AddMethod(_ =>
            {
                if (UnityEngine.Random.Range(0, 2) == 0)
                {
                    _controlFSM.SendEvent("PINCER");
                }
                _controlFSM.SendEvent("MIDDLE");
            });

            ChoiceState.AddTransition("PINCER", RubbleP.name);
            ChoiceState.AddTransition("MIDDLE", RubbleM.name);
            RubbleP.ChangeTransition("FINISHED", Recover.name);
            RubbleM.ChangeTransition("FINISHED", Recover.name);
            Recover.ChangeTransition("FINISHED", _endStateName);
        }
    }
}
