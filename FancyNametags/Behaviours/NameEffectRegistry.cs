using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FancyNametags.Effects;
#if !DISABLE_LUA
using MoonSharp.Interpreter;
#endif

namespace FancyNametags.Behaviours;

public static class NameEffectRegistry
{
    private static readonly List<EffectEntry> _entries = new();
    private static bool _defaultsRegistered;
    public static IReadOnlyList<EffectEntry> Entries => _entries;

    public static void Register(string displayName, Type effectComponentType, object data = null, string id = null)
    {
        if (effectComponentType == null || !typeof(BaseNameEffect).IsAssignableFrom(effectComponentType))
            throw new ArgumentException($"{effectComponentType} must derive from BaseNameEffect", nameof(effectComponentType));

        if (id is null)
            id = data is string luaFile
                ? $"lua:{Path.GetRelativePath(Path.GetDirectoryName(typeof(NameEffectRegistry).Assembly.Location), luaFile).Replace("\\", "/")}"
                : effectComponentType.FullName;

        if (_entries.Exists(e => e.Id == id))
            return;

        _entries.Add(new EffectEntry(displayName, effectComponentType, id, data));
    }

    public static bool TryGetById(string id, out EffectEntry entry)
    {
        entry = Entries.FirstOrDefault(e => e.Id == id);
        return entry != null;
    }

    public static void RegisterAllEffects()
    {
        if (_defaultsRegistered) return;
        _defaultsRegistered = true;

        Register("Color Wave", typeof(ColorWave));
        Register("Bobber", typeof(TextBobber));
        Register("Glitch", typeof(TextGlitch));
        Register("Pulse", typeof(TextPulse));
        Register("Rainbow", typeof(TextRainbow));

#if !DISABLE_LUA
        // lua
        string dllDir = Path.GetDirectoryName(typeof(NameEffectRegistry).Assembly.Location);
        string luaDir = Path.Combine(dllDir, "LuaEffects");
        Directory.CreateDirectory(luaDir);

        foreach (string file in Directory.GetFiles(luaDir, "*.lua", SearchOption.AllDirectories))
        {
            try
            {
                var script = LuaNameEffect.SafeScript();
                script.DoFile(file);

                DynValue nameValue = script.Globals.Get("EffectName");
                string name = nameValue.Type == DataType.String ? nameValue.String : null;

                if (string.IsNullOrEmpty(name))
                {
                    Plugin.Log.LogWarning($"Skipping {Path.GetFileName(file)}. No name field.");
                    continue;
                }

                Register(name, typeof(LuaNameEffect), file);
            }
            catch (ScriptRuntimeException ex)
            {
                Plugin.Log.LogWarning($"Skipping {Path.GetFileName(file)} because of {ex.Message}. Likely not a FancyNameEffect");
            }
            catch (SyntaxErrorException ex)
            {
                Plugin.Log.LogWarning($"Skipping {Path.GetFileName(file)} because of {ex.Message}. Likely not a FancyNameEffect");
            }
        }
#endif
    }
}