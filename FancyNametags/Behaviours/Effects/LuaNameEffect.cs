using System;
using TMPro;
using UnityEngine;
using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Loaders;

namespace FancyNametags.Effects;

public class LuaNameEffect : BaseNameEffect
{
    protected internal override bool ModifyVertices => true;
    protected internal override bool ModifyColors => true;

    private Script _script;
    private Closure _luaAnimateCharacter;
    private Closure _luaShouldAnimateThisFrame;
    private bool _initialized;
    private int _lastFrame = -1;
    private Vector3[] _lastVertices;
    private Color32[] _lastColors;

    private int _lastShouldAnimateFrame = -1;
    private bool _lastShouldAnimateResult = true;

    public override void Initialize(TMP_Text nametag, VRRig rig, object data = null)
    {
        base.Initialize(nametag, rig, data);

        if (!rig || data is not string luaFile)
        {
            Views.SelectView.Instance.ActiveError = "Unexpected error occured";
            return;
        }

        try
        {
            _script = SafeScript();
            _script.DoFile(luaFile);

            _script.Globals["Color32"] = (Func<double, double, double, double, DynValue>)
                ((r, g, b, a) => LuaConvert.Color32(_script, new Color32((byte)r, (byte)g, (byte)b, (byte)a)));
            _script.Globals["HSVToRGB"] = (Func<float, float, float, DynValue>)
                ((h, s, v) => LuaConvert.Color32(_script, Color.HSVToRGB(h, s, v)));
            _script.Globals["Vector3"] = (Func<float, float, float, DynValue>)
                ((x, y, z) => LuaConvert.Vector3(_script, new Vector3(x, y, z)));

            _script.Globals["GetCharacterCount"] = (Func<int>)(() => NameTag.textInfo.characterCount);
            _script.Globals["GetTime"] = (Func<float>)(() => Time.time);
            _script.Globals["Log"] = (Action<string>)((message) => Plugin.Log.LogInfo(message));

            _script.Globals["GetRigPosition"] = (Func<DynValue>)(() => LuaConvert.Vector3(_script, Rig.transform.position));
            _script.Globals["GetRigVelocity"] = (Func<DynValue>)(() => LuaConvert.Vector3(_script, Rig.LatestVelocity()));
            _script.Globals["GetRigScale"] = (Func<float>)(() => Rig.scaleFactor);
            _script.Globals["IsRigLocal"] = (Func<bool>)(() => Rig.isLocal);
            _script.Globals["GetRigColor"] = (Func<DynValue>)(() => LuaConvert.Color32(_script, Rig.playerColor));
            _script.Globals["GetRigMaterialIndex"] = (Func<int>)(() => Rig.setMatIndex);
            _script.Globals["GetRigPlayerName"] = (Func<string>)(() => Rig.playerNameVisible);
            _script.Globals["GetSpeakingLoudness"] = (Func<float>)(() => Rig.SpeakingLoudness);
            _script.Globals["IsLocalPartyMember"] = (Func<bool>)(() => Rig.IsLocalPartyMember);

            var animateFn = _script.Globals.Get("AnimateCharacter");
            if (animateFn.Type != DataType.Function)
            {
                Views.SelectView.Instance.ActiveError = $"Lua script '{luaFile}' does not define AnimateCharacter";
                return;
            }
            _luaAnimateCharacter = animateFn.Function;

            var shouldAnimateFn = _script.Globals.Get("ShouldAnimateThisFrame");
            _luaShouldAnimateThisFrame = shouldAnimateFn.Type == DataType.Function ? shouldAnimateFn.Function : null;

            _initialized = true;
        }
        catch (Exception ex)
        {
            Plugin.Log.LogError($"Failed to load Lua nametag effect '{luaFile}': {ex}");
            Views.SelectView.Instance.ActiveError = "Failed to load Lua effect";
            _initialized = false;
        }
    }

    protected internal override bool ShouldAnimateThisFrame()
    {
        if (!_initialized) return false;
        if (_luaShouldAnimateThisFrame == null) return true;

        if (_lastShouldAnimateFrame == Time.frameCount)
            return _lastShouldAnimateResult;

        _lastShouldAnimateFrame = Time.frameCount;

        try
        {
            DynValue result = _luaShouldAnimateThisFrame.Call();
            bool shouldAnimate = result.Type switch
            {
                DataType.Boolean => result.Boolean,
                DataType.Number => result.Number != 0,
                DataType.Nil => false,
                _ => true
            };

            _lastShouldAnimateResult = shouldAnimate;
            return shouldAnimate;
        }
        catch (Exception ex)
        {
            Plugin.Log.LogError($"Lua ShouldAnimateThisFrame threw: {ex}");
            _initialized = false;
            _lastShouldAnimateResult = false;
            return false;
        }
    }

    protected internal override void AnimateCharacter(
        int charIndex,
        int vertexIndex,
        TMP_CharacterInfo charInfo,
        Vector3[] vertices,
        Color32[] colors)
    {
        if (!_initialized) return;

        if (_lastFrame != Time.frameCount || !ReferenceEquals(_lastColors, colors) || !ReferenceEquals(_lastVertices, vertices))
        {
            _lastFrame = Time.frameCount;
            _lastColors = colors;
            _lastVertices = vertices;

            _script.Globals["Vertices"] = LuaConvert.ArrayProxy(_script, vertices, LuaConvert.Vector3, LuaConvert.FromLuaVector3);
            _script.Globals["Colors"] = LuaConvert.ArrayProxy(_script, colors, LuaConvert.Color32, LuaConvert.FromLuaColor32);
        }

        try
        {
            _luaAnimateCharacter.Call(charIndex, vertexIndex);
        }
        catch (Exception ex)
        {
            Plugin.Log.LogError($"Lua AnimateCharacter threw: {ex}");
            _initialized = false;
        }
    }

    private void OnDestroy()
    {
        _script = null;
    }

    public static Script SafeScript()
    {
        var script = new Script(CoreModules.Preset_HardSandbox)
        {
            Options =
            {
                ScriptLoader = new FileSystemScriptLoader()
            }
        };
        return script;
    }
}