using System;
using UnityEngine;
using FancyNametags.Effects;

namespace FancyNametags.Behaviours;

public class EffectDebugGUI : MonoBehaviour
{
    public bool IsOpen = true;

    private Vector2 _scrollPos;
    private Rect _windowRect = new Rect(20, 20, 320, 480);
    private string _statusMessage = "";

    private void OnGUI()
    {
        if (!IsOpen) return;

        _windowRect = GUILayout.Window(
            GetInstanceID(),
            _windowRect,
            DrawWindow,
            "Fancy Nametags — Effect Debug"
        );
    }

    private void DrawWindow(int id)
    {
        var controller = NameEffectControllerRegistry.LocalController;

        if (!controller)
        {
            GUILayout.Label("No local NameEffectController found.");
            GUI.DragWindow(new Rect(0, 0, 10000, 20));
            return;
        }

        GUILayout.BeginHorizontal();
        GUILayout.Label($"Vertex: {controller.VertexEffect?.EffectId ?? "<none>"}");
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        GUILayout.Label($"Color:  {controller.ColorEffect?.EffectId ?? "<none>"}");
        GUILayout.EndHorizontal();

        if (!string.IsNullOrEmpty(_statusMessage))
        {
            var c = GUI.color;
            GUI.color = Color.yellow;
            GUILayout.Label(_statusMessage);
            GUI.color = c;
        }

        GUILayout.Space(6);

        if (GUILayout.Button("Clear All Effects"))
        {
            controller.ClearAllEffects();
            NameEffectNetworking.PublishLocalEffects(controller);
            _statusMessage = "Cleared all effects.";
        }

        GUILayout.Space(6);
        GUILayout.Label($"Registered Effects ({NameEffectRegistry.Entries.Count})");

        _scrollPos = GUILayout.BeginScrollView(_scrollPos, GUILayout.Height(360));

        foreach (var entry in NameEffectRegistry.Entries)
        {
            bool isActive = IsEffectActive(controller, entry.Id);
            bool isLua = entry.EffectComponentType == typeof(LuaNameEffect);

            var prevColor = GUI.backgroundColor;
            GUI.backgroundColor = isActive ? Color.green : prevColor;

            string label = isLua ? $"[LUA] {entry.EffectName}" : entry.EffectName;

            if (GUILayout.Button(label))
            {
                ToggleEffect(controller, entry);
            }

            GUI.backgroundColor = prevColor;
        }

        GUILayout.EndScrollView();

        GUI.DragWindow(new Rect(0, 0, 10000, 20));
    }

    private void ToggleEffect(NameEffectController controller, EffectEntry entry)
    {
        try
        {
            if (IsEffectActive(controller, entry.Id))
            {
                DisableEffect(controller, entry.Id);
                NameEffectNetworking.PublishLocalEffects(controller);
                _statusMessage = $"Disabled '{entry.EffectName}'.";
                return;
            }

            var effectType = entry.EffectComponentType;
            var effect = controller.gameObject.AddComponent(effectType) as BaseNameEffect;
            if (effect == null)
            {
                _statusMessage = $"Failed to create effect '{entry.EffectName}'.";
                return;
            }

            effect.EffectId = entry.Id;
            object effectData = entry.OptionalData;

            var oldVertex = controller.VertexEffect;
            var oldColor = controller.ColorEffect;

            bool clearVertex = oldVertex &&
                ((effect.ModifyVertices && oldVertex.ModifyVertices) ||
                 (effect.ModifyColors && oldVertex.ModifyColors));

            bool clearColor = oldColor &&
                ((effect.ModifyVertices && oldColor.ModifyVertices) ||
                 (effect.ModifyColors && oldColor.ModifyColors));

            if (clearVertex) controller.ClearVertexEffect();
            if (clearColor) controller.ClearColorEffect();

            if (effect.ModifyVertices)
            {
                controller.SetVertexEffect(effect, effectData);
            }

            if (effect.ModifyColors)
            {
                controller.SetColorEffect(effect, effectData);
            }

            NameEffectNetworking.PublishLocalEffects(controller);
            _statusMessage = $"Enabled '{entry.EffectName}'.";
        }
        catch (Exception ex)
        {
            _statusMessage = $"Error: {ex.Message}";
            Debug.LogError($"[EffectDebugGUI] Toggle failed for '{entry.EffectName}': {ex}");
        }
    }

    private static bool IsEffectActive(NameEffectController controller, string effectId)
    {
        if (controller == null) return false;
        if (controller.VertexEffect != null && controller.VertexEffect.EffectId == effectId) return true;
        if (controller.ColorEffect != null && controller.ColorEffect.EffectId == effectId) return true;
        return false;
    }

    private static void DisableEffect(NameEffectController controller, string effectId)
    {
        bool vertexMatches = controller.VertexEffect != null && controller.VertexEffect.EffectId == effectId;
        bool colorMatches = controller.ColorEffect != null && controller.ColorEffect.EffectId == effectId;

        if (vertexMatches)
        {
            controller.ClearVertexEffect();
        }

        if (colorMatches && controller.ColorEffect != controller.VertexEffect)
        {
            controller.ClearColorEffect();
        }
    }
}