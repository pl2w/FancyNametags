local base = 80
local range = 175
local speed = 3.0

EffectName = "Breathing Pulse"

function AnimateCharacter(charIndex, vertexIndex)
    local wave = (math.sin(GetTime() * speed) + 1) / 2
    local alpha = base + wave * range

    Colors[vertexIndex + 0] = Color32(100, 200, 255, alpha)
    Colors[vertexIndex + 1] = Color32(100, 200, 255, alpha)
    Colors[vertexIndex + 2] = Color32(100, 200, 255, alpha)
    Colors[vertexIndex + 3] = Color32(100, 200, 255, alpha)
end
