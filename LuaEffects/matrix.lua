trailLength = GetConfig("trailLength", 2, "Length of the matrix trail, in characters")
speed = GetConfig("speed", 4, "Speed of the falling matrix wave")

EffectName = "Matrix Rain"

function AnimateCharacter(charIndex, vertexIndex)
    local total = math.max(1, GetCharacterCount())
    local wave = (GetTime() * speed + charIndex) % (total + trailLength)
    local pos = total - 1 - charIndex
    local dist = wave - pos

    local intensity = 0
    if dist >= 0 and dist <= trailLength then
        intensity = 1 - dist / trailLength
    end

    local g = math.floor(255 * intensity)
    Colors[vertexIndex + 0] = Color32(0, g, 0, 255)
    Colors[vertexIndex + 1] = Color32(0, g, 0, 255)
    Colors[vertexIndex + 2] = Color32(0, g, 0, 255)
    Colors[vertexIndex + 3] = Color32(0, g, 0, 255)
end