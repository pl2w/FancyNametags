using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;

namespace FancyNametags.Behaviours;

public static class NameEffectControllerRegistry
{
    private static readonly Dictionary<NetPlayer, NameEffectController> Controllers = new();

    public static NameEffectController LocalController;

    public static string LocalOverrideId { get; private set; }
    public static string LocalOverrideName { get; private set; }
    public static bool IsOverrideActive => !string.IsNullOrEmpty(LocalOverrideId);

    public static void Register(NetPlayer player, NameEffectController controller)
    {
        if (player == null) return;
        Controllers[player] = controller;
    }

    public static void Unregister(NetPlayer player)
    {
        if (player == null) return;
        Controllers.Remove(player);
    }

    public static bool TryGet(NetPlayer player, out NameEffectController controller)
        => Controllers.TryGetValue(player, out controller);

    public static void SetLocalOverride(string effectId)
    {
        LocalOverrideId = effectId;
        LocalOverrideName = null;

        if (!string.IsNullOrEmpty(effectId) && NameEffectRegistry.TryGetById(effectId, out var entry))
            LocalOverrideName = entry.EffectName;

        foreach (var pair in Controllers)
            ApplyOverrideOrRestore(pair.Key, pair.Value);

        if (LocalController != null)
            ApplyOverrideOrRestore(null, LocalController);
    }

    private static void ApplyOverrideOrRestore(NetPlayer netPlayer, NameEffectController controller)
    {
        if (controller == null) return;

        if (IsOverrideActive)
        {
            NameEffectNetworking.ApplyOverride(controller, LocalOverrideId);
        }
        else
        {
            Player photonPlayer = netPlayer?.GetPlayerRef() ?? PhotonNetwork.LocalPlayer;
            NameEffectNetworking.ApplyFromProperties(photonPlayer, controller);
        }
    }
}