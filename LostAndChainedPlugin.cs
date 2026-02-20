using BepInEx;
using HarmonyLib;
using LostAndChained.Patches;
using Silksong.AssetHelper.ManagedAssets;
using UnityEngine;

namespace LostAndChained;


[BepInAutoPlugin(id: "io.github.samihamer1.lostandchained")]
[BepInDependency("org.silksong-modding.fsmutil")]
[BepInDependency("org.silksong-modding.assethelper")]
public partial class LostAndChainedPlugin : BaseUnityPlugin
{
    private static Harmony _harmony = null!;
    public static LostAndChainedPlugin? Instance;

    //There's so few assets I need that I can't be bothered setting up the resourceloader to work with this. Maybe later.
    public ManagedAsset<GameObject> SilkBossScene;
    public ManagedAsset<AudioClip> SilkNeedleAntic;
    public ManagedAsset<AudioClip> SilkNeedleThrow1;
    public ManagedAsset<AudioClip> SilkNeedleThrow2;
    public ManagedAsset<AudioClip> SilkNeedleImpact1;
    public ManagedAsset<AudioClip> SilkNeedleImpact2;
    public ManagedAsset<AudioClip> SilkNeedleImpact3;

    public ManagedAsset<MusicCue> hornetTheme;

    private void Awake()
    {
        _harmony = new Harmony("io.github.samihamer1.lostandchained");

        Instance = this;

        _harmony.PatchAll(typeof(LostLacePatches));

        Logger.LogInfo($"Plugin {Name} ({Id}) has loaded!");

        //DebugTools.DumpGameObjectPaths("Cradle_03", true);
        SilkBossScene = ManagedAsset<GameObject>.FromSceneAsset("Cradle_03", "Boss Scene");
        SilkNeedleAntic = ManagedAsset<AudioClip>.FromNonSceneAsset("Assets/Audio/SFX/HornetEnemy/Silk Boss/silk_boss_needle_antic.wav", "sfxstatic_assets_areacradle");
        SilkNeedleThrow1 = ManagedAsset<AudioClip>.FromNonSceneAsset("Assets/Audio/SFX/HornetEnemy/Silk Boss/silk_boss_needle_throw_1.wav", "sfxstatic_assets_areacradle");
        SilkNeedleThrow2 = ManagedAsset<AudioClip>.FromNonSceneAsset("Assets/Audio/SFX/HornetEnemy/Silk Boss/silk_boss_needle_throw_2.wav", "sfxstatic_assets_areacradle");
        SilkNeedleImpact1 = ManagedAsset<AudioClip>.FromNonSceneAsset("Assets/Audio/SFX/HornetEnemy/Silk Boss/silk_boss_needle_impact_1.wav", "sfxstatic_assets_areacradle");
        SilkNeedleImpact2 = ManagedAsset<AudioClip>.FromNonSceneAsset("Assets/Audio/SFX/HornetEnemy/Silk Boss/silk_boss_needle_impact_2.wav", "sfxstatic_assets_areacradle");
        SilkNeedleImpact3 = ManagedAsset<AudioClip>.FromNonSceneAsset("Assets/Audio/SFX/HornetEnemy/Silk Boss/silk_boss_needle_impact_3.wav", "sfxstatic_assets_areacradle");
        hornetTheme = ManagedAsset<MusicCue>.FromNonSceneAsset("Assets/Audio/MusicCues/Cloak Battle.asset", "audiocuesdynamic_assets_areaslab");
    }

    public void UpdateEvolvedHP(int newHP)
    {
        GameObject thread = HudCanvas.instance.gameObject.Child("Thread");
        if (thread == null) { return; }

        GameObject spool = thread.Child("Spool");
        if (spool == null) { return; }

        GameObject bindOrb = spool.Child("Bind Orb");
        if (bindOrb == null) { return; }

        GameObject evolved = bindOrb.Child("Evolved");
        if (evolved == null) { return; }

        GameObject fakeHealth = evolved.Child("Fake Health");
        if (fakeHealth == null) { return; }

        if (evolved.activeSelf)
        {
            IconCounter counter = fakeHealth.GetComponent<IconCounter>();
            int origHP = counter.currentValue;

            //1 since the first is a prefab
            for (int i = 1; i < fakeHealth.transform.childCount; i++)
            {
                GameObject hp = fakeHealth.transform.GetChild(i).gameObject;
                IconCounterItem counteritem = hp.GetComponent<IconCounterItem>();

                if (i <= newHP && i > origHP && newHP > origHP)
                {
                    hp.SetActive(false); //refreshing the gain hp animation, but only if its a newly added hp mask
                }
            }

            counter.SetCurrent(newHP);
         }

    }

    public void ModifyEvolvedHPBar()
    {
        GameObject thread = HudCanvas.instance.gameObject.Child("Thread");
        if (thread == null) { return; }

        GameObject spool = thread.Child("Spool");
        if (spool == null) { return; }

        GameObject bindOrb = spool.Child("Bind Orb");
        if (bindOrb == null) { return; }

        GameObject evolved = bindOrb.Child("Evolved");
        if (evolved == null) { return; }

        GameObject fakeHealth = evolved.Child("Fake Health");
        if (fakeHealth == null) { return; }

        GameObject health = fakeHealth.Child("Health");
        IconCounterItem counteritem = health.GetComponent<IconCounterItem>();
        counteritem.inactiveDisable = false;
        counteritem.inactiveState.Color = new Color(0, 0, 0, 1);
    }



    private void OnDestroy()
    {
        _harmony.UnpatchSelf();
        //ResourceLoader.UnloadAll();
    }

    public void LogInfo(object message)
    {
        Logger.LogInfo($"[{Name}][Info]: " + message);
    }

    public void LogError(object message)
    {
        Logger.LogError($"[{Name}][Error]: " + message);
    }

    public void LogDebug(object message)
    {
        Logger.LogDebug($"[{Name}][Debug]: " + message);
    }

    public void LogWarn(object message)
    {
        Logger.LogWarning($"[{Name}][Warning]: " + message);
    }
}
