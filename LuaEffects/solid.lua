local r = 255
local g = 50
local b = 50

EffectName = "Solid Red"

function AnimateCharacter(charIndex, vertexIndex)
    Colors[vertexIndex + 0] = Color32(r, g, b, 255)
    Colors[vertexIndex + 1] = Color32(r, g, b, 255)
    Colors[vertexIndex + 2] = Color32(r, g, b, 255)
    Colors[vertexIndex + 3] = Color32(r, g, b, 255)
end
