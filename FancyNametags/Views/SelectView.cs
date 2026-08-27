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

    private string header;

    private Entry[] effectEntries;
    private UIElementPageHandler<Entry> pageHandler;
    private UISelectionHandler selectionHandler;

    public string ActiveError;

    public SelectView()
    {
        Instance = this;

        header = new StringBuilder()
            .BeginCenter()
            .MakeBar('=', ScreenWidth, 0)
            .AppendLine("\nFancy Nametags <size=60%>by pl2w</size>")
            .MakeBar('=', ScreenWidth, 0)
            .EndAlign()
            .AppendLines(1)
            .ToString();

        effectEntries = [
            new("Color Wave", typeof(Effects.ColorWave)),
            new("Bobber", typeof(Effects.TextBobber)),
            new("Glitch", typeof(Effects.TextGlitch)),
            new("Pulse", typeof(Effects.TextPulse)),
            new("Rainbow", typeof(Effects.TextRainbow)),
        ];

        pageHandler = new UIElementPageHandler<Entry>(EKeyboardButton.Left, EKeyboardButton.Right);
        pageHandler.SetElements(effectEntries);
        pageHandler.EntriesPerPage = 8;

        selectionHandler = new UISelectionHandler(EKeyboardButton.Up, EKeyboardButton.Down, EKeyboardButton.Enter);
        selectionHandler.ConfigureSelectionIndicator("<color=#ed6540>> </color>", "", "  ", "");
        selectionHandler.MaxIndex = effectEntries.Length - 1;
        selectionHandler.OnSelected += SetEffect;
    }

    protected override string GetViewText()
    {
        var stringBuilder = new StringBuilder(header);

        stringBuilder
            .BeginColor(Color.red)
            .AppendLine(ActiveError)
            .EndAlign();

        var controller = Behaviours.NameEffectRegistry.LocalController;
        pageHandler.EnumerateElements((entry, relativeIndex) => {
            int index = pageHandler.GetAbsoluteIndex(pageHandler.CurrentPage, relativeIndex);
            string color = controller.VertexEffect?.GetType() == entry.EffectComponentType || controller.ColorEffect?.GetType() == entry.EffectComponentType 
                ? "green" 
                : "white";
            string text = selectionHandler.GetIndicatedText(index, $"<color={color}>{entry.EffectName}</color>");
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

        if (pageHandler.HandleButtonPress(key) || selectionHandler.HandleButtonPress(key))
        {
            UpdateViewScreen();
        }
    }

    private void SetEffect(int index)
    {
        var controller = Behaviours.NameEffectRegistry.LocalController;

        var effectType = effectEntries[index].EffectComponentType;
        var effect = controller.gameObject.AddComponent(effectType) as BaseNameEffect;

        if (effect.ModifyVertices)
        {
            if (controller.VertexEffect?.GetType() == effectType) controller.ClearVertexEffect();
            else controller.SetVertexEffect(effect);
        }
        else
        {
            if (controller.ColorEffect?.GetType() == effectType) controller.ClearColorEffect();
            else controller.SetColorEffect(effect);
        }
    }

    private record class Entry(string EffectName, Type EffectComponentType);
}

public class SelectViewEntry : IComputerViewEntry
{
    public string EntryName => "Fancy Names";
    public System.Type EntryComputerView => typeof(SelectView);
}
