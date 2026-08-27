using System;

namespace FancyNametags.Behaviours;

public class EffectEntry(string effectName, Type effectComponentType)
{
    public string EffectName { get; } = effectName;
    public Type EffectComponentType { get; } = effectComponentType;
}
