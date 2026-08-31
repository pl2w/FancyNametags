speed = GetConfig("speed", 2.0, "Speed of the shifting RGB channels")

EffectName = "RGB Channel"

function AnimateCharacter(charIndex, vertexIndex)
    local t = GetTime() * speed

    local r = math.floor((math.sin(t + charIndex) + 1) / 2 * 255)
    local g = math.floor((math.sin(t + charIndex * 2) + 1) / 2 * 255)
    local b = math.floor((math.sin(t + charIndex * 3) + 1) / 2 * 255)
    local color = Color32(r, g, b, 255)

    Colors[vertexIndex + 0] = color
    Colors[vertexIndex + 1] = color
    Colors[vertexIndex + 2] = color
    Colors[vertexIndex + 3] = color
end