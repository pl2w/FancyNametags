local speed = 2.0
local saturation = 1
local brightness = 1

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
