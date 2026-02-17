using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using LostAndChained.LacePhases._4;
using LostAndChained.LacePhases.One;
using LostAndChained.LacePhases.Three;
using LostAndChained.LacePhases.Two;
using LostAndChained.Phases.Attacks;
using Silksong.FsmUtil;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using static LostAndChained.Constants;

namespace LostAndChained.Components
{
    internal class LaceMain
    {
        private LaceBossScene _mainScene;
        public GameObject _bossObject;
        private GameObject _teleportTelegraph;
        private PlayMakerFSM? LaceControlFSM;

        public LaceMain(LaceBossScene scene,  GameObject laceGameObject)
        {
            _mainScene = scene;
            _bossObject = laceGameObject;
        }

        public void Init()
        {
            LaceControlFSM = _bossObject.LocateMyFSM("Control");

            //LaceControlFSM.RemoveTransition("Tele Init", "FINISHED");

            CreateModifiedAnimations();
            CreateNewControl();
            ModifyTransitions();
            ModifyDeathControl();
            ModifyPhase3();
            CreateNewCrossSlash();
            CreateTeleportTelegraph();
            PatchSuperJumpSequence();
            PatchStun();
        }

        private void PatchStun()
        {
            LaceControlFSM.ChangeTransition("Stun Recover", "FINISHED", LaceAttackList.SplashIn.GetStartStateName());
            LaceControlFSM.GetFirstActionOfType<SetBoolValue>("Stun Recover").boolValue = false;
        }

        private void PatchSuperJumpSequence()
        {
            GameObject sequence = _bossObject.gameObject.transform.parent.gameObject.Child("Superjump Sequence");
            PlayMakerFSM control = sequence.LocateMyFSM("Control");

            control.ChangeTransition("Lift 3", "FINISHED", "Can Superjump");
        }

        private void CreateModifiedAnimations()
        {
            tk2dSpriteAnimator animator = _bossObject.GetComponent<tk2dSpriteAnimator>();
            tk2dSpriteAnimation library = animator.library;

            //Don't know if changes made are permanent, so putting this here just in case.
            if (library.GetClipByName("Combo Slash Triple Fast") != null)
            {
                return;
            }

            Helper.CloneAnimation(library, "Combo Slash Triple", "Combo Slash Triple Fast", 1.5f);
            Helper.CloneAnimation(library, "Tendril Whip G", "Tendril Whip G Fast", 1.3f);
            Helper.CloneAnimation(library, "Tendril Whip End G", "Tendril Whip End G Fast", 1.3f);
            Helper.CloneAnimation(library, "Tendril Whip A", "Tendril Whip A Fast", 1.3f);
            Helper.CloneAnimation(library, "Tendril Whip End A", "Tendril Whip End A Fast", 1.3f);

        }


        private void CreateTeleportTelegraph()
        {
            GameObject original = _bossObject.transform.parent.gameObject.Child("Lost_Lace_appear_emerge");

            GameObject clone = GameObject.Instantiate(original);
            clone.transform.parent = _bossObject.transform.parent;
            clone.name = "Lost_Lace_Teleport_Telegraph";
            clone.transform.localPosition = new Vector3(0, 0.7f, 0);

            clone.Child("Lace").SetActive(false);
            clone.Child("Roar Wave Emitter").SetActive(false);
            clone.Child("Roar Wave Emitter (1)").SetActive(false);

            _teleportTelegraph = clone;
        }

        public void ActivateTeleportTelegraph(Vector3 pos)
        {
            _teleportTelegraph.SetActive(false);
            _teleportTelegraph.SetActive(true);
            _teleportTelegraph.transform.position = pos;
        }

        public void DeactivateTeleportTelegraph()
        {
            _teleportTelegraph.SetActive(false);
        }

        private void CreateNewCrossSlash()
        {
            GameObject crossSlash = _bossObject.transform.parent.gameObject.Child("Lost_Lace_Cross_Slash");
            GameObject newCrossSlash = GameObject.Instantiate(crossSlash);
            newCrossSlash.name = "Lost_Lace_Cross_Slash_Alt";
            newCrossSlash.transform.parent = _bossObject.transform.parent;
            newCrossSlash.transform.localPosition = new Vector3(29.73f, 16, 0);
            newCrossSlash.Child("black_solid").SetActive(false); //disable original visuals
            newCrossSlash.Child("vignette_large_v01").SetActive(false); //disable original visuals

            LaceControlFSM.AddMethod("CS Ready", _ =>
            {
                if (LaceBossScene.Instance.phaseNumber != 3)
                {
                    LaceControlFSM.SendEvent("FINISHED");                    
                } else
                {
                    LaceBossScene.Instance.GMSMain.ActivateBeastSlashEffect();
                }
            });
            LaceControlFSM.AddAction("CS Ready", new Wait() { time = 1.8f, realTime = false, finishEvent = FsmEvent.GetFsmEvent("ALTERNATE") });
            LaceControlFSM.AddTransition("CS Ready", "ALTERNATE", LaceAttackList.CrossSlashCutscene.GetStartStateName());
        }

        public void ActivateAltCrossSlash()
        {
            GameObject altCrossSlash = _bossObject.transform.parent.gameObject.Child("Lost_Lace_Cross_Slash_Alt");

            Vector3 heroPos = HeroController.instance.transform.position;
            altCrossSlash.transform.position = new Vector3(Mathf.Clamp(heroPos.x, 14f, 46f), Mathf.Clamp(heroPos.y, 11, 14), 0);

            altCrossSlash.SetActive(false);
            altCrossSlash.SetActive(true);
        }

        private void ModifyPhase3()
        {
            LaceControlFSM.AddMethod("P3 Roar", _ =>
            {
                _mainScene.GMSMain._bossObject.LocateMyFSM("Control").SendEvent("ROAR");
                LaceControlFSM.GetBoolVariable("Splashed In").value = false;
            });
        }

        private void ModifyDeathControl()
        {
            PlayMakerFSM deathcontrol = _bossObject.LocateMyFSM("Death Control");

            FsmState checkHPState = deathcontrol.AddState("Check HP P2");
            checkHPState.AddMethod(_ =>
            {
                _mainScene.LaceTakenDamage();
            });

            deathcontrol.ChangeTransition("P1", "TOOK DAMAGE", "Check HP P2");
            deathcontrol.AddTransition("Check HP P2", "FINISHED", "P1");
            deathcontrol.RemoveTransition("P3", "ZERO HP");

            FsmState p4 = deathcontrol.AddState("P4");
            p4.AddTransition("ZERO HP", "Can Die?");

            deathcontrol.AddMethod("P3 Start", _ => {
                LaceControlFSM.SetState("Set Roar Pos");
                LaceControlFSM.GetBoolVariable("Splashed In").value = false;
                //LaceControlFSM.gameObject.GetComponent<MeshRenderer>().enabled = true;
            });

        }

        public void ContinueP4Shift()
        {
            LaceControlFSM.SendEvent("CONTINUE");
        }

        public void Phase4Shift()
        {
            LaceControlFSM.SetState(LaceAttackList.Phase4Shift.GetStartStateName());
            _bossObject.GetComponent<HealthManager>().hp = 1000; //just in case. not sure if itll insta kill her or anything otherwise. overwritten anyway.
            PlayMakerFSM deathcontrol = _bossObject.LocateMyFSM("Death Control");
            deathcontrol.SetState("P4");
        }

        public void DieToPhase3()
        {
            PlayMakerFSM deathcontrol = _bossObject.LocateMyFSM("Death Control");
            deathcontrol.SetState("Mid Death Stagger");

            //_bossObject.GetComponent<MeshRenderer>().enabled = false;
        }

        private void ModifyTransitions()
        {
            LaceControlFSM.RemoveTransition("Stop", "TO P3"); //phase 3
            LaceControlFSM.ChangeTransition("P3 Roar End", "FINISHED", LaceAttackList.TendrilIntoDive.GetStartStateName());
            //LaceControlFSM.ChangeTransition("Set Roar Pos", "FINISHED", "P3 Dive Out");
        }

        public void Phase2Shift()
        {
            LaceControlFSM.SetState(LaceAttackList.Phase2Shift.GetStartStateName());
        }

        public void ReadyCombo()
        {
            LaceControlFSM.SendEvent("READY");
        }

        private void CreateNewControl()
        {
            PlayMakerFSM attackControl = LaceControlFSM;


            FsmState IdleChoice = attackControl.AddState("Idle Choice");
            IdleChoice.AddMethod(_ =>
            {
                //Decide which idle to go to
                switch (LaceBossScene.Instance.phaseNumber)
                {
                    case (1):
                        attackControl.SendEvent(PhaseNames.Idle1);
                        break;
                    case 2:
                        attackControl.SendEvent(PhaseNames.Idle2);
                        break;
                    case 3:
                        attackControl.SendEvent(PhaseNames.Idle3);
                        break;
                    case 4:
                        if (!LaceBossScene.Instance.phaseActive) //if the cutscene isnt over
                        {
                            LaceControlFSM.SetState(LaceAttackList.SplashInPerma.GetStartStateName());
                            break; //just wait
                        }
                        attackControl.SendEvent(PhaseNames.Idle4);
                        break;
                    default:
                        attackControl.SendEvent(PhaseNames.Idle4);
                        break;
                }
            });

            //Ensuring original idle is never used

            attackControl.InsertMethod("Idle", _ =>
            {
                attackControl.SendEvent("FINISHED");
            }, 0);

            attackControl.InsertMethod("Attack Choice", _ =>
            {
                attackControl.SendEvent("FINISHED");
            }, 0);

            //Our new idle is defaulted to
            attackControl.AddTransition("Idle", "FINISHED", "Idle Choice");
            attackControl.AddTransition("Attack Choice", "FINISHED", "Idle Choice");


            //Init Attacks
            LaceAttackList.InitLaceAttackStrings(attackControl);

            //Init Phases
            LaceIdle1 laceIdle1 = new LaceIdle1(attackControl);
            attackControl.AddTransition("Idle Choice", laceIdle1.GetControlStateName(), laceIdle1.GetControlStateName());

            LaceIdle2 laceIdle2 = new LaceIdle2(attackControl);
            attackControl.AddTransition("Idle Choice", laceIdle2.GetControlStateName(), laceIdle2.GetControlStateName());

            LaceIdle3 laceIdle3 = new LaceIdle3(attackControl);
            attackControl.AddTransition("Idle Choice", laceIdle3.GetControlStateName(), laceIdle3.GetControlStateName());

            LaceIdle4 laceIdle4 = new LaceIdle4(attackControl);
            attackControl.AddTransition("Idle Choice", laceIdle4.GetControlStateName(), laceIdle4.GetControlStateName());
        }

    }
}
