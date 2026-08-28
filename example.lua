local speed = 0.3
local saturation = 1
local brightness = 1
local hueSpread = 1

EffectName = "My Fancy Name Effect"

function AnimateCharacter (charIndex, vertexIndex)
    local totalChars = GetCharacterCount()
    local charOffset = charIndex / math.max(1, totalChars - 1)

    local hue = (GetTime() * speed - charOffset * hueSpread) % 1
    if (hue < 0) then
        hue = hue + 1
    end

    local color32 = HSVToRGB(hue, saturation, brightness)
    color32.a = 255

    Colors[vertexIndex + 0] = color32
    Colors[vertexIndex + 1] = color32
    Colors[vertexIndex + 2] = color32
    Colors[vertexIndex + 3] = color32

    return Colors
end
