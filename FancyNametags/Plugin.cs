using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using FancyNametags.Behaviours;
using HarmonyLib;

namespace FancyNametags;

[BepInPlugin(PluginInfo.Guid, PluginInfo.Name, PluginInfo.Version)]
public class Plugin : BaseUnityPlugin
{
    public static ManualLogSource Log { get; private set; }

    private Harmony _harmony;

    public void Awake()
    {
        Log = Logger;

        NameEffectRegistry.RegisterDefaults();

        _harmony = new Harmony(PluginInfo.Guid);
        _harmony.PatchAll(Assembly.GetExecutingAssembly());
    }
}

public static class PluginInfo
{
    public const string Guid = "xyz.pl2w.fancynametags";
    public const string Name = "FancyNametags";
    public const string Version = "1.0.0";
}
