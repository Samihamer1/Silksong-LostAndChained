using GenericVariableExtension;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using LostAndChained.GMSPhases.Three;
using LostAndChained.GMSPhases.Two;
using LostAndChained.Phases.Attacks;
using LostAndChained.Phases.One;
using Silksong.AssetHelper.ManagedAssets;
using Silksong.FsmUtil;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Audio;
using static LostAndChained.Constants;
using static System.TimeZoneInfo;

namespace LostAndChained.Components
{
    internal class GMSMain
    {
        private PlayMakerFSM _controlFSM;
        public GameObject _bossObject;
        private LaceBossScene _mainScene;
        private ConstrainPosition _constrain;
        private tk2dSpriteAnimation _silkBossAnim;
        private tk2dSpriteCollectionData _silkBossCollectionData;

        private float arenaCenterX = 33.5f;
        private float arenaCenterY = 10.5f;

        public GMSMain(LaceBossScene mainScene, GameObject bossObject)
        {
            _mainScene = mainScene; _bossObject = bossObject;

            _controlFSM = _bossObject.LocateMyFSM("Control");
        }

        public void Init()
        {
            //hands
            GameObject HandL = _bossObject.Child("Hand L");
            GameObject HandR = _bossObject.Child("Hand R");

            if (HandR == null || HandL == null) { Log.Error("One or more Hands not found."); return; }

            PatchHand(HandL.LocateMyFSM("Hand Control"));
            PatchHand(HandR.LocateMyFSM("Hand Control"));

            //Rest of boss
            PatchMaterial(); //done early to affect the blades too

            _bossObject.transform.parent.gameObject.SetActive(true); //boss scene object

            _bossObject.GetComponent<HealthManager>().damageScaling = new HealthManager.DamageScalingConfig
            {
                Level1Mult = 1,
                Level2Mult = 0.8f,
                Level3Mult = 0.6f,
                Level4Mult = 0.5f,
                Level5Mult = 0.5f
            };

            _controlFSM = _bossObject.LocateMyFSM("Control");
            ModifyGameObjects();
            ModifyControlVariables();
            ModifyControlActions();
            ModifyTransitions();
            CreateNewControl();
            PatchStun();

            _constrain = _bossObject.GetComponent<ConstrainPosition>();
            _constrain.xMin = Constraints.GMSMinX;
            _constrain.xMax = Constraints.GMSMaxX;


            PatchHairMaterial();
            PatchRubble();
            PatchSpikes();
            DisableHairVignette();

            PatchPhaseShift();

            PatchWebs();

            PatchBeastSlashEffect();

            GameManager.instance.StartCoroutine(PatchDeathSequenceCoroutine());
        }

        public void PromptParry()
        {
            GameObject deathsequence = _bossObject.transform.parent.gameObject.Child("Death Sequence");
            PlayMakerFSM deathcontrol = deathsequence.LocateMyFSM("Control");
            deathcontrol.SendEvent("NEXT");
        }

        private IEnumerator PatchDeathSequenceCoroutine()
        {
            yield return LoadThemeClip();

            PatchDeathSequence();
        }

        private void PatchDeathSequence()
        {
            //make lace stop

            GameObject roarHolder = new GameObject("Roar Holder");
            Helper.CreateTempRoar(roarHolder);

            FsmOwnerDefault hornetOwner = new FsmOwnerDefault
            {
                gameObject = HeroController.instance.gameObject,
                ownerOption = OwnerDefaultOption.SpecifyGameObject
            };

            //Object changes
            GameObject deathsequence = _bossObject.transform.parent.gameObject.Child("Death Sequence");
            GameObject deathsilk = deathsequence.Child("Death Silk");

            deathsilk.transform.localPosition = new Vector3(33.84f, 8f, 0.004f);
            deathsilk.GetComponent<tk2dSprite>().color = new Color(0.05f, 0.05f, 0.05f, 1f);

            deathsequence.Child("Death Slashes").transform.SetPositionY(6.25f);
            deathsequence.Child("Hit Flash").transform.SetPositionY(6.25f);

            //fsm changes
            PlayMakerFSM deathcontrol = deathsequence.LocateMyFSM("Control");
            deathcontrol.RemoveTransition("Soul Snare?", "CANCEL");
            deathcontrol.RemoveTransition("Bind Or Needolin", "NEEDOLIN");
            deathcontrol.GetFirstActionOfType<FloatClamp>("Death Slashes Up").maxValue = 39f;

            FsmState delayState = deathcontrol.AddState("Death Delay");
            delayState.AddAction(new Wait { time = 5f, realTime = false, finishEvent = FsmEvent.GetFsmEvent("FINISHED") });

            FsmState revealState = deathcontrol.AddState("Death Reveal");
            FsmColor notopaque = deathcontrol.GetFirstActionOfType<ScreenFader>("Final Bind").endColour;
            FsmColor opaque = deathcontrol.GetFirstActionOfType<ScreenFader>("Final Bind").startColour;

            revealState.AddAction(new ScreenFader { startColour = notopaque, endColour = opaque, duration = 0.5f });
            revealState.AddAction(new Wait { time = 4.5f, realTime = false, finishEvent = FsmEvent.GetFsmEvent("FINISHED") });
            revealState.AddMethod(_ =>
            {
                deathsilk.SetActive(false);
                HeroController.instance.GetComponent<MeshRenderer>().enabled = true;
                HeroController.instance.transform.SetPosition2D(30, 6.5f);
                HeroController.instance.FaceRight();
                HeroController.instance.GetComponent<tk2dSpriteAnimator>().Play("Prostrate");
            });

            //new anims
            FsmState risetoKneelState = deathcontrol.AddState("Hornet RiseToKneel");
            risetoKneelState.AddAction(new Tk2dPlayAnimationWithEvents { gameObject = hornetOwner, clipName = "ProstrateRiseToKneel NoLoop", animationCompleteEvent = FsmEvent.GetFsmEvent("FINISHED") });
            risetoKneelState.AddMethod(_ =>
            {
                LaceBossScene.Instance.LaceMain.Phase4Shift();
            });

            FsmState slowTimeState = deathcontrol.AddState("Slow Time");
            slowTimeState.AddAction(new ScaleTime { timeScale = 0.25f, adjustFixedDeltaTime = true, everyFrame = false });
            slowTimeState.AddAction(new Wait { time = 0.25f, realTime = false, finishEvent = FsmEvent.GetFsmEvent("FINISHED") });

            FsmState parryPromptState = deathcontrol.AddState("Hornet Parry Prompt");
            parryPromptState.AddAction(new ShowControlReminderSingle
            {
                DisappearOnButtonPress = false,
                DisappearEvent = "BOSS PARRY DOWN",
                FadeInDelay = 0.5f,
                FadeInTime = 0.2f,
                FadeOutTime = 0.1f,
                Prompt = new TeamCherry.Localization.LocalisedFsmString { Key = "", Sheet = "" },
                Text = new TeamCherry.Localization.LocalisedFsmString { Key = "PROMPT_PARRY", Sheet = "Prompts" },
                Button = GlobalEnums.HeroActionButton.QUICK_CAST,
                AppearEvent = new FsmString(),
                PlayerDataBool = new FsmString()
            });
            parryPromptState.AddAction(new ListenForQuickCast
            {
                eventTarget = deathcontrol.GetAction<ListenForCast>("Bind Or Needolin", 1).eventTarget,
                wasPressed = FsmEvent.GetFsmEvent("PARRY")
            });

            FsmState parryState = deathcontrol.AddState("Hornet Parry");
            parryState.AddAction(new SendEventToRegister { eventName = "BOSS PARRY DOWN" });
            parryState.AddAction(new Tk2dPlayAnimationWithEvents { gameObject = hornetOwner, clipName = "Parry Stance Ground", animationCompleteEvent = FsmEvent.GetFsmEvent("FINISHED") });
            parryState.AddAction(new ScaleTime { timeScale = 1f, adjustFixedDeltaTime = true, everyFrame = false });
            parryState.AddMethod(_ =>
            {
                HeroController.instance.GetComponent<SpriteFlash>().flashArmoured();
                AudioClip sound = (AudioClip)HeroController.instance.silkSpecialFSM.GetFirstActionOfType<AudioPlayerOneShotSingle>("Parry Start").audioClip.value;
                HeroController.instance.GetComponent<AudioSource>().PlayOneShot(sound);
                HeroController.instance.gameObject.Child("Special Attacks").Child("Parry Stance Flash").SetActive(true);

                LaceBossScene.Instance.LaceMain.ContinueP4Shift();
            });

            FsmState parryRecoverState = deathcontrol.AddState("Hornet Parry Recover");
            parryRecoverState.AddAction(new Tk2dPlayAnimationWithEvents { gameObject = hornetOwner, clipName = "Parry Recover Ground", animationCompleteEvent = FsmEvent.GetFsmEvent("FINISHED") });

            FsmState challengeState = deathcontrol.AddState("Hornet Challenge");
            challengeState.AddMethod(_ =>
            {
                HeroController.instance.GetComponent<tk2dSpriteAnimator>().Play("Challenge Strong");
                RandomAudioClipTable table = (RandomAudioClipTable)HeroController.instance.silkSpecialFSM.GetAction<PlayRandomAudioClipTable>("Standard", 0).Table.value;
                AudioClip sound = (AudioClip)table.clips[0].Clip;

                AudioClip sound2 = (AudioClip)HeroController.instance.silkSpecialFSM.GetFirstActionOfType<AudioPlayerOneShotSingle>("Taunt Antic").audioClip.value;
                HeroController.instance.GetComponent<AudioSource>().PlayOneShot(sound);
                HeroController.instance.GetComponent<AudioSource>().PlayOneShot(sound2);

                roarHolder.transform.position = HeroController.instance.transform.position;
                Helper.StartTempRoar(roarHolder);

                MusicCue song = LostAndChainedPlugin.Instance.hornetTheme.InstantiateAsset<MusicCue>();
                song.snapshot.TransitionTo(0.1f);

                AudioManager.Instance.ApplyMusicCue(song, 0f, 0.1f, applySnapshot: false);

                LaceBossScene.Instance.phaseActive = true;
                LaceBossScene.Instance.LaceMain._bossObject.GetComponent<DamageHero>().enabled = true;
            });
            challengeState.AddAction(new Wait { time = 3f, realTime = false, finishEvent = FsmEvent.GetFsmEvent("FINISHED") });

            FsmState endChallengeState = deathcontrol.AddState("End Challenge");
            endChallengeState.AddAction(new Tk2dPlayAnimationWithEvents { gameObject = hornetOwner, clipName = "ChallengeStrongToIdle", animationCompleteEvent = FsmEvent.GetFsmEvent("FINISHED") });
            endChallengeState.AddMethod(_ =>
            {
                Helper.EndTempRoar(roarHolder);
            });

            FsmState regainControlState = deathcontrol.AddState("Regain Control");
            regainControlState.AddMethod(_ =>
            {
                HeroController.instance.gameObject.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
                HeroController.instance.RegainControl();
                HeroController.instance.StartAnimationControl();
                HeroController.instance.playerData.SetBool("isInvincible", false);
                HeroController.instance.playerData.SetBool("disablePause", false);
                ToolItemManager.activeState = ToolsActiveStates.Active;
            });

            //FsmState wakeupState = deathcontrol.AddState("Hornet Wakeup");

            deathcontrol.ChangeTransition("Final Bind", "FINISHED", delayState.name);
            deathcontrol.AddTransition(delayState.name, "FINISHED", revealState.name);
            deathcontrol.AddTransition(revealState.name, "FINISHED", risetoKneelState.name);
            deathcontrol.AddTransition(risetoKneelState.name, "NEXT", slowTimeState.name);
            deathcontrol.AddTransition(slowTimeState.name, "FINISHED", parryPromptState.name);
            deathcontrol.AddTransition(parryPromptState.name, "PARRY", parryState.name);
            deathcontrol.AddTransition(parryState.name, "FINISHED", parryRecoverState.name);
            deathcontrol.AddTransition(parryRecoverState.name, "FINISHED", challengeState.name);
            deathcontrol.AddTransition(challengeState.name, "FINISHED", endChallengeState.name);
            deathcontrol.AddTransition(endChallengeState.name, "FINISHED", regainControlState.name);
            //deathcontrol.AddTransition(challengeState.name, "FINISHED", );

            //edit Y vals
            deathcontrol.GetAction<SetPosition>("Death Slashes Up", 11).y = 11f;
            deathcontrol.GetAction<SetPosition>("Death Slashes Up", 12).y = 11f;
            deathcontrol.GetAction<AnimateYPositionTo>("Strike Down", 5).ToValue = 9.8f;

        }

        private void PatchRubble()
        {
            GameObject field = _bossObject.transform.parent.gameObject.Child("Rubble Fields");

            GameObject rubbleL = field.Child("Rubble Field L");
            GameObject rubbleM = field.Child("Rubble Field M");
            GameObject rubbleR = field.Child("Rubble Field R");

            rubbleM.transform.localPosition = new Vector3(28, 23);
            rubbleL.transform.localPosition = new Vector3(9, 23);
            rubbleR.transform.localPosition = new Vector3(47, 23);
        }

        private void PatchSpikes()
        {
            GameObject spikes = _bossObject.transform.parent.gameObject.Child("Spike Floors");

            float x = 8;
            float xmax = 50;

            for (int i = 1; i < 7; i++)
            {
                GameObject floor = spikes.Child("Spike Floor " + i);
                if (floor == null) { continue; }

                floor.transform.localPosition = new Vector3(x + (((xmax - x) / 6) * i-1), 4.5f, 0.4f);
            }
        }

        private void PatchBeastSlashEffect()
        {
            PlayMakerFSM attackControl = _bossObject.LocateMyFSM("Attack Control");
            attackControl.AddMethod("Do Slash", _ =>
            {
                GameManager.instance.StartCoroutine(LaceCrossSlash());
            });

            attackControl.GetAction<FloatClamp>("Shift Hero", 3).minValue = Constants.Constraints.GMSMinX;
            attackControl.GetAction<FloatClamp>("Shift Hero", 3).maxValue = Constants.Constraints.GMSMaxX;
        }

        private IEnumerator LaceCrossSlash()
        {
            yield return new WaitForSeconds(1.8f);
            LaceBossScene.Instance.LaceMain.ActivateAltCrossSlash();
        }

        public void ActivateBeastSlashEffect()
        {
            GameObject effect = _bossObject.transform.parent.gameObject.Child("beast_slash_sequence");
            if (effect == null) { return; }
            HeroController.instance.transform.SetPositionY(Constraints.ArenaCenterY);
            effect.transform.localPosition = HeroController.instance.transform.position;
            effect.SetActive(true);
        }

        private void PatchWebs()
        {
            GameObject patterns = _bossObject.transform.parent.gameObject.Child("Strand Patterns");
            if (patterns == null) { return; }

            GameObject pattern1 = patterns.Child("Pattern 1");
            GameObject pattern2 = patterns.Child("Pattern 2");

            patterns.transform.localPosition = new Vector3(Constraints.ArenaCenterX, Constraints.ArenaCenterY, 0);
            pattern1.transform.localPosition = Vector3.zero;
            pattern2.transform.localPosition = Vector3.zero;

            PatchWeb(pattern1.LocateMyFSM("silk_boss_pattern_control"));
            PatchWeb(pattern2.LocateMyFSM("silk_boss_pattern_control"));
        }

        private void PatchWeb(PlayMakerFSM webControl)
        {
            webControl.GetVector3Variable("Init Pos").Value = new Vector3(Constraints.ArenaCenterX, Constraints.ArenaCenterY, 0);
        }

        private void PatchPhaseShift()
        {
            PlayMakerFSM phaseControl = _bossObject.LocateMyFSM("Phase Control");

            phaseControl.GetAction<TransitionToAudioSnapshot>("Stagger Hit", 3).enabled = false;
            phaseControl.GetAction<TransitionToAudioSnapshot>("Stagger Hit", 15).enabled = false;

            phaseControl.RemoveTransition("Stagger Pause", "FINISHED");

            phaseControl.AddGlobalTransition("SHIFTPHASE2", "Stagger Hit");

            phaseControl.RemoveTransition("Death Type", "CURSED");


            #region phase 2 transition (required because attack control and control are different fsms)

            CreatePhase2Shift();

            #endregion

            #region phase 3 transition for the same reasons

            CreatePhase3Shift();

            #endregion
        }

        private void CreatePhase3Shift()
        {

            #region new idle

            FsmState idle3 = _controlFSM.CopyState("Idle", "Idle 3");
            idle3.GetFirstActionOfType<SetBoolValueAtTime>().Time = 0.05f;

            _controlFSM.InsertMethod("Idle", 0, _ =>
            {
                if (LaceBossScene.Instance.phaseNumber == 3)
                {
                    _controlFSM.SetState("Idle 3"); //cba
                }
            });

            #endregion

            GameObject GMS = _controlFSM.gameObject;
            FsmOwnerDefault GMSOwner = _controlFSM.GetFirstActionOfType<AnimatePositionBy>("Rerise Up").gameObject;

            FsmState P3Delay = _controlFSM.AddState("P3 Delay");
            FsmState P3Down = _controlFSM.CopyState("Rerise Up","P3 Down");
            FsmState P3Delay2 = _controlFSM.AddState("P3 Delay 2");
            FsmState P3Up = _controlFSM.CopyState("Rerise Up", "P3 Up");
            FsmState P3Delay3 = _controlFSM.AddState("P3 Delay 3");
            FsmState P3RoarAntic = _controlFSM.CopyState("Rerise Roar Antic", "P3 Roar Antic");
            FsmState P3Roar = _controlFSM.CopyState("Rerise Roar", "P3 Roar");
            FsmState P3RoarEnd = _controlFSM.CopyState("Rerise Roar End", "P3 Roar End");
            FsmState P3Start = _controlFSM.CopyState("Rerise End", "P3 Start");

            P3Down.GetFirstActionOfType<AnimatePositionBy>().shiftBy.value = new Vector3(0, -18, 0);
            P3Down.GetFirstActionOfType<AnimatePositionBy>().time = 0.75f;
            P3Down.GetFirstActionOfType<Wait>().time = 0.75f;
            P3Down.AddAction(new SendEventToRegister { eventName = "SILK STUNNED" });

            P3Delay2.AddMethod(_ =>
            {
                GMS.transform.parent.position = new Vector3(0, 0, 0); //moving boss scene into foreground
                GMS.transform.localPosition = new Vector3(Constants.Constraints.ArenaCenterX, Constants.Constraints.ArenaCenterY - 18f, 0.01f);
            });
            P3Delay2.AddAction(new Wait { time = 6f, realTime = false, finishEvent = FsmEvent.GetFsmEvent("FINISHED") });

            P3Up.GetFirstActionOfType<AnimatePositionBy>().shiftBy.value = new Vector3(0, 18, 0);
            P3Up.GetFirstActionOfType<AnimatePositionBy>().time = 0.75f;
            P3Up.GetFirstActionOfType<Wait>().time = 0.75f;

            P3Delay3.AddAction(new Wait { time = 0.35f, realTime = false, finishEvent = FsmEvent.GetFsmEvent("FINISHED") });

            P3Roar.GetFirstActionOfType<TransitionToAudioSnapshot>().enabled = false;
            GameObject spawnpoint = P3Roar.GetFirstActionOfType<StartRoarEmitter>().spawnPoint.gameObject.value;
            P3Roar.GetFirstActionOfType<StartRoarEmitter>().enabled = false;
            P3Roar.AddMethod(_ =>
            {
                Helper.StartTempRoar(spawnpoint);
            });
            P3Roar.AddAction(new FaceObjectV2 { objectA = GMSOwner, objectB = HeroController.instance.gameObject, spriteFacesRight = true, playNewAnimation = false, newAnimationClip = "", resetFrame = true, everyFrame = false, pauseBetweenTurns = 0 });

            P3RoarEnd.AddMethod(_ =>
            {
                Helper.EndTempRoar(spawnpoint);
            });
            P3RoarEnd.AddAction(new SendEventToRegister { eventName = "BLADES RETURN" });

            //transitions
            P3Down.ChangeTransition("FINISHED", P3Delay2.name);
            P3Delay2.AddTransition("FINISHED", P3Up.name);
            P3Up.RemoveTransition("FINISHED");
            P3Up.AddTransition("ROAR", P3Delay3.name);
            P3Delay3.AddTransition("FINISHED", P3RoarAntic.name);
            P3RoarAntic.ChangeTransition("FINISHED", P3Roar.name);
            P3Roar.ChangeTransition("FINISHED", P3RoarEnd.name);
            P3RoarEnd.ChangeTransition("FINISHED", P3Start.name);
            P3Start.ChangeTransition("FINISHED", "Idle 3");

            _controlFSM.AddGlobalTransition("LACE KILLED", P3Down.name);
        }

        private void CreatePhase2Shift()
        {
            GameObject GMS = _controlFSM.gameObject;
            FsmOwnerDefault GMSOwner = _controlFSM.GetFirstActionOfType<AnimatePositionBy>("Rerise Up").gameObject;


            #region new idle

            FsmState Idle2 = _controlFSM.AddState("Idle 2");
            FsmState AttackPrepare2 = _controlFSM.CopyState("Attack Prepare", "Attack Prepare 2");
            FsmState Dormant2 = _controlFSM.AddState("Dormant 2");

            Idle2.AddAction(new BoolTest { boolVariable = _controlFSM.GetBoolVariable("Attack Prepare"), isTrue = FsmEvent.GetFsmEvent("PREPARE"), everyFrame = true, isFalse = new FsmEvent("") });
            AttackPrepare2.GetFirstActionOfType<Wait>().time = 1f;

            Idle2.AddTransition("PREPARE", AttackPrepare2.name);
            AttackPrepare2.RemoveTransition("MOVE START");
            AttackPrepare2.ChangeTransition("FINISHED", Idle2.name);

            #endregion

            #region transition

            FsmState P2Delay = _controlFSM.AddState("P2 Delay");
            FsmState P2Init = _controlFSM.AddState("P2 Init");
            FsmState P2Rise = _controlFSM.CopyState("Rerise Up", "P2 Rise");
            FsmState P2Start = _controlFSM.CopyState("Rerise End", "P2 Start");

            P2Delay.AddAction(new Wait { time = 2f, realTime = false, finishEvent = FsmEvent.GetFsmEvent("FINISHED") });

            P2Init.AddMethod(_ =>
            {
                if (LaceBossScene.Instance.phaseNumber != 2)
                {
                    //dont continue if we arent actually in phase 2, this is for the p4 cutscene
                    _controlFSM.SendEvent("CANCEL");
                    return;
                }

                GMS.transform.parent.position = new Vector3(0, 0, 20); //moving boss scene into background
                GMS.transform.localPosition = new Vector3(Constants.Constraints.ArenaCenterX, Constants.Constraints.ArenaCenterY - 18f, 0.01f);
            });

            P2Rise.GetFirstActionOfType<SendEventByName>().enabled = false; //idk what this does but i dont think i need it

            P2Start.AddAction(new SendEventToRegister { eventName = "BLADES RETURN" });

            //transitions
            P2Delay.AddTransition("FINISHED", P2Init.name);
            P2Init.AddTransition("FINISHED", P2Rise.name);
            P2Init.AddTransition("CANCEL", P2Delay.name);
            P2Rise.ChangeTransition("FINISHED", P2Start.name);
            P2Start.ChangeTransition("FINISHED", Idle2.name);

            _controlFSM.ChangeTransition("Move Stop", "FINISHED", "P2 Delay");
            #endregion
        }

        private void PatchStun()
        {
            PlayMakerFSM stunControl = _bossObject.LocateMyFSM("Stun Control");
            stunControl.GetIntVariable("Stun Combo").value = 99999;
            stunControl.GetIntVariable("Stun Hit Max").value = 99999;
        }

        private void DisableHairVignette()
        {
            GameObject hair = _bossObject.transform.parent.gameObject.Child("Silk_Hair");
            if (hair == null) { return; }
            GameObject follow = hair.Child("Hair Point Follow");
            if (follow == null) { return; }
            GameObject blackVignette = follow.Child("Black Vignette");
            GameObject whiteVignette = follow.Child("Light Vignette");
            if (blackVignette != null)
            {
                blackVignette.SetActive(false);
            }
            if (whiteVignette != null)
            {
                whiteVignette.SetActive(false);
            }

        }

        private void PatchHairMaterial()
        {
            GameObject hair = _bossObject.transform.parent.gameObject.Child("Silk_Hair");
            foreach (MeshRenderer renderer in hair.GetComponentsInChildren<MeshRenderer>(true))
            {
                renderer.material.color = new Color(0.05f, 0.05f, 0.05f, 1f);
            }
        }

        private void PatchMaterial()
        {
            _silkBossCollectionData = _bossObject.GetComponent<tk2dSprite>().collectionInst;

            //void color
            //Material mat = _silkBossCollectionData.materials[0];
            //foreach (Material m in _silkBossCollectionData.materials)
            //{
            //    m.color = new Color(0.05f, 0.05f, 0.05f, 1f);
            //}

            foreach (tk2dSprite sprite in _bossObject.GetComponentsInChildren<tk2dSprite>(true))
            {
                sprite.color = new Color(0.05f, 0.05f, 0.05f, 1f);
            }


            ////foreach (tk2dSpriteDefinition sprite in _silkBossCollectionData.spriteDefinitions)
            ////{
            ////    sprite.material = mat;
            ////}

            //_silkBossCollectionData.InitDictionary();

            //_bossObject.GetComponent<MeshRenderer>().SetMaterial(mat);

            //foreach (tk2dSpriteAnimationClip clip in _bossObject.GetComponent<tk2dSpriteAnimator>().library.clips)
            //{
            //    foreach (tk2dSpriteAnimationFrame frame in clip.frames)
            //    {
            //        frame.spriteCollection = _silkBossCollectionData;
                    
            //    }
            //}
        }

        private void ModifyGameObjects()
        {
            GameObject bossScene = _bossObject.transform.parent.gameObject;
            GameObject arenaCenter = bossScene.Child("Arena Centre");
            arenaCenter.transform.SetPositionX(arenaCenterX);
            arenaCenter.transform.SetPositionY(arenaCenterY);
        }

        private void ModifyTransitions()
        {
            if (_controlFSM == null) { return; }

            // _controlFSM.RemoveTransition("Begin", "FINISHED");
            //_controlFSM.AddTransition("Ready", "FINISHED", "Intro Up");
            _controlFSM.AddTransition("Dormant", "FINISHED", "Ready");
            _controlFSM.RemoveTransition("Intro Up", "FINISHED");
            _controlFSM.AddTransition("Intro Up", "INTROROAR", "Intro Roar");
           
        }

        private void CreateNewControl()
        {
            PlayMakerFSM attackControl = _bossObject.LocateMyFSM("Attack Control");


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
                    default:
                        //take care of this later
                        attackControl.SendEvent(PhaseNames.Idle3);
                        break;
                }
             });

            //Ensuring original idle is never used
            attackControl.InsertMethod("Idle", _ =>
            {
                attackControl.SendEvent("FINISHED");
            }, 0);

            //Our new idle is defaulted to
            attackControl.AddTransition("Idle", "FINISHED", "Idle Choice");


            //Init Attacks
            GMSAttackList.InitGMSAttackStrings(attackControl);

            //Init Phases
            GMSIdle1 gmsIdle1 = new GMSIdle1(attackControl);
            attackControl.AddTransition("Idle Choice", PhaseNames.Idle1, gmsIdle1.GetControlStateName());

            GMSIdle2 gmsIdle2 = new GMSIdle2(attackControl);
            attackControl.AddTransition("Idle Choice", PhaseNames.Idle2, gmsIdle2.GetControlStateName());

            GMSIdle3 gmsIdle3 = new GMSIdle3(attackControl);
            attackControl.AddTransition("Idle Choice", PhaseNames.Idle3, gmsIdle3.GetControlStateName());
        }

        public void BossRise()
        {
            if (_controlFSM == null) { return; }

            _controlFSM.SetState("Intro Up");

        }

        public void BossRoar()
        {
            if (_controlFSM == null) { return; }

            _controlFSM.SendEvent("INTROROAR");

        }

        private void ModifyControlVariables()
        {
            if (_controlFSM == null) { return; }

            PlayMakerFSM attackControl = _bossObject.LocateMyFSM("Attack Control");
            if (attackControl == null) { return; }

            attackControl.GetFloatVariable("Constrain X Min").Value = Constraints.GMSMinX;
            attackControl.GetFloatVariable("Constrain X Max").Value = Constraints.GMSMaxX;

            _controlFSM.GetFloatVariable("Idle Y").Value = arenaCenterY;
            _controlFSM.GetFloatVariable("Stun Y").Value = arenaCenterY;
            _controlFSM.GetFloatVariable("Bell Stun Y").Value = arenaCenterY;
        }

        private void ModifyControlActions()
        {
            if (_controlFSM == null) { return; }

            PlayMakerFSM attackControl = _bossObject.LocateMyFSM("Attack Control");
            if (attackControl == null) { return; }




            SetPosition initialPos = _controlFSM.GetAction<SetPosition>("Dormant", 1);
            initialPos.x = 34.5f;
            initialPos.y = arenaCenterY;

            _controlFSM.GetAction<SendEventByName>("Title Up", 1).enabled = false; //title card
            _controlFSM.GetAction<Wait>("Intro Roar", 4).time = 3.92f;
            _controlFSM.GetAction<Wait>("Title Up", 2).time = 0f; //roar time
            _controlFSM.GetFirstActionOfType<StartRoarEmitter>("Intro Roar").enabled = false;

            _controlFSM.GetFirstActionOfType<SetBoolValueAtTime>("Idle").Time = 0.25f; //less drift delay
            _controlFSM.GetFirstActionOfType<SetVelocityByScale>("Drift F").speed = -15f; //faster drift forward
            _controlFSM.GetFirstActionOfType<SetVelocityByScale>("Drift B").speed = 30f; //faster drift backward

            //adding roar
            GameObject spawnpoint = _controlFSM.GetFirstActionOfType<StartRoarEmitter>("Intro Roar").spawnPoint.gameObject.value;
            Helper.CreateTempRoar(spawnpoint);
            _controlFSM.AddMethod("Intro Roar", _ =>
            {
                Helper.StartTempRoar(spawnpoint);
            });
            _controlFSM.AddMethod("Begin", _ =>
            {
                Helper.EndTempRoar(spawnpoint);
            });
        }

        #region hand control

        private void PatchHand(PlayMakerFSM handFSM)
        {
            //Alright, this one is gonna be messy.
            //Mostly going to be changing Y values here.
            //If all goes well, you'll never have to look at this again.

            float minBoundXLeft = Constraints.GMSMinX + 5;
            float maxBoundXLeft = Constraints.ArenaCenterX - 1f;
            float minBoundXRight = Constraints.ArenaCenterX + 1f;
            float maxBoundXRight = Constraints.GMSMaxX - 5;

            float swipeBottomYmin = Constraints.GroundY + 1.1f;
            float swipeBottomYmax = swipeBottomYmin + 0.5f;

            float swipeMidYmin = Constraints.ArenaCenterY - 4f;
            float swipeMidYmax = swipeMidYmin + 0.5f;

            float swipeEither1 = Constraints.ArenaCenterY - 3f;
            float swipeEither2 = Constraints.ArenaCenterY + 0.65f;


            //old constraints
            //x 28 to 54

            handFSM.GetAction<FloatClamp>("Stomp Aim", 1).minValue = Constraints.GMSMinX -3.5f;
            handFSM.GetAction<FloatClamp>("Stomp Aim", 1).maxValue = Constraints.GMSMaxX + 0.5f;

            handFSM.GetAction<FloatClamp>("Stomp", 1).minValue = Constraints.GMSMinX - 6f;
            handFSM.GetAction<FloatClamp>("Stomp", 1).maxValue = Constraints.GMSMaxX + 1f;

            handFSM.GetAction<FloatClamp>("Stomp Dash L", 2).minValue = minBoundXLeft;
            handFSM.GetAction<FloatClamp>("Stomp Dash L", 2).maxValue = maxBoundXLeft;

            handFSM.GetAction<FloatClamp>("Stomp Dash R", 2).minValue = minBoundXRight;
            handFSM.GetAction<FloatClamp>("Stomp Dash R", 2).maxValue = maxBoundXRight;

            handFSM.GetAction<CheckXPosition>("Swipe Dir", 1).compareTo = Constraints.GMSMaxX - 8f;
            handFSM.GetAction<CheckXPosition>("Swipe Dir", 2).compareTo = Constraints.GMSMinX + 21f;


            //Swipe R
            handFSM.GetAction<RandomFloat>("Swipe R", 3).min = swipeBottomYmin;
            handFSM.GetAction<RandomFloat>("Swipe R", 3).max = swipeBottomYmax;

            handFSM.GetAction<RandomFloat>("Swipe R", 9).min = swipeMidYmin;
            handFSM.GetAction<RandomFloat>("Swipe R", 9).max = swipeMidYmax;

            handFSM.GetAction<RandomFloatEither>("Swipe R", 15).value1 = swipeEither1;
            handFSM.GetAction<RandomFloatEither>("Swipe R", 15).value2 = swipeEither2;

            //Swipe L
            handFSM.GetAction<RandomFloat>("Swipe L", 3).min = swipeBottomYmin;
            handFSM.GetAction<RandomFloat>("Swipe L", 3).max = swipeBottomYmax;

            handFSM.GetAction<RandomFloat>("Swipe L", 9).min = swipeMidYmin;
            handFSM.GetAction<RandomFloat>("Swipe L", 9).max = swipeMidYmax;

            handFSM.GetAction<RandomFloatEither>("Swipe L", 15).value1 = swipeEither1;
            handFSM.GetAction<RandomFloatEither>("Swipe L", 15).value2 = swipeEither2;

            //Swipe High R
            handFSM.GetAction<SetFloatValue>("Swipe High R", 4).floatValue = Constraints.GroundY + 0.5f;
            handFSM.GetAction<SetFloatValue>("Swipe High R", 10).floatValue = Constraints.GroundY + 2.5f;
            handFSM.GetAction<SetFloatValue>("Swipe High R", 16).floatValue = Constraints.GroundY + 9.5f;

            //Swipe High L
            handFSM.GetAction<SetFloatValue>("Swipe High L", 4).floatValue = Constraints.GroundY + 0.5f;
            handFSM.GetAction<SetFloatValue>("Swipe High L", 10).floatValue = Constraints.GroundY + 2.5f;
            handFSM.GetAction<SetFloatValue>("Swipe High L", 16).floatValue = Constraints.GroundY + 9.5f;

            //Claw Dir
            handFSM.GetAction<FloatClamp>("Claw Dir", 2).minValue = Constraints.GMSMinX;
            handFSM.GetAction<FloatClamp>("Claw Dir", 2).maxValue = Constraints.GMSMaxX;

            handFSM.GetAction<CheckXPosition>("Claw Dir", 4).compareTo = Constraints.GMSMaxX - 6f;
            handFSM.GetAction<CheckXPosition>("Claw Dir", 5).compareTo = Constraints.GMSMinX + 6f;

            //Claw R
            handFSM.GetAction<SetFloatValue>("Claw R", 2).floatValue = Constraints.ArenaCenterY+1;
            handFSM.GetAction<SetFloatValue>("Claw R", 10).floatValue = Constraints.ArenaCenterY + 2;
            handFSM.GetAction<SetFloatValue>("Claw R", 18).floatValue = Constraints.ArenaCenterY + 1;

            //Claw L
            handFSM.GetAction<SetFloatValue>("Claw L", 2).floatValue = Constraints.ArenaCenterY + 1;
            handFSM.GetAction<SetFloatValue>("Claw L", 10).floatValue = Constraints.ArenaCenterY + 2;
            handFSM.GetAction<SetFloatValue>("Claw L", 18).floatValue = Constraints.ArenaCenterY + 1;

            //Idle
            handFSM.GetAction<FloatClamp>("Idle", 5).minValue = Constraints.GMSMinX + 1;
            handFSM.GetAction<FloatClamp>("Idle", 5).maxValue = Constraints.GMSMaxX - 1;

            //Prepare
            handFSM.GetAction<FloatClamp>("Prepare", 5).minValue = Constraints.GMSMinX + 1;
            handFSM.GetAction<FloatClamp>("Prepare", 5).maxValue = Constraints.GMSMaxX - 1;

            //Attacking
            handFSM.GetAction<FloatClamp>("Attacking", 6).minValue = Constraints.GMSMinX + 1;
            handFSM.GetAction<FloatClamp>("Attacking", 6).maxValue = Constraints.GMSMaxX - 1;

            //Each of the finger blades
            //Because OF COURSE we're hard coding Y positions into the individual attacks
            //WHEN THERES ALREADY A CONTROLLER FOR THE ATTACKS!!

            GameObject FingerBladeM = handFSM.gameObject.Child("Finger Blade M");
            GameObject FingerBladeL = handFSM.gameObject.Child("Finger Blade L");
            GameObject FingerBladeR = handFSM.gameObject.Child("Finger Blade R");

            if (FingerBladeL == null || FingerBladeM == null || FingerBladeR == null) { Log.Error("One or more Finger Blades not found."); return; }


            PatchFingerBlade(FingerBladeM.LocateMyFSM("Control"));
            PatchFingerBlade(FingerBladeL.LocateMyFSM("Control"));
            PatchFingerBlade(FingerBladeR.LocateMyFSM("Control"));

            GameManager.instance.StartCoroutine(PatchFingerBladesAudio(FingerBladeM.LocateMyFSM("Control"), FingerBladeL.LocateMyFSM("Control"), FingerBladeR.LocateMyFSM("Control")));
        }

        private IEnumerator PatchFingerBladesAudio(PlayMakerFSM control1, PlayMakerFSM control2, PlayMakerFSM control3) 
        {
            //Load audio clips first
            yield return LoadClips();

            PlayMakerFSM[] controls = new PlayMakerFSM[] { control1, control2, control3 };

            foreach (PlayMakerFSM control in controls)
            {
                PatchFingerBladeAudio(control);
            }
        }

        private void PatchFingerBladeAudio(PlayMakerFSM fingerFSM)
        {
            //Audio Tables
            RandomAudioClipTable anticTable = ScriptableObject.CreateInstance<RandomAudioClipTable>();
            anticTable.clips = new RandomAudioClipTable.ProbabilityAudioClip[]
            {
                new RandomAudioClipTable.ProbabilityAudioClip()
                {
                    Clip = LostAndChainedPlugin.Instance.SilkNeedleAntic.InstantiateAsset<AudioClip>(),
                    Probability = 1f
                }
            };
            anticTable.cooldownDuration = 0.5f;
            anticTable.pitchMin = 0.85f;
            anticTable.pitchMax = 1.15f;

            RandomAudioClipTable shootTable = ScriptableObject.CreateInstance<RandomAudioClipTable>();
            shootTable.clips = new RandomAudioClipTable.ProbabilityAudioClip[]
            {
                new RandomAudioClipTable.ProbabilityAudioClip()
                {
                    Clip = LostAndChainedPlugin.Instance.SilkNeedleThrow1.InstantiateAsset<AudioClip>(),
                    Probability = 1f
                },
                new RandomAudioClipTable.ProbabilityAudioClip()
                {
                    Clip = LostAndChainedPlugin.Instance.SilkNeedleThrow2.InstantiateAsset<AudioClip>(),
                    Probability = 1f
                }
            };
            shootTable.cooldownDuration = 0.2f;
            shootTable.pitchMin = 0.85f;
            shootTable.pitchMax = 1.15f;

            RandomAudioClipTable impactTable = ScriptableObject.CreateInstance<RandomAudioClipTable>();
            impactTable.clips = new RandomAudioClipTable.ProbabilityAudioClip[]
            {
                new RandomAudioClipTable.ProbabilityAudioClip()
                {
                    Clip = LostAndChainedPlugin.Instance.SilkNeedleImpact1.InstantiateAsset<AudioClip>(),
                    Probability = 1f
                },
                new RandomAudioClipTable.ProbabilityAudioClip()
                {
                    Clip = LostAndChainedPlugin.Instance.SilkNeedleImpact2.InstantiateAsset<AudioClip>(),
                    Probability = 1f
                },
                new RandomAudioClipTable.ProbabilityAudioClip()
                {
                    Clip = LostAndChainedPlugin.Instance.SilkNeedleImpact3.InstantiateAsset<AudioClip>(),
                    Probability = 1f
                }
            };
            impactTable.cooldownDuration = 0.1f;
            impactTable.pitchMin = 0.85f;
            impactTable.pitchMax = 1.15f;


            //audio cues
            fingerFSM.GetAction<PlayRandomAudioClipTable>("Antic Pull", 1).Table = anticTable;
            fingerFSM.GetAction<PlayRandomAudioClipTable>("Shoot", 1).Table = shootTable;
            fingerFSM.GetAction<PlayRandomAudioClipTable>("Thunk", 1).Table = impactTable;

        }

        public IEnumerator LoadClips()
        {
            //i should probably make a better system for this later
            LostAndChainedPlugin.Instance.SilkNeedleAntic.Load();
            LostAndChainedPlugin.Instance.SilkNeedleThrow1.Load();
            LostAndChainedPlugin.Instance.SilkNeedleThrow2.Load();
            LostAndChainedPlugin.Instance.SilkNeedleImpact1.Load();
            LostAndChainedPlugin.Instance.SilkNeedleImpact2.Load();
            LostAndChainedPlugin.Instance.SilkNeedleImpact3.Load();

            yield return new WaitUntil(() => LostAndChainedPlugin.Instance.SilkNeedleAntic.IsLoaded &&
            LostAndChainedPlugin.Instance.SilkNeedleThrow1.IsLoaded &&
            LostAndChainedPlugin.Instance.SilkNeedleThrow2.IsLoaded &&
            LostAndChainedPlugin.Instance.SilkNeedleImpact1.IsLoaded &&
            LostAndChainedPlugin.Instance.SilkNeedleImpact2.IsLoaded &&
            LostAndChainedPlugin.Instance.SilkNeedleImpact3.IsLoaded);
        }

        public IEnumerator LoadThemeClip()
        {
            LostAndChainedPlugin.Instance.hornetTheme.Load();

            yield return new WaitUntil(() => LostAndChainedPlugin.Instance.hornetTheme.IsLoaded);
        }

        private void PatchFingerBlade(PlayMakerFSM fingerFSM)
        {
            fingerFSM.GetFloatVariable("Ground Y").value = Constraints.GroundY;


            //Couple of positions again
            fingerFSM.GetAction<SetFloatValue>("Set Stomp",7).floatValue = Constraints.GMSMinX;
            fingerFSM.GetAction<SetFloatValue>("Set Stomp", 8).floatValue = Constraints.GMSMaxX;
            fingerFSM.GetAction<SetFloatValue>("Set Stomp Q", 4).floatValue = Constraints.GMSMinX;
            fingerFSM.GetAction<SetFloatValue>("Set Stomp Q", 5).floatValue = Constraints.GMSMaxX;

            //hotfix for Silk Boss being null during Rise Side
            fingerFSM.InsertMethod("Rise Side", _ =>
            {
                fingerFSM.GetGameObjectVariable("Silk Boss").value = _bossObject;
            }, 1);


            //Keeping them onscreen when L or R
          
            fingerFSM.AddMethod("Antic Pull", _ =>
            {
                GameManager.instance.StartCoroutine(ClampFingerPosition(fingerFSM));
            });

            fingerFSM.GetFirstActionOfType<GetPosition2d>("Antic Pull").enabled = false;
        }

        private IEnumerator ClampFingerPosition(PlayMakerFSM fingerFSM)
        {
            while (fingerFSM.ActiveStateName == "Antic Pull")
            {
                Vector3 travelPos = fingerFSM.GetVector3Variable("Travel Pos").value;
                GameObject currentCamera = GameManager.instance.cameraCtrl.gameObject;
                

                GameObject finger = fingerFSM.gameObject;

                float distance = 10.5f;
                float clampedX = Mathf.Clamp(travelPos.x, currentCamera.transform.position.x - distance, currentCamera.transform.position.x + distance);

                fingerFSM.GetFloatVariable("X Pos").value = clampedX;

                yield return new WaitForEndOfFrame();
            }
        }

        #endregion
    }
}
