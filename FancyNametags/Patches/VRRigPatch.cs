using HarmonyLib;
using FancyNametags.Effects;

namespace FancyNametags;

[HarmonyPatch(typeof(VRRig), "OnEnable")]
public static class VRRigOnEnablePatch
{
    public static void Postfix(VRRig __instance)
    {
        var nameTagObject = __instance.playerText1.gameObject;
        if (nameTagObject.GetComponent<NameEffectController>())
            return;

        var controller = nameTagObject.AddComponent<NameEffectController>();
        controller.Initialize(__instance.playerText1, __instance);

        var bobber = nameTagObject.AddComponent<TextBobber>();
        controller.SetVertexEffect(bobber);

        var wave = nameTagObject.AddComponent<ColorWave>();
        controller.SetColorEffect(wave);
    }
}