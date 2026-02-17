using HutongGames.PlayMaker;
using LostAndChained.Phases.Attacks;
using Silksong.FsmUtil;
using System;
using System.Collections.Generic;
using System.Text;

namespace LostAndChained.LacePhases.Attacks
{
    internal class BaseAttackString
    {
        //An attack string will be like
        // Swipe 1 - Swipe 2 - Dive where each is a base attack
        // store each base attack in a list
        // auto create a state to transition to the next
        internal PlayMakerFSM _controlFSM;
        internal string _endStateName;
        internal List<Type> _attackChain = new List<Type>();
        internal string _attackName;
        internal string _startingStateName = "";

        public BaseAttackString(PlayMakerFSM controlFSM, string EndStateName, string attackName, List<Type> attackChain)
        {
            _controlFSM = controlFSM;
            _endStateName = EndStateName;
            _attackChain = attackChain;
            _attackName = attackName;
            Init();
        }

        //Alternate constructor if EndStateName isn't needed
        public BaseAttackString(PlayMakerFSM controlFSM, string attackName, List<Type> attackChain) : this(controlFSM, "Idle Choice", attackName, attackChain)
        {
        }

        public void Init()
        {
            //Creating attacks in reverse so that nonexisting states don't have transitions made to
            string previousStartingStateName = _endStateName;
            Dictionary<Type, int> countedTypes = new Dictionary<Type, int>(); //To allow for multiple instances of the same attack in a string

            for (int i = _attackChain.Count - 1; i >= 0; i--)
            {
                Type attackType = _attackChain[i];
                if (countedTypes.ContainsKey(attackType))
                {
                    countedTypes[attackType]++;
                } else
                {
                    countedTypes.Add(attackType, 1);
                }
                int addon = countedTypes[attackType];

                BaseAttack atk = (BaseAttack)Activator.CreateInstance(attackType, _controlFSM, previousStartingStateName, _attackName+addon);
                previousStartingStateName = atk.GetStartStateName();
            }

            _startingStateName = previousStartingStateName;
        }

        public string GetStartStateName()
        {
            return _startingStateName;
        }
    }
}
