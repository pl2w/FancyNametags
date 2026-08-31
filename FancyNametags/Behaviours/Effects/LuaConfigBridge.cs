using System.Collections.Generic;
using BepInEx.Configuration;
using MoonSharp.Interpreter;

namespace FancyNametags.Effects;

public static class LuaConfigBridge
{
    private static readonly Dictionary<string, ConfigEntryBase> Entries = new();

    public static DynValue GetOrBind(ConfigFile config, string section, string key, DynValue defaultValue, string description)
    {
        var entryKey = $"{section}::{key}";

        if (Entries.TryGetValue(entryKey, out var existing))
            return ToDynValue(existing);

        ConfigEntryBase entry = defaultValue.Type switch
        {
            DataType.Boolean => config.Bind(section, key, defaultValue.Boolean, new ConfigDescription(description ?? string.Empty)),
            DataType.Number => config.Bind(section, key, defaultValue.Number, new ConfigDescription(description ?? string.Empty)),
            DataType.String => config.Bind(section, key, defaultValue.String, new ConfigDescription(description ?? string.Empty)),
            _ => throw new ScriptRuntimeException($"unsupported default type '{defaultValue.Type}' for key '{key}'")
        };

        Entries[entryKey] = entry;
        return ToDynValue(entry);
    }

    public static IEnumerable<(string Key, ConfigEntryBase Entry)> GetEntriesForSection(string section)
    {
        string prefix = section + "::";
        foreach (var kvp in Entries)
        {
            if (kvp.Key.StartsWith(prefix))
                yield return (kvp.Key[prefix.Length..], kvp.Value);
        }
    }

    public static DynValue ToDynValue(ConfigEntryBase entry) => entry switch
    {
        ConfigEntry<bool> b => DynValue.NewBoolean(b.Value),
        ConfigEntry<double> d => DynValue.NewNumber(d.Value),
        ConfigEntry<string> s => DynValue.NewString(s.Value),
        _ => DynValue.Nil
    };
}