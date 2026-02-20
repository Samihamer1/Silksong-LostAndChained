using HarmonyLib;
using HutongGames.PlayMaker;
using LostAndChained.Components;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace LostAndChained.Patches
{
    internal static class LostLacePatches
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlayMakerFSM), "Start")]
        private static void ModifyBoss(PlayMakerFSM __instance)
        {
            if (__instance.name == "Lost Lace Boss" && __instance.FsmName == "Control" &&
                __instance.gameObject.layer == LayerMask.NameToLayer("Enemies"))
            {
                __instance.gameObject.transform.parent.gameObject.AddComponent<LaceBossScene>(); //Adding to the Scene

                LostAndChainedPlugin.Instance.ModifyEvolvedHPBar(); //putting this here because I want to
            }
        }


        [HarmonyPrefix]
        [HarmonyPatch(typeof(GameManager), nameof(GameManager.Awake))]
        private static void InitResourceLoader(GameManager __instance)
        {
            //__instance.StartCoroutine(ResourceLoader.Init());
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(FsmState), nameof(FsmState.OnEnter))]
        private static void LogLaceState(FsmState __instance)
        {
            if (__instance.fsm.GameObject.name == "Lost Lace Boss" && __instance.fsm.name == "Control")
            {
                Log.Debug("Entered state: " + __instance.Name + " , layer " + __instance.fsm.GameObject.layer + ", splashed in " + __instance.fsm.GetFsmBool("Splashed In"));
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(PlayerData), nameof(PlayerData.AddHealth))] 
        private static void UpdateEvolvedOnAdd(PlayerData __instance)
        {
            LostAndChainedPlugin.Instance.UpdateEvolvedHP(__instance.health);
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(PlayerData), nameof(PlayerData.TakeHealth))]
        private static void UpdateEvolvedOnTake(PlayerData __instance)
        {
            LostAndChainedPlugin.Instance.UpdateEvolvedHP(__instance.health);
        }
    }
}
