using BepInEx.Configuration;

namespace FancyNametags;

public static class Configuration
{
    public static ConfigEntry<string> ActiveEffectId;

    public static void Initialize(ConfigFile config)
    {
        ActiveEffectId = config.Bind("General", "Persistant Effect", string.Empty);
    }
}
