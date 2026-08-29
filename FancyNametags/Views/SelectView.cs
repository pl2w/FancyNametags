using System.Text;
using ComputerInterface.Behaviors.UI;
using ComputerInterface.Enumerations;
using ComputerInterface.Extensions;
using ComputerInterface.Interfaces;
using ComputerInterface.Models;
using UnityEngine;
using System;
using System.Linq;
using FancyNametags.Behaviours;
using FancyNametags.Effects;

namespace FancyNametags.Views;

public class SelectView : ComputerView
{
    public static SelectView Instance;

    private readonly string _header;

    private readonly UIElementPageHandler<EffectEntry> _pageHandler;
    private readonly UISelectionHandler _selectionHandler;

    public string ActiveError;

    public SelectView()
    {
        Instance = this;

        _header = new StringBuilder()
            .BeginCenter()
            .MakeBar('=', ScreenWidth, 0)
            .AppendLine("\nFancy Nametags <size=60%>by pl2w</size>")
            .MakeBar('=', ScreenWidth, 0)
            .EndAlign()
            .AppendLines(1)
            .ToString();

        _pageHandler = new UIElementPageHandler<EffectEntry>(EKeyboardButton.Left, EKeyboardButton.Right);
        _pageHandler.EntriesPerPage = 8;
        _selectionHandler = new UISelectionHandler(EKeyboardButton.Up, EKeyboardButton.Down, EKeyboardButton.Enter);
        _selectionHandler.ConfigureSelectionIndicator("<color=#ed6540>> </color>", "", "  ", "");
        _selectionHandler.OnSelected += SetEffect;

        RefreshEntries();
    }

    public override void OnViewShown(object[] arguments)
    {
        base.OnViewShown(arguments);
        RefreshEntries();
    }

    private void RefreshEntries()
    {
        _pageHandler.SetElements(NameEffectRegistry.Entries.ToArray());
        _selectionHandler.MaxIndex = Math.Max(0, NameEffectRegistry.Entries.Count - 1);
    }

    protected override string GetViewText()
    {
        var stringBuilder = new StringBuilder(_header);

        stringBuilder
            .BeginColor(Color.red)
            .AppendLine(ActiveError)
            .EndAlign();

        var controller = NameEffectControllerRegistry.LocalController;
        _pageHandler.EnumerateElements((entry, relativeIndex) =>
        {
            int index = _pageHandler.GetAbsoluteIndex(_pageHandler.CurrentPage, relativeIndex);
            string color = controller != null && (
                controller.VertexEffect?.GetType() == entry.EffectComponentType ||
                controller.ColorEffect?.GetType() == entry.EffectComponentType)
                ? "green"
                : "white";
            string text = _selectionHandler.GetIndicatedText(index, $"<color={color}>{entry.EffectName}</color>");
            stringBuilder.AppendLine(text);
        });

        // pageHandler.AppendFooter(stringBuilder);
        return stringBuilder.ToString();
    }

    public override void OnButtonPressed(EKeyboardButton key)
    {
        ActiveError = string.Empty;

        if (key == EKeyboardButton.Back)
        {
            ReturnToMainMenu();
            return;
        }

        if (key == EKeyboardButton.Option1)
        {
            var controller = NameEffectControllerRegistry.LocalController;
            if (controller != null)
            {
                controller.ClearAllEffects();
                NameEffectNetworking.PublishLocalEffects(controller);
            }
            UpdateViewScreen();
            return;
        }

        if (_pageHandler.HandleButtonPress(key) || _selectionHandler.HandleButtonPress(key))
        {
            UpdateViewScreen();
        }
    }

    private void SetEffect(int index)
    {
        var controller = NameEffectControllerRegistry.LocalController;
        if (controller == null) return;

        var entry = NameEffectRegistry.Entries[index];
        var effectType = entry.EffectComponentType;

        if (IsEffectActive(controller, effectType))
        {
            DisableEffect(controller, effectType);
            NameEffectNetworking.PublishLocalEffects(controller);
            return;
        }

        var effect = controller.gameObject.AddComponent(effectType) as BaseNameEffect;
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

        if (effect.ModifyVertices && effect.ModifyColors)
        {
            controller.SetColorEffect(effect, effectData);
            controller.SetVertexEffect(effect, effectData);
        }
        else if (effect.ModifyVertices) controller.SetVertexEffect(effect, effectData);
        else if (effect.ModifyColors) controller.SetColorEffect(effect, effectData);

        NameEffectNetworking.PublishLocalEffects(controller);
    }

    private static bool IsEffectActive(NameEffectController controller, Type effectType)
    {
        if (controller.VertexEffect != null && controller.VertexEffect.GetType() == effectType) return true;
        if (controller.ColorEffect != null && controller.ColorEffect.GetType() == effectType) return true;
        return false;
    }

    private static void DisableEffect(NameEffectController controller, Type effectType)
    {
        bool vertexMatches = controller.VertexEffect != null && controller.VertexEffect.GetType() == effectType;
        bool colorMatches = controller.ColorEffect != null && controller.ColorEffect.GetType() == effectType;

        if (vertexMatches) controller.ClearVertexEffect();
        if (colorMatches && controller.ColorEffect != controller.VertexEffect) controller.ClearColorEffect();
    }
}

public class SelectViewEntry : IComputerViewEntry
{
    public string EntryName => "Fancy Names";
    public Type EntryComputerView => typeof(SelectView);
}