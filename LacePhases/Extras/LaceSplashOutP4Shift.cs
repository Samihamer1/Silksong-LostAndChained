using HutongGames.PlayMaker;
using LostAndChained.Phases.Attacks;
using Silksong.FsmUtil;

namespace LostAndChained.LacePhases.Extras
{
    internal class LaceSplashOutP4Shift : BaseAttack
    {
        public LaceSplashOutP4Shift(PlayMakerFSM controlFSM, string EndStateName, string parentAttackString) : base(controlFSM, EndStateName, parentAttackString)
        {
        }

        public override string GetStartStateNamePure()
        {
            return "Override Check";
        }

        public override void Init()
        {

            //Dive out states
            FsmState OverrideState = AddState("Override Check");
            FsmState DiveOutGState = CopyState("Dive Out G", "Dive Out G-2");


            DiveOutGState.InsertMethod(0, _ =>
            {
                _controlFSM.GetBoolVariable("Splashed In").value = false;
            });

            OverrideState.AddMethod(_ =>
            {
                _controlFSM.GetFloatVariable("Tele X").value = 39;
                _controlFSM.GetFloatVariable("Tele Y").value = Constants.Constraints.LaceLandY;
            }
            );


            //transition fixes
            OverrideState.AddTransition("FINISHED", DiveOutGState.name);
            DiveOutGState.ChangeTransition("FINISHED", _endStateName);

        }

        public override string GetAttackName()
        {
            return "Splash Out P4";
        }
    }
}
