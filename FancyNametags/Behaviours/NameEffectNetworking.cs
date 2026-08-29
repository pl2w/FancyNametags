using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using FancyNametags.Effects;

namespace FancyNametags.Behaviours;

public static class NameEffectNetworking
{
    private const string VertexProp = "FN_Vertex";
    private const string ColorProp = "FN_Color";

    private static PropertyListener _listener;

    public static void Initialize()
    {
        if (_listener != null) return;
        _listener = new PropertyListener();
        PhotonNetwork.AddCallbackTarget(_listener);
    }

    public static void PublishLocalEffects(NameEffectController controller)
    {
        var props = new Hashtable
        {
            [VertexProp] = controller.VertexEffect?.EffectId ?? string.Empty,
            [ColorProp] = controller.ColorEffect?.EffectId ?? string.Empty
        };
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);
    }

    public static void ApplyFromProperties(Player photonPlayer, NameEffectController controller)
    {
        if (photonPlayer == null || controller == null) return;

        controller.ClearAllEffects();

        string vertexId = photonPlayer.CustomProperties.TryGetValue(VertexProp, out var v) ? v as string : null;
        string colorId = photonPlayer.CustomProperties.TryGetValue(ColorProp, out var c) ? c as string : null;

        BaseNameEffect vertexEffect = null;
        BaseNameEffect colorEffect = null;
        object vertexData = null;
        object colorData = null;

        if (!string.IsNullOrEmpty(vertexId) && NameEffectRegistry.TryGetById(vertexId, out var vEntry))
        {
            vertexEffect = controller.gameObject.AddComponent(vEntry.EffectComponentType) as BaseNameEffect;
            vertexEffect.EffectId = vEntry.Id;
            vertexData = vEntry.OptionalData;
        }
        else if (!string.IsNullOrEmpty(vertexId))
        {
            Plugin.Log.LogWarning($"Unknown vertex effect id: {vertexId}");
        }

        if (!string.IsNullOrEmpty(colorId))
        {
            if (colorId == vertexId && vertexEffect != null)
            {
                colorEffect = vertexEffect;
                colorData = vertexData;
            }
            else if (NameEffectRegistry.TryGetById(colorId, out var cEntry))
            {
                colorEffect = controller.gameObject.AddComponent(cEntry.EffectComponentType) as BaseNameEffect;
                colorEffect.EffectId = cEntry.Id;
                colorData = cEntry.OptionalData;
            }
            else
            {
                Plugin.Log.LogWarning($"Unknown color effect id: {colorId}");
            }
        }

        if (vertexEffect != null) controller.SetVertexEffect(vertexEffect, vertexData);
        if (colorEffect != null) controller.SetColorEffect(colorEffect, colorData);
    }

    public static void ApplyOverride(NameEffectController controller, string effectId)
    {
        if (controller == null) return;

        controller.ClearAllEffects();

        if (string.IsNullOrEmpty(effectId) || !NameEffectRegistry.TryGetById(effectId, out var entry)) return;

        var effect = controller.gameObject.AddComponent(entry.EffectComponentType) as BaseNameEffect;
        if (effect == null) return;

        effect.EffectId = entry.Id;
        object data = entry.OptionalData;

        if (effect.ModifyVertices) controller.SetVertexEffect(effect, data);
        if (effect.ModifyColors) controller.SetColorEffect(effect, data);
    }

    private class PropertyListener : IInRoomCallbacks
    {
        public void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
        {
            if (targetPlayer == null || targetPlayer.IsLocal) return;

            if (!changedProps.ContainsKey(VertexProp) && !changedProps.ContainsKey(ColorProp))
                return;

            var netPlayer = NetPlayer.Get(targetPlayer);
            if (netPlayer == null || netPlayer.IsNull) return;

            if (NameEffectControllerRegistry.TryGet(netPlayer, out var controller))
            {
                if (NameEffectControllerRegistry.IsOverrideActive)
                    ApplyOverride(controller, NameEffectControllerRegistry.LocalOverrideId);
                else
                    ApplyFromProperties(targetPlayer, controller);
            }
        }

        public void OnPlayerEnteredRoom(Player newPlayer) { }
        public void OnPlayerLeftRoom(Player otherPlayer) { }
        public void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged) { }
        public void OnMasterClientSwitched(Player newMasterClient) { }
    }
}