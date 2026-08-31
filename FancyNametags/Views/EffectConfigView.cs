using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BepInEx.Configuration;
using ComputerInterface.Behaviors.UI;
using ComputerInterface.Enumerations;
using ComputerInterface.Extensions;
using ComputerInterface.Models;
using FancyNametags.Behaviours;
using UnityEngine;

namespace FancyNametags.Views;

public class EffectConfigView : ComputerView
{
    public static EffectConfigView Instance;

    private const int VisibleEntries = 6;

    private EffectEntry _entry;
    private List<(string Key, ConfigEntryBase Entry)> _configEntries = new();
    private readonly UISelectionHandler _selectionHandler;

    public EffectConfigView()
    {
        Instance = this;

        _selectionHandler = new UISelectionHandler(EKeyboardButton.Up, EKeyboardButton.Down, EKeyboardButton.Enter);
        _selectionHandler.ConfigureSelectionIndicator("<color=#ed6540>> </color>", "", "  ", "");
    }

    public override void OnViewShown(object[] arguments)
    {
        base.OnViewShown(arguments);

        if (arguments == null || arguments.Length == 0 || arguments[0] is not EffectEntry entry)
        {
            ReturnToMainMenu();
            return;
        }

        _entry = entry;
        _configEntries = NameEffectRegistry.GetConfigEntries(entry).ToList();
        _selectionHandler.MaxIndex = Math.Max(0, _configEntries.Count - 1);
        _selectionHandler.CurrentSelectionIndex = 0;
    }

    private string BuildHeader()
    {
        return new StringBuilder()
            .BeginCenter()
            .MakeBar('=', ScreenWidth, 0)
            .AppendLine($"\nConfigure Effect - {_entry?.EffectName ?? ""}")
            .MakeBar('=', ScreenWidth, 0)
            .EndAlign()
            .AppendLines(1)
            .ToString();
    }

    protected override string GetViewText()
    {
        var sb = new StringBuilder(BuildHeader());

        if (_configEntries.Count == 0)
        {
            sb.BeginColor(Color.gray)
              .AppendLine("This effect has no configurable settings.")
              .EndAlign();
        }
        else
        {
            int keyWidth = _configEntries.Max(e => e.Key.Length);
            
            int scrollOffset = Math.Clamp(
                _selectionHandler.CurrentSelectionIndex - (VisibleEntries - 1),
                0,
                Math.Max(0, _configEntries.Count - VisibleEntries));
            scrollOffset = Math.Min(scrollOffset, _selectionHandler.CurrentSelectionIndex);

            int start = scrollOffset;
            int end = Math.Min(start + VisibleEntries, _configEntries.Count);

            if (start > 0)
                sb.BeginColor(Color.gray).AppendLine("^").EndAlign();

            for (int i = start; i < end; i++)
            {
                var (key, entry) = _configEntries[i];
                string paddedKey = key.PadRight(keyWidth);
                string line = $"<color=grey>{paddedKey}</color>  <color=#ed6540>{FormatValue(entry)}</color>";
                sb.AppendLine(_selectionHandler.GetIndicatedText(i, line));
            }

            if (end < _configEntries.Count)
                sb.BeginColor(Color.gray).AppendLine("v").EndAlign();

            string description = GetSelectedDescription();
            if (!string.IsNullOrEmpty(description))
            {
                sb.AppendLine()
                  .MakeBar('-', ScreenWidth, 0)
                  .AppendLines(1)
                  .BeginColor(Color.gray)
                  .Append(description)
                  .EndAlign();
            }
        }

        return sb.ToString();
    }

    private string GetSelectedDescription()
    {
        int index = _selectionHandler.CurrentSelectionIndex;
        if (index < 0 || index >= _configEntries.Count)
            return null;

        var entry = _configEntries[index].Entry;
        return entry?.Description?.Description;
    }

    public override void OnButtonPressed(EKeyboardButton key)
    {
        if (key == EKeyboardButton.Back)
        {
            ReturnToPreviousView();
            return;
        }

        if (_configEntries.Count == 0) return;

        if (key == EKeyboardButton.Left || key == EKeyboardButton.Right)
        {
            AdjustValue(_configEntries[_selectionHandler.CurrentSelectionIndex].Entry, key == EKeyboardButton.Right);
            UpdateViewScreen();
            return;
        }

        if (_selectionHandler.HandleButtonPress(key))
        {
            UpdateViewScreen();
        }
    }

    private static string FormatValue(ConfigEntryBase entry) => entry switch
    {
        ConfigEntry<bool> b => b.Value ? "On" : "Off",
        ConfigEntry<int> i => i.Value.ToString(),
        ConfigEntry<float> f => f.Value.ToString("0.###"),
        ConfigEntry<double> d => d.Value.ToString("0.###"),
        ConfigEntry<string> s => s.Value,
        _ => entry.BoxedValue?.ToString() ?? ""
    };

    private void AdjustValue(ConfigEntryBase entry, bool increase)
    {
        switch (entry)
        {
            case ConfigEntry<bool> b:
                b.Value = !b.Value;
                break;
            case ConfigEntry<int> i:
                i.Value += increase ? 1 : -1;
                break;
            case ConfigEntry<float> f:
                f.Value += increase ? 0.1f : -0.1f;
                break;
            case ConfigEntry<double> d:
                d.Value += increase ? 0.1 : -0.1;
                break;
        }

        RefreshActiveEffect();
    }

    private void RefreshActiveEffect()
    {
        var controller = NameEffectControllerRegistry.LocalController;
        if (controller == null || _entry == null) return;

        if (controller.VertexEffect != null && controller.VertexEffect.EffectId == _entry.Id)
            controller.VertexEffect.ApplyConfig();

        if (controller.ColorEffect != null && controller.ColorEffect.EffectId == _entry.Id
                                           && controller.ColorEffect != controller.VertexEffect)
            controller.ColorEffect.ApplyConfig();
    }
}