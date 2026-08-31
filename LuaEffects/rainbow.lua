speed = GetConfig("speed", 2.0, "Speed of the rainbow scroll animation")
saturation = GetConfig("saturation", 1, "Saturation of the rainbow spectrum")
brightness = GetConfig("brightness", 1, "Brightness of the rainbow spectrum")

EffectName = "Scrolling Rainbow"

function AnimateCharacter(charIndex, vertexIndex)
    local total = math.max(1, GetCharacterCount())
    local hue = (GetTime() * speed + charIndex / total) % 1
    local color = HSVToRGB(hue, saturation, brightness)

    Colors[vertexIndex + 0] = color
    Colors[vertexIndex + 1] = color
    Colors[vertexIndex + 2] = color
    Colors[vertexIndex + 3] = color
end