using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using UnityEngine;

namespace PaniniPlugin;

[BepInPlugin("me.wahfl2.plugins.panini", "Panini FOV", "0.0.1")]
[BepInProcess("ULTRAKILL.exe")]
public class Plugin : BaseUnityPlugin
{
    private Harmony _patcher;
    
    private void Awake()
    {
        ConfigSetup();
        
        _patcher = new Harmony("PaniniFOV");
        _patcher.PatchAll();
        
        Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
    }
    
    public static ConfigEntry<float> ConfigWarp;

    private void ConfigSetup()
    {
        ConfigWarp = Config.Bind(
            "General",
            "Warp",
            50f,
            "The amount of \"Panini\" warping to apply to your FOV"
        );
    }
}
