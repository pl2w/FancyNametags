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
