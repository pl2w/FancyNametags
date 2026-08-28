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
        _listener = new PropertyListener();
        PhotonNetwork.AddCallbackTarget(_listener);
    }

    public static void Deinitialize()
    {
        if (_listener != null)
            PhotonNetwork.RemoveCallbackTarget(_listener);
    }

    public static void PublishLocalEffects(NameEffectController controller)
    {
        var props = new Hashtable
        {
            [VertexProp] = NameEffectRegistry.GetId(controller.VertexEffect?.GetType()) ?? string.Empty,
            [ColorProp]  = NameEffectRegistry.GetId(controller.ColorEffect?.GetType()) ?? string.Empty
        };
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);
    }

    public static void ApplyFromProperties(Player photonPlayer, NameEffectController controller)
    {
        if (photonPlayer == null || controller == null) return;

        controller.ClearAllEffects();

        string vertexId = photonPlayer.CustomProperties.TryGetValue(VertexProp, out var v) ? v as string : null;
        string colorId  = photonPlayer.CustomProperties.TryGetValue(ColorProp, out var c) ? c as string : null;

        BaseNameEffect vertexEffect = null;
        BaseNameEffect colorEffect = null;

        if (!string.IsNullOrEmpty(vertexId) && NameEffectRegistry.TryGetById(vertexId, out var vEntry))
            vertexEffect = controller.gameObject.AddComponent(vEntry.EffectComponentType) as BaseNameEffect;

        if (!string.IsNullOrEmpty(colorId))
        {
            if (colorId == vertexId && vertexEffect != null)
                colorEffect = vertexEffect;
            else if (NameEffectRegistry.TryGetById(colorId, out var cEntry))
                colorEffect = controller.gameObject.AddComponent(cEntry.EffectComponentType) as BaseNameEffect;
        }

        if (vertexEffect != null) controller.SetVertexEffect(vertexEffect);
        if (colorEffect != null) controller.SetColorEffect(colorEffect);
    }

    private class PropertyListener : IInRoomCallbacks
    {
        public void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
        {
            if (!changedProps.ContainsKey(VertexProp) && !changedProps.ContainsKey(ColorProp))
                return;

            var netPlayer = NetPlayer.Get(targetPlayer);
            if (netPlayer == null || netPlayer.IsNull) return;

            if (NameEffectControllerRegistry.TryGet(netPlayer, out var controller))
                ApplyFromProperties(targetPlayer, controller);
        }

        public void OnPlayerEnteredRoom(Player newPlayer) { }
        public void OnPlayerLeftRoom(Player otherPlayer) { }
        public void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged) { }
        public void OnMasterClientSwitched(Player newMasterClient) { }
    }
}