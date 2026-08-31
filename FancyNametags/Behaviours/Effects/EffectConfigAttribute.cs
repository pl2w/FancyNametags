using System;

namespace FancyNametags.Effects;

[AttributeUsage(AttributeTargets.Field)]
public class EffectConfigAttribute(string description = null) : Attribute
{
    public string Description { get; } = description;
}