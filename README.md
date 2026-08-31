# Fancy Nametags
> Makes your nametag fancy!

---

## Installation

1. Ensure you have the latest version of BepInEx 5 installed.
2. Make sure you have the latest version of Computer Interface installed.
3. Download the latest version from the [Releases](https://github.com/pl2w/FancyNametags/releases/latest) page.
4. Move the mod into your plugins folder.

---

## Usage

FancyNametags adds a **"Fancy Names"** entry to Computer Interface. Open it to browse and apply effects to your own nametag.

**Controls**

| Button | Action |
| --- | --- |
| `Up` / `Down` | Move the selection cursor |
| `Left` / `Right` | Switch pages |
| `Enter` | Toggle the selected effect on/off |
| `Option 1` | Clear all active effects |
| `Option 2` | Force the selected effect onto every nametag you see, locally, regardless of what effect that player has chosen (press again to remove the override) |
| `Back` | Return to the main menu |

An effect is shown in green while it's active on your nametag, and Lua-based effects are tagged `[LUA]` in the list.

Effects come in two flavors, and you can have one of each active at the same time:
- **Vertex effects** – move or distort the characters of your name (e.g. *Bobber*).
- **Color effects** – change the color of your name (e.g. *Rainbow*, *Color Wave*).

Selecting an effect that occupies the same slot as your current one will replace it. Your last selected effects are saved to the BepInEx config and re-applied automatically the next time you launch the game. Effects are networked to others who have the mod and effect installed.

### Built-in effects
- Color Wave
- Bobber
- Glitch
- Pulse
- Rainbow

### Lua effects

FancyNametags also loads any `.lua` file placed in the `LuaEffects` folder next to the plugin DLL (`BepInEx/plugins/FancyNametags/LuaEffects`).

A minimal effect looks like this:

```lua
EffectName = "My Fancy Name Effect"

function AnimateCharacter(charIndex, vertexIndex)
    -- called once per character, every frame, for every character in the nametag
    local color32 = HSVToRGB((GetTime() * 0.3 + charIndex * 0.1) % 1, 1, 1)
    color32.a = 255
    Colors[vertexIndex + 0] = color32
    Colors[vertexIndex + 1] = color32
    Colors[vertexIndex + 2] = color32
    Colors[vertexIndex + 3] = color32
end
```

Each character is made up of 4 vertices (`vertexIndex` through `vertexIndex + 3`), one per corner of the character quad. Set `Colors[...]` to change color, `Vertices[...]` to move/distort the character, or both.

Available globals inside a script:

| Name | Description |
| --- | --- |
| `EffectName` | (string, required) Display name shown in the menu |
| `AnimateCharacter(charIndex, vertexIndex)` | (function, required) Called every frame for every character |
| `ShouldAnimateThisFrame()` | (function, optional) Return `false` to skip animating entirely on the current frame (e.g. to throttle an expensive effect to every other frame). Evaluated once per frame. Defaults to always animating if not defined |
| `Colors` / `Vertices` | Arrays for the current frame you can read/write into |
| `GetCharacterCount()` | Total number of characters in the nametag |
| `GetTime()` | Current time in seconds, for animating |
| `Color32(r, g, b, a)` | Construct a color from 0–255 RGBA components |
| `HSVToRGB(h, s, v)` | Construct a color from hue/saturation/value |
| `Vector3(x, y, z)` | Construct a 3D vector |
| `Log(message)` | Print to the BepInEx console, useful for debugging |

---

## Disclaimer

This product is not affiliated with Another Axiom Inc. or its videogames Gorilla Tag and Orion Drift and is not endorsed or otherwise sponsored by Another Axiom.
Portions of the materials contained herein are property of Another Axiom. ©2021 Another Axiom Inc.
