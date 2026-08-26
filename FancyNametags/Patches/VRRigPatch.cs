using HarmonyLib;
using FancyNametags.Effects;

namespace FancyNametags;

[HarmonyPatch(typeof(VRRig), "OnEnable")]
public static class VRRigOnEnablePatch
{
    public static void Postfix(VRRig __instance)
    {
        //var effect = __instance.playerText1.gameObject.AddComponent<TextBobber>();
        //effect.Initialize(__instance.playerText1, __instance);
        //
        //var effect2 = __instance.playerText1.gameObject.AddComponent<ColorWave>();
        //effect2.Initialize(__instance.playerText1, __instance);
        
        var effect3 = __instance.playerText1.gameObject.AddComponent<TextGlitch>();
        effect3.Initialize(__instance.playerText1, __instance);
    }
}