using System;
using TMPro;
using UnityEngine;

namespace FancyNametags.Effects;

#if !DISABLE_LUA
using NLua;

public class LuaNameEffect : BaseNameEffect
{
    protected internal override bool ModifyVertices => true;
    protected internal override bool ModifyColors => true;

    private Lua _state;
    private LuaFunction _luaAnimateCharacter;
    private LuaFunction _luaShouldAnimateThisFrame;
    private bool _initialized;
    private int _lastFrame = -1;
    private Vector3[] _lastVertices;
    private Color32[] _lastColors;

    private int _lastShouldAnimateFrame = -1;
    private bool _lastShouldAnimateResult = true;

    public override void Initialize(TMP_Text nametag, VRRig rig, object data = null)
    {
        base.Initialize(nametag, rig, data);

        if (data is not string luaFile)
        {
            Views.SelectView.Instance.ActiveError = "Unexpected error occured";
            return;
        }

        try
        {
            _state = SafeLua();
            _state.DoFile(luaFile);

            _state["Color32"] = (Func<double, double, double, double, Color32>)((r, g, b, a) => new Color32((byte)r, (byte)g, (byte)b, (byte)a));
            _state["HSVToRGB"] = (Func<float, float, float, Color32>)((h, s, v) => Color.HSVToRGB(h, s, v));
            _state["Vector3"] = (Func<float, float, float, Vector3>)((x, y, z) => new Vector3(x, y, z));

            _state["GetCharacterCount"] = () => NameTag.textInfo.characterCount;
            _state["GetTime"] = () => Time.time;
            _state["Log"] = (string message) => Plugin.Log.LogInfo(message);

            _luaAnimateCharacter = _state["AnimateCharacter"] as LuaFunction;

            if (_luaAnimateCharacter == null)
            {
                Views.SelectView.Instance.ActiveError = $"Lua script '{luaFile}' does not define AnimateCharacter";
                return;
            }
            
            _luaShouldAnimateThisFrame = _state["ShouldAnimateThisFrame"] as LuaFunction;

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
        {
            return _lastShouldAnimateResult;
        }

        _lastShouldAnimateFrame = Time.frameCount;

        try
        {
            object[] result = _luaShouldAnimateThisFrame.Call();
            bool shouldAnimate = result is { Length: > 0 } && result[0] switch
            {
                bool b => b,
                double d => d != 0,
                null => false,
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
            _state["Colors"] = colors;
            _state["Vertices"] = vertices;
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
        _state?.Dispose();
    }

    // danger list http://lua-users.org/wiki/SandBoxes
    public static Lua SafeLua()
    {
        var lua = new Lua();
        lua.DoString(@"
            luanet = nil
            import = nil
            os, io, package, debug = nil, nil, nil, nil
            require, module, dofile, loadfile, load, loadstring = nil, nil, nil, nil, nil, nil
            getmetatable, setmetatable, rawget, rawset, rawequal = nil, nil, nil, nil, nil
            collectgarbage, newproxy, getfenv, setfenv = nil, nil, nil, nil
            if string then string.dump = nil end
        ");
        return lua;
    }
}
#endif