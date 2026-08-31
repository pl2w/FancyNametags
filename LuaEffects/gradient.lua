startR = GetConfig("startR", 255, "Start color Red value (0-255)")
startG = GetConfig("startG", 0, "Start color Green value (0-255)")
startB = GetConfig("startB", 0, "Start color Blue value (0-255)")

finishR = GetConfig("finishR", 0, "Finish color Red value (0-255)")
finishG = GetConfig("finishG", 255, "Finish color Green value (0-255)")
finishB = GetConfig("finishB", 0, "Finish color Blue value (0-255)")

EffectName = "Gradient"

function AnimateCharacter(charIndex, vertexIndex)
    local start = Color32(startR, startG, startB, 255)
    local finish = Color32(finishR, finishG, finishB, 255)

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