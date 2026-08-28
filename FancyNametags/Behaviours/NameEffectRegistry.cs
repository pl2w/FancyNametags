using System;
using System.Collections.Generic;
using System.Linq;
using FancyNametags.Effects;

namespace FancyNametags.Behaviours;

public static class NameEffectRegistry
{
    private static readonly List<EffectEntry> _entries = new();
    public static IReadOnlyList<EffectEntry> Entries => _entries;

    public static void Register(string displayName, Type effectComponentType)
    {
        if (effectComponentType == null || !typeof(BaseNameEffect).IsAssignableFrom(effectComponentType))
            throw new ArgumentException($"{effectComponentType} must derive from BaseNameEffect", nameof(effectComponentType));

        if (_entries.Exists(e => e.EffectComponentType == effectComponentType))
            return;

        _entries.Add(new EffectEntry(displayName, effectComponentType));
    }
    
    public static bool TryGetById(string id, out EffectEntry entry)
    {
        entry = Entries.FirstOrDefault(e => e.EffectComponentType.FullName == id);
        return entry != null;
    }

    public static string GetId(Type effectComponentType) => effectComponentType?.FullName;

    public static void RegisterDefaults()
    {
        Register("Color Wave", typeof(ColorWave));
        Register("Bobber", typeof(TextBobber));
        Register("Glitch", typeof(TextGlitch));
        Register("Pulse", typeof(TextPulse));
        Register("Rainbow", typeof(TextRainbow));
    }
}