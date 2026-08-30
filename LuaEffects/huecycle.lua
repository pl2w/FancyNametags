local speed = 0.4
local saturation = 1
local brightness = 1

EffectName = "Hue Cycle"

function AnimateCharacter(charIndex, vertexIndex)
    local hue = (GetTime() * speed) % 1
    local color = HSVToRGB(hue, saturation, brightness)

    Colors[vertexIndex + 0] = color
    Colors[vertexIndex + 1] = color
    Colors[vertexIndex + 2] = color
    Colors[vertexIndex + 3] = color
end
