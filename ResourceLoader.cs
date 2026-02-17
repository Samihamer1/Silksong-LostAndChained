//using HutongGames.PlayMaker.Actions;
//using Silksong.AssetHelper.ManagedAssets;
//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Text;
//using UnityEngine;
//using UnityEngine.AddressableAssets;
//using UnityEngine.Android;
//using UnityEngine.ResourceManagement.AsyncOperations;
//using UnityEngine.SceneManagement;
//using Object = UnityEngine.Object;


//namespace LostAndChained
//{
//    internal static class ResourceLoader
//    {
//        private static readonly Dictionary<string, string[]> ScenePrefabs = new()
//        {
//            ["Cradle_03"] = ["Boss Scene"]
//        };

//        private static readonly Dictionary<string, string[]> BundleAssets = new()
//        {
//            ["tk2danimations_assets_areacradle"] = ["Silk Boss Anim"],
//            ["tk2dcollections_assets_areacradle"] = ["Silk Boss Cln"],
//            ["sfxstatic_assets_areacradle"] = ["silk_boss_needle_antic", "silk_boss_needle_throw_1", "silk_boss_needle_throw_2", "silk_boss_needle_impact_1", "silk_boss_needle_impact_2", "silk_boss_needle_impact_3"],
//        };


//        private static List<AssetBundle> _manuallyLoadedBundles = new();

//        private static readonly Dictionary<Type, Dictionary<string, IManagedAsset>> Assets = new();

//        internal static void Init()
//        {
//            //SilkBossScene = ManagedAsset<GameObject>.FromSceneAsset("Cradle_03", "Boss Scene");
//            InitSceneAssets();
//            //LoadNonSceneAssets();
//        }

//        internal static void InitSceneAssets()
//        {
//            foreach (string key in ScenePrefabs.Keys)
//            {
//                string[] assetPaths = ScenePrefabs[key];

//                foreach (string assetPath in assetPaths)
//                {
//                    ManagedAsset<GameObject> asset = ManagedAsset<GameObject>.FromSceneAsset(key, assetPath);

//                    if (!Assets.ContainsKey(typeof(ManagedAsset<GameObject>))) {
//                        Assets.Add(typeof(ManagedAsset<GameObject>), new Dictionary<string, IManagedAsset>());
//                    }

//                    Dictionary<string, IManagedAsset> subDict = Assets[typeof(ManagedAsset<GameObject>)];
//                    if (!subDict.ContainsKey(assetPath))
//                    {
//                        subDict.Add(assetPath, asset);
//                    }
//                    else
//                    {
//                        subDict[assetPath] = asset;
//                        Log.Warn($"There is already an asset \"{assetPath}\". Replacing with new variant.");
//                    }
//                }
//            }
//        }

//        internal static T? Get<T>(string assetName) where T : IManagedAsset
//        {
//            //gms boss scene
//            LostAndChainedPlugin.Instance.SilkBossScene.Load();
//            yield return LostAndChainedPlugin.Instance.SilkBossScene.Handle;

//            if (LostAndChainedPlugin.Instance.SilkBossScene.Handle.OperationException != null)
//            {
//                Log.Error($"Error loading asset: {LostAndChainedPlugin.Instance.SilkBossScene.Handle.OperationException}");
//                // No reason to continue the coroutine because there's no asset to spawn
//                yield break;
//            }

//            GameObject clone = LostAndChainedPlugin.Instance.SilkBossScene.InstantiateAsset();
//        }


//        /// <summary>
//        /// Manually load asset bundles.
//        /// </summary>
//        internal static IEnumerator Init()
//        {
//            yield return LoadScenePrefabs();
//        }

//        /// <summary>
//        /// Load all prefabs located in scenes.
//        /// </summary>
//        private static IEnumerator LoadScenePrefabs()
//        {
//            AudioManager.BlockAudioChange = true;
//            foreach (var (sceneName, prefabNames) in ScenePrefabs)
//            {
//                string loadScenePath = $"Scenes/{sceneName}";

//                var loadSceneHandle = Addressables.LoadSceneAsync(loadScenePath, LoadSceneMode.Additive);
//                yield return loadSceneHandle;

//                if (loadSceneHandle.Status == AsyncOperationStatus.Succeeded)
//                {
//                    var sceneInstance = loadSceneHandle.Result;
//                    var scene = sceneInstance.Scene;
//                    foreach (var rootObj in scene.GetRootGameObjects())
//                    {
//                        foreach (string prefabName in prefabNames)
//                        {
//                            GameObject? prefab = rootObj.GetComponentsInChildren<Transform>(true)
//                                .FirstOrDefault(obj => obj.name == prefabName)?.gameObject;
//                            if (prefab)
//                            {
//                                prefab.SetActive(false);
//                                var prefabCopy = Object.Instantiate(prefab);
//                                prefabCopy.name = prefabName;
//                                Object.DontDestroyOnLoad(prefabCopy);

//                                CheckSpecialBehaviour(prefabCopy);

//                                TryAdd(prefabCopy);
//                            }
//                        }
//                    }
//                }
//                else
//                {
//                    Log.Error(loadSceneHandle.OperationException);
//                }

//                var unloadSceneHandle =
//                    Addressables.UnloadSceneAsync(loadSceneHandle);
//                yield return unloadSceneHandle;
//            }

//            AudioManager.BlockAudioChange = false;
//        }

//        /// <summary>
//        /// Load all required assets located within loaded<see cref="AssetBundle">asset bundles</see>.
//        /// </summary>
//        internal static IEnumerator LoadBundleAssets()
//        {
//            string platformFolder = Application.platform switch
//            {
//                RuntimePlatform.WindowsPlayer => "StandaloneWindows64",
//                RuntimePlatform.OSXPlayer => "StandaloneOSX",
//                RuntimePlatform.LinuxPlayer => "StandaloneLinux64",
//                _ => ""
//            };
//            string bundlesPath = Path.Combine(Addressables.RuntimePath, platformFolder);
//            foreach (var (bundleName, assetNames) in BundleAssets)
//            {                
//                bool bundleAlreadyLoaded = false;

//                foreach (string assetName in assetNames) //foreach wanted asset
//                {
//                    bool assetFound = false;
//                    foreach (var loadedBundle in AssetBundle.GetAllLoadedAssetBundles()) //foreach loaded bundle
//                    {
//                        foreach (string assetPath in loadedBundle.GetAllAssetNames()) //check all objects for wanted asset
//                        {
//                            if (assetPath.GetAssetRoot() == assetName) //if object is found
//                            {
//                                bundleAlreadyLoaded = true;
//                                assetFound = true;

//                                var loadAssetRequest = loadedBundle.LoadAssetAsync(assetPath);
//                                yield return loadAssetRequest;

//                                var loadedAsset = loadAssetRequest.asset;


//                                if (loadedAsset is GameObject loadedPrefab)
//                                {
//                                    CheckSpecialBehaviour(loadedPrefab);
//                                }
//                                else if (loadedAsset)
//                                {
//                                    TryAdd(loadedAsset);
//                                }

//                                break;
//                            }
//                        }

//                        if (assetFound)
//                        {
//                            break;
//                        }
//                    }

//                }

//                if (bundleAlreadyLoaded)
//                {
//                    Log.Warn($"Bundle {bundleName} already loaded!");
//                    continue;
//                }

//                string bundlePath = Path.Combine(bundlesPath, $"{bundleName}.bundle");
//                var bundleLoadRequest = AssetBundle.LoadFromFileAsync(bundlePath);
//                yield return bundleLoadRequest;

//                AssetBundle bundle = bundleLoadRequest.assetBundle;
//                _manuallyLoadedBundles.Add(bundle);
//                foreach (string assetPath in bundle.GetAllAssetNames())
//                {
//                    foreach (string assetName in assetNames)
//                    {
//                        if (assetPath.GetAssetRoot() == assetName)
//                        {
//                            var assetLoadRequest = bundle.LoadAssetAsync(assetPath);
//                            yield return assetLoadRequest;

//                            var loadedAsset = assetLoadRequest.asset;
//                            if (loadedAsset is GameObject loadedPrefab)
//                            {
//                                CheckSpecialBehaviour(loadedPrefab);
//                            }

//                            TryAdd(loadedAsset);
//                        }
//                    }
//                }
//            }
//        }

//        /// <summary>
//        /// Get the root asset name from its path.
//        /// </summary>
//        /// <param name="assetPath">The full path to the asset.</param>
//        /// <returns>The trimmed asset name.</returns>
//        private static string GetAssetRoot(this string assetPath) =>
//            assetPath.Split("/").Last().Replace(".asset", "").Replace(".prefab", "").Replace(".wav", "");

//        /// <summary>
//        /// Whether an asset with a specified name is already loaded.
//        /// </summary>
//        /// <param name="assetName">The name of the asset to check.</param>
//        /// <returns></returns>
//        private static bool Has(string assetName)
//        {
//            foreach (var (_, subDict) in Assets)
//            {
//                foreach (var (name, existingAsset) in subDict)
//                {
//                    if (assetName == name && existingAsset)
//                    {
//                        return true;
//                    }
//                }
//            }

//            return false;
//        }

//        /// <summary>
//        /// Try adding a new asset.
//        /// </summary>
//        /// <param name="asset">The asset to add.</param>
//        private static void TryAdd<T>(T asset) where T : Object
//        {
//            var assetName = asset.name;
//            if (Has(assetName))
//            {
//                Log.Warn($"Asset \"{assetName}\" has already been loaded!");
//                return;
//            }

//            var assetType = asset.GetType();
//            if (Assets.ContainsKey(assetType))
//            {
//                var existingAssetSubDict = Assets[assetType];
//                if (existingAssetSubDict != null)
//                {
//                    if (existingAssetSubDict.ContainsKey(assetName))
//                    {
//                        var existingAsset = existingAssetSubDict[assetName];
//                        if (existingAsset != null)
//                        {
//                            Log.Warn($"There is already an asset \"{assetName}\" of type \"{assetType}\"!");
//                        }
//                        else
//                        {
//                            Log.Info(
//                                $"Key \"{assetName}\" for sub-dictionary of type \"{assetType}\" exists, but its value is null; Replacing with new asset...");
//                            Assets[assetType][assetName] = asset;
//                        }
//                    }
//                    else
//                    {
//                        Log.Debug($"Adding asset \"{assetName}\" of type \"{assetType}\".");
//                        Assets[assetType].Add(assetName, asset);
//                    }
//                }
//                else
//                {
//                    Log.Error($"Failed to get sub-dictionary of type \"{assetType}\"!");
//                    Assets.Add(assetType, new Dictionary<string, Object>());
//                }
//            }
//            else
//            {
//                Assets.Add(assetType, new Dictionary<string, Object> { [assetName] = asset });
//                Log.Debug(
//                    $"Added new sub-dictionary of type \"{assetType}\" with initial asset \"{assetName}\".");
//            }
//        }

//        /// <summary>
//        /// Check whether a loaded prefab should be modified in some way.
//        /// </summary>
//        /// <param name="prefab">The prefab to check.</param>
//        private static void CheckSpecialBehaviour(GameObject prefab)
//        {
//            switch (prefab.name)
//            {
//                case "":
//                    {
//                        break;
//                    }

//            }
//        }

//        /// <summary>
//        /// Unload bundles that were manually loaded for this mod.
//        /// </summary>
//        internal static void UnloadManualBundles()
//        {
//            foreach (var bundle in _manuallyLoadedBundles)
//            {
//                string bundleName = bundle.name;
//                var unloadBundleHandle = bundle.UnloadAsync(true);
//                unloadBundleHandle.completed += _ => { Log.Info($"Successfully unloaded bundle \"{bundleName}\""); };
//            }

//            _manuallyLoadedBundles.Clear();

//            foreach (var (_, obj) in Assets[typeof(GameObject)])
//            {
//                if (obj is GameObject gameObject && gameObject.activeSelf)
//                {
//                    Log.Info($"Recycling all instances of prefab \"{gameObject.name}\"");
//                    GameObject.Destroy(obj);
//                }
//            }
//        }

//        /// <summary>
//        /// Fetch an asset.
//        /// </summary>
//        /// <param name="assetName">The name of the asset to fetch.</param>
//        /// <typeparam name="T">The type of asset to fetch.</typeparam>
//        /// <returns>The fetched object if it exists, otherwise returns null.</returns>
//        internal static T? Get<T>(string assetName) where T : Object
//        {
//            Type assetType = typeof(T);
//            if (Assets.ContainsKey(assetType))
//            {
//                var subDict = Assets[assetType];
//                if (subDict != null)
//                {
//                    if (subDict.ContainsKey(assetName))
//                    {
//                        var assetObj = subDict[assetName];
//                        if (assetObj != null)
//                        {
//                            return assetObj as T;
//                        }

//                        Log.Error($"Failed to get asset \"{assetName}\"; asset is null!");
//                        return null;
//                    }

//                    Log.Error($"Sub-dictionary for type \"{assetType}\" does not contain key \"{assetName}\"!");
//                    return null;
//                }

//                Log.Error($"Failed to get asset \"{assetName}\"; sub-dictionary of key \"{assetType}\" is null!");
//                return null;
//            }

//            Log.Error($"Could not find a sub-dictionary of type \"{assetType}\"!");
//            return null;
//        }

//        /// <summary>
//        /// Unload all saved assets.
//        /// </summary>
//        internal static void UnloadAll()
//        {
//            foreach (var assetDict in Assets.Values)
//            {
//                foreach (var asset in assetDict.Values)
//                {
//                    Object.DestroyImmediate(asset);
//                }
//            }

//            Assets.Clear();
//            GC.Collect();
//        }
//    }


//}
