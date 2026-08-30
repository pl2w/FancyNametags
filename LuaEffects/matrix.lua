local trailLength = 2

EffectName = "Matrix Rain"

function AnimateCharacter(charIndex, vertexIndex)
    local total = math.max(1, GetCharacterCount())
    local wave = (GetTime() * 4 + charIndex) % (total + trailLength)
    local pos = total - 1 - charIndex
    local dist = wave - pos

    local intensity = 0
    if dist >= 0 and dist <= trailLength then
        intensity = 1 - dist / trailLength
    end

    local g = math.floor(255 * intensity)
    local color = Color32(0, g, 0, 255)

    Colors[vertexIndex + 0] = color
    Colors[vertexIndex + 1] = color
    Colors[vertexIndex + 2] = color
    Colors[vertexIndex + 3] = color
end
