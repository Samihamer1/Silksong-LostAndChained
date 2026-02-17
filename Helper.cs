using Silksong.FsmUtil;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LostAndChained
{
    public static class Helper
    {
        public static GameObject Child(this GameObject go, string name)
        {
            return go.transform.Find(name).gameObject;
        }

        public static List<GameObject> GetAllDescendants(GameObject root)
        {
            List<GameObject> result = new List<GameObject>();
            Stack<Transform> stack = new Stack<Transform>();
            stack.Push(root.transform);

            while (stack.Count > 0)
            {
                Transform current = stack.Pop();

                foreach (Transform child in current)
                {
                    result.Add(child.gameObject);
                    stack.Push(child);
                }
            }

            return result;
        }

        public static void CreateTempRoar(GameObject spawnpoint)
        {
            if (GameManager.instance.gameCams.gameObject.transform.Find("Roar Wave Emitter").gameObject == null) { return; }
            GameObject temproar = GameObject.Instantiate(GameManager.instance.gameCams.gameObject.transform.Find("Roar Wave Emitter").gameObject);
            temproar.name = "Temp Roar";

            PlayMakerFSM control = temproar.GetFsmPreprocessed("Control");
            control.SetState("Init");
            control.RemoveGlobalTransition("ROAR WAVE END");

            temproar.transform.parent = spawnpoint.transform;
            temproar.transform.localPosition = Vector3.zeroVector;
        }

        public static void StartTempRoar(GameObject spawnpoint)
        {
            GameObject temproar = spawnpoint.Child("Temp Roar");
            if (temproar == null) { return; }

            PlayMakerFSM control = temproar.GetFsmPreprocessed("Control");
            control.SetState("Emit Start");
        }

        public static void EndTempRoar(GameObject spawnpoint)
        {
            GameObject temproar = spawnpoint.Child("Temp Roar");
            if (temproar != null)
            {
                PlayMakerFSM control = temproar.LocateMyFSM("Control");
                control.SendEvent("END");
            }
        }


        public static void CloneAnimation(tk2dSpriteAnimation library, string originalName, string newName, float fpsModifier)
        {
            tk2dSpriteAnimationClip orig = library.GetClipByName(originalName);
            tk2dSpriteAnimationClip clone = new tk2dSpriteAnimationClip();
            clone.CopyFrom(orig);
            clone.fps = orig.fps * fpsModifier;
            clone.name = newName;

            List<tk2dSpriteAnimationClip> newList = library.clips.ToList();
            newList.Add(clone);

            library.clips = newList.ToArray();

            library.isValid = false;
            library.ValidateLookup();
        }

    }
}
