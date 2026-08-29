using System;

namespace FancyNametags.Behaviours;

public class EffectEntry(string effectName, Type effectComponentType, object data = null, string id = null)
{
    public string EffectName { get; } = effectName;
    public Type EffectComponentType { get; } = effectComponentType;
    public string Id { get; } = id;
#nullable enable
    public object? OptionalData { get; } = data;
}
