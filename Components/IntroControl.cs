using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using Silksong.FsmUtil;
using System.Collections;
using TMProOld;
using UnityEngine;
using static TeamCherry.DebugMenu.DebugMenu;

namespace LostAndChained.Components
{
    internal class IntroControl
    {
        private GameObject IntroGameObject;
        private LaceBossScene MainScene;
        private PlayMakerFSM _fsm;

        public IntroControl(LaceBossScene scene, GameObject introControl)
        {
            IntroGameObject = introControl;
            MainScene = scene;

            _fsm = IntroGameObject.LocateMyFSM("Control");
        }

        public void Init()
        {
            AddFallToState("Lace Re-emerge");
            AddFallToState("Lace Emerge");

            _fsm.GetState("Lace Emerge").AddMethod(_ => { GameManager.instance.StartCoroutine(DelayRise()); });
            _fsm.GetState("Lace Re-emerge").AddMethod(_ => { GameManager.instance.StartCoroutine(DelayRise()); });
            _fsm.GetState("Lace Roar").AddMethod(_ => { MainScene.GMSMain.BossRoar(); });

            RemoveGMSScreams();

            PatchText();
        }

        private void PatchText()
        {
            GameObject scene = IntroGameObject.transform.parent.gameObject;
            GameObject titleText = scene.Child("Boss Title").Child("Title Text").Child("Silk_Title_Text");
            GameObject titleMain = titleText.Child("Title Main");
            GameObject titleSub = titleText.Child("Title Sub");

            titleMain.GetComponent<TextMeshPro>().text = "Lost";
            titleSub.GetComponent<TextMeshPro>().text = "And Chained";
        }

        private IEnumerator DelayRise()
        {
            yield return new WaitForSeconds(2.75f);
            MainScene.GMSMain.BossRise();
        }


        private void RemoveGMSScreams()
        {
            if (_fsm == null) { return; }

            _fsm.GetAction<AnimatorPlay>("Silk Scream", 0).enabled = false;
            _fsm.GetAction<AnimatorPlay>("Begin Battle", 3).enabled = false;
        }


        private void AddFallToState(string name)
        {
            if (_fsm == null) { return; }
            FsmState state = _fsm.GetState(name);

            if (state == null) { return; }

            state.AddMethod(_ =>
            {
                if (MainScene == null) { return; }
                MainScene.Cocoon.Fall();
            });
        }
    }
}
