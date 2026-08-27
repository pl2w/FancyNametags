using System.Collections.Generic;

namespace FancyNametags.Behaviours;

public static class NameEffectRegistry
{
    private static readonly Dictionary<NetPlayer, NameEffectController> Controllers = new();

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
}