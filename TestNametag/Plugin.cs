using BepInEx;
using FancyNametags.Behaviours;
using FancyNametags.Effects;

namespace TestNametag;

[BepInPlugin(PluginInfo.Guid, PluginInfo.Name, PluginInfo.Version)]
[BepInDependency(FancyNametags.PluginInfo.Guid)]
public class Plugin : BaseUnityPlugin
{
    public Plugin()
    {
        NameEffectRegistry.Register("TestEffect", typeof(TestEffect));
    }
}

public static class PluginInfo
{
    public const string Guid = "xyz.pl2w.fancynametags.test";
    public const string Name = "TestNametag";
    public const string Version = "1.0.0";
}