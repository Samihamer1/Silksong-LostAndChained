using HutongGames.PlayMaker;
using Silksong.FsmUtil;
using System;
using System.Collections.Generic;
using System.Text;

namespace LostAndChained.Phases.Attacks
{
    internal abstract class BaseAttack
    {
        internal PlayMakerFSM _controlFSM;
        internal string _endStateName;
        internal string _parentAttackString;

        /// <summary>
        /// Create an instance of an attack - this includes an entire chain of states in the controlFSM
        /// </summary>
        /// <param name="controlFSM">FSM to create attack in</param>
        /// <param name="EndStateName">State entered after the attack is over</param>
        /// <param name="parentAttackString">If belonging to an AttackChain, the name of that chain. Use "" if not belonging to one.</param>
        public BaseAttack(PlayMakerFSM controlFSM, string EndStateName, string parentAttackString)
        {
            _controlFSM = controlFSM;
            _endStateName = EndStateName;
            _parentAttackString = parentAttackString;
            Init();            
        }

        /// <summary>
        /// Copy a state and prefix name with _parentAttackString and attack name
        /// </summary>
        /// <param name="fromState"></param>
        /// <param name="toState"></param>
        /// <returns></returns>
        public FsmState CopyState(string fromState, string toState)
        {
            return _controlFSM.CopyState(fromState, _parentAttackString + GetAttackName() + toState);
        }

        /// <summary>
        /// Add a state and prefix name with _parentAttackString and attack name
        /// </summary>
        /// <param name="newState"></param>
        /// <returns></returns>
        public FsmState AddState(string newState)
        {
            return _controlFSM.AddState(_parentAttackString+GetAttackName()+newState);
        }

        public abstract void Init();

        public abstract string GetStartStateNamePure();

        public string GetStartStateName()
        {
            return _parentAttackString + GetAttackName() + GetStartStateNamePure();
        }

        public abstract string GetAttackName();
    }
}
