using HarmonyLib;
using PaniniPlugin.Panini;
using UnityEngine;

namespace PaniniPlugin.Patches;

[HarmonyPatch]
internal sealed class Patches
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(CameraController), nameof(CameraController.Awake))]
    static void AddPaniniCamera(CameraController __instance)
    {
        __instance.gameObject.AddComponent<CubeCamera>();
        
        var player = __instance.transform.parent.gameObject;
        player.AddComponent<PaniniCamera>();
        var cam = player.AddComponent<Camera>();
        cam.depth = 1f;
    }
    
    [HarmonyPostfix]
    [HarmonyPatch(typeof(CanvasController), nameof(CameraController.Awake))]
    static void AddWarpOption(CanvasController __instance)
    {
        __instance.gameObject.AddComponent<InjectConfig>();
    }
}