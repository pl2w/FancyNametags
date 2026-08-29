EffectName = "Alternating"

function AnimateCharacter(charIndex, vertexIndex)
    local colorA = Color32(255, 0, 0, 255)
    local colorB = Color32(0, 0, 255, 255)

    local color
    if charIndex % 2 == 0 then
        color = colorA
    else
        color = colorB
    end

    Colors[vertexIndex + 0] = color
    Colors[vertexIndex + 1] = color
    Colors[vertexIndex + 2] = color
    Colors[vertexIndex + 3] = color
end
