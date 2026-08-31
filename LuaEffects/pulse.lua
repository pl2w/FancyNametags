base = GetConfig("base", 80, "Minimum base alpha level")
range = GetConfig("range", 175, "Alpha transparency variance range")
speed = GetConfig("speed", 3.0, "Speed of the breathing pulse animation")

EffectName = "Breathing Pulse"

function AnimateCharacter(charIndex, vertexIndex)
    local wave = (math.sin(GetTime() * speed) + 1) / 2
    local alpha = base + wave * range

    Colors[vertexIndex + 0] = Color32(100, 200, 255, alpha)
    Colors[vertexIndex + 1] = Color32(100, 200, 255, alpha)
    Colors[vertexIndex + 2] = Color32(100, 200, 255, alpha)
    Colors[vertexIndex + 3] = Color32(100, 200, 255, alpha)
end