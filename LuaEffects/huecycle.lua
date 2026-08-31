speed = GetConfig("speed", 0.4, "Speed of the hue transition cycle")
saturation = GetConfig("saturation", 1, "Saturation of the colors")
brightness = GetConfig("brightness", 1, "Brightness level of the colors")

EffectName = "Hue Cycle"

function AnimateCharacter(charIndex, vertexIndex)
    local hue = (GetTime() * speed) % 1
    local color = HSVToRGB(hue, saturation, brightness)

    Colors[vertexIndex + 0] = color
    Colors[vertexIndex + 1] = color
    Colors[vertexIndex + 2] = color
    Colors[vertexIndex + 3] = color
end