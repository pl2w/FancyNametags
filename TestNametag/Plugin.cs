using BepInEx;

namespace TestNametag;

[BepInPlugin(PluginInfo.Guid, PluginInfo.Name, PluginInfo.Version)]
public class Plugin : BaseUnityPlugin;

public static class PluginInfo
{
    public const string Guid = "xyz.pl2w.fancynametags.test";
    public const string Name = "TestNametag";
    public const string Version = "1.0.0";
}