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

            // auto set effect
            if (controller.VertexEffect is null && controller.ColorEffect is null && NameEffectRegistry.Entries.FirstOrDefault(entry => entry.Id == Configuration.ActiveEffectId.Value) is EffectEntry entry)
            {
                Plugin.Log.LogInfo("Applying saved name effect");
                var effect = controller.gameObject.AddComponent(entry.EffectComponentType) as Effects.BaseNameEffect;
                effect.EffectId = entry.Id;
                if (effect.ModifyVertices) controller.SetVertexEffect(effect, entry.OptionalData);
                if (effect.ModifyColors) controller.SetColorEffect(effect, entry.OptionalData);
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
