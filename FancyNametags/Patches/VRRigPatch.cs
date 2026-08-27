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
            NameEffectRegistry.LocalController = controller;
            return;
        }

        NameEffectRegistry.Register(__instance.Creator, controller);
    }
}

[HarmonyPatch(typeof(VRRig), "OnDisable")]
public static class VRRigOnDisablePatch
{
    public static void Prefix(VRRig __instance)
    {
        NameEffectRegistry.Unregister(__instance.Creator);
    }
}