using System;
using NLua;
using TMPro;
using UnityEngine;

namespace FancyNametags.Effects;

public class LuaNameEffect : BaseNameEffect
{
    protected internal override bool ModifyVertices => false;
    protected internal override bool ModifyColors => true;

    private Lua _state;
    private LuaFunction _luaAnimateCharacter;

    public override void Initialize(TMP_Text nametag, VRRig rig, object data) 
    {
        base.Initialize(nametag, rig, data);
        if (data is not string luaFile)
        {
            Views.SelectView.Instance.ActiveError = "Unexpected error occured";
            return;
        }

        _state = SafeLua();
        _state.DoFile(luaFile);

        _state["Color32"] = (Func<double, double, double, double, Color32>)((r, g, b, a) => new Color32((byte)r, (byte)g, (byte)b, (byte)a));
        _state["HSVToRGB"] = (Func<float, float, float, Color32>)((h, s, v) => (Color32)Color.HSVToRGB(h, s, v));

        _state["GetCharacterCount"] = () => NameTag.textInfo.characterCount;
        _state["GetTime"] = () => Time.time;
        _state["Log"] = (string message) => Plugin.Log.LogInfo(message);
        _state["HSVToRGB"] = (float hue, float saturation, float brightness) => (Color32)Color.HSVToRGB(hue, saturation, brightness);

        _luaAnimateCharacter = (LuaFunction)_state["AnimateCharacter"];
    }

    protected internal override void AnimateCharacter(
        int charIndex,
        int vertexIndex,
        TMP_CharacterInfo charInfo,
        Vector3[] vertices,
        Color32[] colors)
    {

         _state["Colors"] = colors;
         _state["Vertices"] = vertices;

         _luaAnimateCharacter.Call(charIndex, vertexIndex);
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
