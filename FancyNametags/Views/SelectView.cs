using System.Text;
using ComputerInterface.Behaviors.UI;
using ComputerInterface.Enumerations;
using ComputerInterface.Extensions;
using ComputerInterface.Interfaces;
using ComputerInterface.Models;
using UnityEngine;
using System;
using FancyNametags.Effects;

namespace FancyNametags.Views;

public class SelectView : ComputerView
{
    public static SelectView Instance;

    private readonly string _header;

    private readonly Entry[] _effectEntries;
    private readonly UIElementPageHandler<Entry> _pageHandler;
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

        _effectEntries = [
            new Entry("Color Wave", typeof(ColorWave)),
            new Entry("Bobber", typeof(TextBobber)),
            new Entry("Glitch", typeof(TextGlitch)),
            new Entry("Pulse", typeof(TextPulse)),
            new Entry("Rainbow", typeof(TextRainbow)),
        ];

        _pageHandler = new UIElementPageHandler<Entry>(EKeyboardButton.Left, EKeyboardButton.Right);
        _pageHandler.SetElements(_effectEntries);
        _pageHandler.EntriesPerPage = 8;

        _selectionHandler = new UISelectionHandler(EKeyboardButton.Up, EKeyboardButton.Down, EKeyboardButton.Enter);
        _selectionHandler.ConfigureSelectionIndicator("<color=#ed6540>> </color>", "", "  ", "");
        _selectionHandler.MaxIndex = _effectEntries.Length - 1;
        _selectionHandler.OnSelected += SetEffect;
    }

    protected override string GetViewText()
    {
        var stringBuilder = new StringBuilder(_header);

        stringBuilder
            .BeginColor(Color.red)
            .AppendLine(ActiveError)
            .EndAlign();

        var controller = Behaviours.NameEffectRegistry.LocalController;
        _pageHandler.EnumerateElements((entry, relativeIndex) =>
        {
            int index = _pageHandler.GetAbsoluteIndex(_pageHandler.CurrentPage, relativeIndex);
            string color = controller.VertexEffect?.GetType() == entry.EffectComponentType || controller.ColorEffect?.GetType() == entry.EffectComponentType
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
            Behaviours.NameEffectRegistry.LocalController.ClearAllEffects();
            return;
        }

        if (_pageHandler.HandleButtonPress(key) || _selectionHandler.HandleButtonPress(key))
        {
            UpdateViewScreen();
        }
    }

    private void SetEffect(int index)
    {
        var controller = Behaviours.NameEffectRegistry.LocalController;

        var effectType = _effectEntries[index].EffectComponentType;
        var effect = controller.gameObject.AddComponent(effectType) as BaseNameEffect;

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
            controller.SetColorEffect(effect);
            controller.SetVertexEffect(effect);
        }
        else if (effect.ModifyVertices) controller.SetVertexEffect(effect);
        else if (effect.ModifyColors) controller.SetColorEffect(effect);
    }

    private class Entry(string effectName, Type effectComponentType)
    {
        public string EffectName { get; } = effectName;
        public Type EffectComponentType { get; } = effectComponentType;
    }
}

public class SelectViewEntry : IComputerViewEntry
{
    public string EntryName => "Fancy Names";
    public Type EntryComputerView => typeof(SelectView);
}
