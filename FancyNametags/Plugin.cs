using System.Reflection;
using BepInEx;
using HarmonyLib;

namespace FancyNametags;

[BepInPlugin(PluginInfo.Guid, PluginInfo.Name, PluginInfo.Version)]
public class Plugin : BaseUnityPlugin
{
    private Harmony _harmony;

    public void Awake()
    {
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