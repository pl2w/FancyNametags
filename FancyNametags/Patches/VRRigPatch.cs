using System.Linq;
using FancyNametags.Behaviours;
using HarmonyLib;

namespace FancyNametags;

[HarmonyPatch(typeof(VRRig), "OnEnable")]
public static class VRRigOnEnablePatch
{
    public static void Postfix(VRRig __instance)
    {
        var nameTagObject = __instance.playerText1.gameObject;
        var controller = nameTagObject.GetComponent<NameEffectController>();
        if (controller == null)
            controller = nameTagObject.AddComponent<NameEffectController>();

        controller.Initialize(__instance.playerText1, __instance);

        if (__instance.isLocal)
        {
            NameEffectControllerRegistry.LocalController = controller;
            if (NameEffectControllerRegistry.IsOverrideActive)
                NameEffectNetworking.ApplyOverride(controller, NameEffectControllerRegistry.LocalOverrideId);
        
            // auto set effects
            if (controller.VertexEffect is null && controller.ColorEffect is null)
            {
                string vertexId = Configuration.ActiveVertexEffectId.Value;
                string colorId = Configuration.ActiveColorEffectId.Value;
                bool sameEntry = !string.IsNullOrEmpty(vertexId) && vertexId == colorId;
        
                if (sameEntry && NameEffectRegistry.Entries.FirstOrDefault(e => e.Id == vertexId) is EffectEntry combined)
                {
                    Plugin.Log.LogInfo("Applying saved name effect (combined)");
                    var effect = controller.gameObject.AddComponent(combined.EffectComponentType) as Effects.BaseNameEffect;
                    effect.EffectId = combined.Id;
                    if (effect.ModifyVertices) controller.SetVertexEffect(effect, combined.OptionalData);
                    if (effect.ModifyColors) controller.SetColorEffect(effect, combined.OptionalData);
                }
                else
                {
                    if (!string.IsNullOrEmpty(vertexId) && NameEffectRegistry.Entries.FirstOrDefault(e => e.Id == vertexId) is EffectEntry vEntry)
                    {
                        Plugin.Log.LogInfo("Applying saved vertex effect");
                        var vEffect = controller.gameObject.AddComponent(vEntry.EffectComponentType) as Effects.BaseNameEffect;
                        vEffect.EffectId = vEntry.Id;
                        controller.SetVertexEffect(vEffect, vEntry.OptionalData);
                    }
        
                    if (!string.IsNullOrEmpty(colorId) && NameEffectRegistry.Entries.FirstOrDefault(e => e.Id == colorId) is EffectEntry cEntry)
                    {
                        Plugin.Log.LogInfo("Applying saved color effect");
                        var cEffect = controller.gameObject.AddComponent(cEntry.EffectComponentType) as Effects.BaseNameEffect;
                        cEffect.EffectId = cEntry.Id;
                        controller.SetColorEffect(cEffect, cEntry.OptionalData);
                    }
                }
        
                if (controller.VertexEffect != null || controller.ColorEffect != null)
                    NameEffectNetworking.PublishLocalEffects(controller);
            }
        
            return;
        }

        NameEffectControllerRegistry.Register(__instance.Creator, controller);

        var photonPlayer = __instance.Creator?.GetPlayerRef();
        if (photonPlayer != null)
        {
            if (NameEffectControllerRegistry.IsOverrideActive)
                NameEffectNetworking.ApplyOverride(controller, NameEffectControllerRegistry.LocalOverrideId);
            else
                NameEffectNetworking.ApplyFromProperties(photonPlayer, controller);
        }
    }
}

[HarmonyPatch(typeof(VRRig), "OnDisable")]
public static class VRRigOnDisablePatch
{
    public static void Prefix(VRRig __instance)
    {
        NameEffectControllerRegistry.Unregister(__instance.Creator);
    }
}
