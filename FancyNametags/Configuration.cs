using BepInEx.Configuration;

namespace FancyNametags;

public static class Configuration
{
    public static ConfigFile Config { get; private set; }

    public static ConfigEntry<string> ActiveVertexEffectId;
    public static ConfigEntry<string> ActiveColorEffectId;

    public static void Initialize(ConfigFile config)
    {
        Config = config;

        ActiveVertexEffectId = config.Bind("General", "Persistent Vertex Effect", string.Empty);
        ActiveColorEffectId = config.Bind("General", "Persistent Color Effect", string.Empty);
    }
}