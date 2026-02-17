using Silksong.AssetHelper.ManagedAssets;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LostAndChained.Components
{
    internal class LaceBossScene : MonoBehaviour
    {
        public static LaceBossScene Instance;
        private IntroControl IntroControl;
        public GMSMain GMSMain;
        public Cocoon Cocoon;
        public LaceMain LaceMain;
        public int phaseNumber = 1; 
        public bool phaseActive = true; //if phase transitioning is allowed

        public bool laceAttemptingCombo = false;

        public IEnumerator Start()
        {
            Instance = this;

            //yield return ResourceLoader.LoadBundleAssets();

            //gms boss scene
            LostAndChainedPlugin.Instance.SilkBossScene.Load();
            yield return LostAndChainedPlugin.Instance.SilkBossScene.Handle;

            if (LostAndChainedPlugin.Instance.SilkBossScene.Handle.OperationException != null)
            {
                Log.Error($"Error loading asset: {LostAndChainedPlugin.Instance.SilkBossScene.Handle.OperationException}");
                // No reason to continue the coroutine because there's no asset to spawn
                // you're so right, example code
                yield break;
            }


            GameObject clone = LostAndChainedPlugin.Instance.SilkBossScene.InstantiateAsset();
            try
            {

                GameObject gms = clone.Child("Silk Boss");

                IntroControl = new IntroControl(this, gameObject.Child("Intro Control"));
                GMSMain = new GMSMain(this, gms);
                Cocoon = new Cocoon(this, gameObject.Child("Abyss_Cocoon_Silk"));
                LaceMain = new LaceMain(this, gameObject.Child("Lost Lace Boss"));

                IntroControl.Init();
                GMSMain.Init();
                Cocoon.Init();
                LaceMain.Init();

                clone.SetActive(true);


                //Link HP
                gameObject.Child("Lost Lace Boss").GetComponent<HealthManager>().hp = Constants.PhaseValues.Phase1HP; //initial phase 1
                gms.GetComponent<HealthManager>().sendDamageTo = gameObject.Child("Lost Lace Boss").GetComponent<HealthManager>();

                //gameObject.Child("Lost Lace Boss").GetComponent<HealthManager>().OnDeath += LaceTakenDamage;
                gms.GetComponent<HealthManager>().TookDamage += GMSTakenDamage;
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }

        private void GMSTakenDamage()
        {
            GameObject lace = gameObject.Child("Lost Lace Boss");
            HealthManager manager = lace.GetComponent<HealthManager>();
            if (manager.hp <= 0 && phaseActive)
            {
                if (phaseNumber != 2)
                {
                    //Phase transition
                    phaseNumber++;
                    phaseActive = false;

                    PhaseTransition();
                }                
            }
        }

        public void LaceTakenDamage()
        {
            GameObject lace = gameObject.Child("Lost Lace Boss");
            HealthManager manager = lace.GetComponent<HealthManager>();

            if (manager.hp <= 0 && phaseActive)
            {
                if (phaseNumber == 2 || phaseNumber == 4)
                {
                    //Phase transition
                    phaseNumber++;
                    phaseActive = false;

                    PhaseTransition();
                }
            }
        }

        private void PhaseTransition()
        {
            //For effects that occur immediately.
            switch (phaseNumber)
            {
                case 1:
                    //what how
                    break;
                case 2:
                    //make gms pop here instantly
                    GMSMain._bossObject.LocateMyFSM("Phase Control").SetState("Stagger Hit");
                    //LaceMain.Phase2Shift();
                    break;
                case 3:
                    LaceMain.DieToPhase3();
                    GMSMain._bossObject.LocateMyFSM("Control").SetState("P3 Down");
                    GMSMain._bossObject.LocateMyFSM("Attack Control").SendEvent("ATTACK STOP");
                    //lace on the further away side
                    break;
                case 4:
                    //kill gms for realsies
                    GMSMain._bossObject.LocateMyFSM("Phase Control").SetState("Death Hit");
                    break;
            }
        }

        

        private void OnDestroy()
        {
            //ResourceLoader.UnloadManualBundles();
            if (!HeroController.instance.playerData.UnlockSilkFinalCutscene)
            {
                return;
            }
            HeroController.instance.playerData.UnlockSilkFinalCutscene = false; //removing the silk changes post p4

            HudCanvas.instance.gameObject.Child("Health").SetActive(true);
            HudCanvas.instance.gameObject.Child("Extras").SetActive(true);
            HudCanvas.instance.gameObject.Child("Tool Icons").SetActive(true);


            GameObject thread = HudCanvas.instance.gameObject.Child("Thread");
            if (thread == null) { return; }

            GameObject spool = thread.Child("Spool");
            if (spool == null) { return; }

            GameObject bindOrb = spool.Child("Bind Orb");
            if (bindOrb == null) { return; }

            bindOrb.GetComponent<MeshRenderer>().enabled = true;

            GameObject evolved = bindOrb.Child("Evolved");
            if (evolved == null) { return; }

            evolved.SetActive(false);
        }

    }
}
