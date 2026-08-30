using BepInEx.Configuration;

namespace FancyNametags;

public static class Configuration
{
    public static ConfigEntry<string> ActiveVertexEffectId;
    public static ConfigEntry<string> ActiveColorEffectId;

    public static void Initialize(ConfigFile config)
    {
        ActiveVertexEffectId = config.Bind("General", "Persistent Vertex Effect", string.Empty);
        ActiveColorEffectId = config.Bind("General", "Persistent Color Effect", string.Empty);
    }
}
