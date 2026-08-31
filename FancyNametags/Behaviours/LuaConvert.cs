using System;
using MoonSharp.Interpreter;
using UnityEngine;

namespace FancyNametags.Effects;

public static class LuaConvert
{
    public static DynValue Vector3(Script script, Vector3 v)
    {
        var t = new Table(script)
        {
            ["x"] = v.x,
            ["y"] = v.y,
            ["z"] = v.z
        };
        return DynValue.NewTable(t);
    }

    public static DynValue Color32(Script script, Color32 c)
    {
        var t = new Table(script)
        {
            ["r"] = (int)c.r,
            ["g"] = (int)c.g,
            ["b"] = (int)c.b,
            ["a"] = (int)c.a
        };
        return DynValue.NewTable(t);
    }

    public static DynValue Color32(Script script, Color c) => Color32(script, (Color32)c);

    public static Vector3 FromLuaVector3(DynValue v)
    {
        if (v.Type != DataType.Table) throw new ScriptRuntimeException("expected a vector table with x, y, z");
        var t = v.Table;
        return new Vector3((float)t.Get("x").Number, (float)t.Get("y").Number, (float)t.Get("z").Number);
    }

    public static Color32 FromLuaColor32(DynValue v)
    {
        if (v.Type != DataType.Table) throw new ScriptRuntimeException("expected a color table with r, g, b, a");
        var t = v.Table;
        return new Color32(
            (byte)t.Get("r").Number,
            (byte)t.Get("g").Number,
            (byte)t.Get("b").Number,
            (byte)(t.Get("a").IsNil() ? 255 : t.Get("a").Number));
    }

    public static Table ArrayProxy<T>(Script script, T[] array, Func<Script, T, DynValue> toLua, Func<DynValue, T> fromLua)
    {
        var proxy = new Table(script);
        var meta = new Table(script);

        meta["__index"] = DynValue.NewCallback((ctx, args) =>
        {
            int idx = (int)args[1].Number;
            if (idx < 0 || idx >= array.Length) return DynValue.Nil;
            return toLua(script, array[idx]);
        });

        meta["__newindex"] = DynValue.NewCallback((ctx, args) =>
        {
            int idx = (int)args[1].Number;
            if (idx >= 0 && idx < array.Length)
                array[idx] = fromLua(args[2]);
            return DynValue.Nil;
        });

        proxy.MetaTable = meta;
        return proxy;
    }
}