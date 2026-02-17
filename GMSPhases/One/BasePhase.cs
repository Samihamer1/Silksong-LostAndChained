using System;
using System.Collections.Generic;
using System.Text;

namespace LostAndChained.Phases.One
{
    internal abstract class BasePhase
    {
        internal PlayMakerFSM _controlFSM;
        public BasePhase(PlayMakerFSM controlFSM)
        {
            _controlFSM = controlFSM;
            Init();
        }

        public abstract void Init();

        public abstract string GetControlStateName();
    }
}
