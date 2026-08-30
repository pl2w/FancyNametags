EffectName = "Glitch Flicker"

local function rand()
    return math.random()
end

function AnimateCharacter(charIndex, vertexIndex)
    local color
    if rand() > 0.5 then
        local r = math.floor(rand() * 255)
        local g = math.floor(rand() * 255)
        local b = math.floor(rand() * 255)
        color = Color32(r, g, b, 255)
    else
        color = Color32(30, 30, 30, 255)
    end

    Colors[vertexIndex + 0] = color
    Colors[vertexIndex + 1] = color
    Colors[vertexIndex + 2] = color
    Colors[vertexIndex + 3] = color
end
