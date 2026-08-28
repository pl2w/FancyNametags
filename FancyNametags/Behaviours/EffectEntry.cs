using System;

namespace FancyNametags.Behaviours;

public class EffectEntry(string effectName, Type effectComponentType, object data = null)
{
    public string EffectName { get; } = effectName;
    public Type EffectComponentType { get; } = effectComponentType;
#nullable enable
    public object? OptionalData { get; } = data;
}
