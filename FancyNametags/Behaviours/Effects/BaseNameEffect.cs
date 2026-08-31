using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using BepInEx.Configuration;
using TMPro;
using UnityEngine;

namespace FancyNametags.Effects;

public abstract class BaseNameEffect : MonoBehaviour
{
    protected TMP_Text NameTag;
    protected VRRig Rig;

    public string EffectId;

    protected internal abstract bool ModifyVertices { get; }
    protected internal abstract bool ModifyColors { get; }

    private static readonly Dictionary<(Type, string), ConfigEntryBase> ConfigEntries = new();

    public virtual void Initialize(TMP_Text nametag, VRRig rig, object data = null)
    {
        NameTag = nametag;
        Rig = rig;
        ApplyConfig();
    }

    protected internal abstract void AnimateCharacter(
        int charIndex,
        int vertexIndex,
        TMP_CharacterInfo charInfo,
        Vector3[] vertices,
        Color32[] colors
    );

    protected internal virtual bool ShouldAnimateThisFrame() => true;

    private static readonly MethodInfo BindMethodDefinition = typeof(ConfigFile).GetMethod(
        nameof(ConfigFile.Bind),
        1,
        [typeof(string), typeof(string), Type.MakeGenericMethodParameter(0), typeof(ConfigDescription)]);
    
    private static IEnumerable<FieldInfo> GetConfigFields(Type effectType) =>
        effectType.GetFields(BindingFlags.Public | BindingFlags.Instance)
            .Where(f => f.GetCustomAttribute<EffectConfigAttribute>() != null);

    public static void BindConfig(ConfigFile config, Type effectType)
    {
        var go = new GameObject("__configDefaults") { hideFlags = HideFlags.HideAndDontSave };
        var defaults = (BaseNameEffect)go.AddComponent(effectType);

        foreach (var field in GetConfigFields(effectType))
        {
            var attr = field.GetCustomAttribute<EffectConfigAttribute>();
            var defaultValue = field.GetValue(defaults);

            var bindMethod = BindMethodDefinition.MakeGenericMethod(field.FieldType);
            var description = new ConfigDescription(attr.Description ?? string.Empty);
            var entry = (ConfigEntryBase)bindMethod.Invoke(config, [effectType.Name, field.Name, defaultValue, description]);

            ConfigEntries[(effectType, field.Name)] = entry;
        }

        Destroy(go);
    }

    public static IEnumerable<(string Key, ConfigEntryBase Entry)> GetConfigEntries(Type effectType)
    {
        foreach (var field in GetConfigFields(effectType))
        {
            if (ConfigEntries.TryGetValue((effectType, field.Name), out var entry))
                yield return (field.Name, entry);
        }
    }

    protected internal virtual void ApplyConfig()
    {
        var type = GetType();
        foreach (var field in GetConfigFields(type))
        {
            if (ConfigEntries.TryGetValue((type, field.Name), out var entry))
                field.SetValue(this, entry.BoxedValue);
        }
    }
}