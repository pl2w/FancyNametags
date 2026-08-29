EffectName = "Gradient"

function AnimateCharacter(charIndex, vertexIndex)
    local start = Color32(255, 0, 0, 255)
    local finish = Color32(0, 255, 0, 255)

    local total = math.max(1, GetCharacterCount())
    local t = charIndex / total

    local r = math.floor(start.r + (finish.r - start.r) * t)
    local g = math.floor(start.g + (finish.g - start.g) * t)
    local b = math.floor(start.b + (finish.b - start.b) * t)
    local color = Color32(r, g, b, 255)

    Colors[vertexIndex + 0] = color
    Colors[vertexIndex + 1] = color
    Colors[vertexIndex + 2] = color
    Colors[vertexIndex + 3] = color
end
