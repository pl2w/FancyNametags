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
            .AppendLine("\nFancy Nametags <size=60%>by pl2w & crafterbot</size>")
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
        SyncSelectionToPage();
    }

    private void SyncSelectionToPage()
    {
        int entriesPerPage = _pageHandler.EntriesPerPage;
        int firstIndex = _pageHandler.CurrentPage * entriesPerPage;
        int lastIndex = Math.Min(firstIndex + entriesPerPage, NameEffectRegistry.Entries.Count) - 1;

        int relative = _selectionHandler.CurrentSelectionIndex % entriesPerPage;
        _selectionHandler.MaxIndex = lastIndex;
        _selectionHandler.CurrentSelectionIndex = Math.Min(firstIndex + relative, _selectionHandler.MaxIndex);
    }

    protected override string GetViewText()
    {
        var stringBuilder = new StringBuilder(_header);

        stringBuilder
            .BeginColor(Color.red)
            .AppendLine(ActiveError)
            .EndAlign();

        if (NameEffectControllerRegistry.IsOverrideActive)
            stringBuilder
                .BeginColor(Color.cyan)
                .AppendLine($"Forced: {NameEffectControllerRegistry.LocalOverrideName}")
                .EndAlign();

        var controller = NameEffectControllerRegistry.LocalController;
        _pageHandler.EnumerateElements((entry, relativeIndex) =>
        {
            int index = _pageHandler.GetAbsoluteIndex(_pageHandler.CurrentPage, relativeIndex);
            bool isLua = entry.EffectComponentType == typeof(LuaNameEffect);
            string color = IsEffectActive(controller, entry.Id) ? "green" : "white";
            string luaTag = isLua ? "<color=#00FFFF>[LUA] </color>" : "";
            string text = _selectionHandler.GetIndicatedText(index, $"<color={color}>{luaTag}{entry.EffectName}</color>");
            stringBuilder.AppendLine(text);
        });

        _pageHandler.AppendFooter(stringBuilder);
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

        if (key == EKeyboardButton.Option2)
        {
            ToggleLocalOverride();
            UpdateViewScreen();
            return;
        }

        int pageBefore = _pageHandler.CurrentPage;
        bool pageChanged = _pageHandler.HandleButtonPress(key);
        if (pageChanged)
        {
            if (_pageHandler.CurrentPage != pageBefore) SyncSelectionToPage();
            UpdateViewScreen();
            return;
        }

        if (_selectionHandler.HandleButtonPress(key))
        {
            UpdateViewScreen();
        }
    }

    private void ToggleLocalOverride()
    {
        if (NameEffectControllerRegistry.IsOverrideActive)
        {
            NameEffectControllerRegistry.SetLocalOverride(null);
            return;
        }

        var entry = NameEffectRegistry.Entries[_selectionHandler.CurrentSelectionIndex];
        NameEffectControllerRegistry.SetLocalOverride(entry.Id);
    }

    private void SetEffect(int index)
    {
        var controller = NameEffectControllerRegistry.LocalController;
        if (controller == null) return;
    
        var entry = NameEffectRegistry.Entries[index];
        var effectType = entry.EffectComponentType;
    
        if (IsEffectActive(controller, entry.Id))
        {
            DisableEffect(controller, entry.Id);
            NameEffectNetworking.PublishLocalEffects(controller);
            return;
        }
    
        var effect = controller.gameObject.AddComponent(effectType) as BaseNameEffect;
        if (effect == null)
        {
            Instance.ActiveError = $"Failed to create effect '{entry.EffectName}'";
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
            Configuration.ActiveVertexEffectId.Value = entry.Id;
        }
    
        if (effect.ModifyColors)
        {
            controller.SetColorEffect(effect, effectData);
            Configuration.ActiveColorEffectId.Value = entry.Id;
        }
    
        NameEffectNetworking.PublishLocalEffects(controller);
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
            Configuration.ActiveVertexEffectId.Value = string.Empty;
        }
    
        if (colorMatches && controller.ColorEffect != controller.VertexEffect)
        {
            controller.ClearColorEffect();
            Configuration.ActiveColorEffectId.Value = string.Empty;
        }
    }
}

public class SelectViewEntry : IComputerViewEntry
{
    public string EntryName => "Fancy Names";
    public Type EntryComputerView => typeof(SelectView);
}
