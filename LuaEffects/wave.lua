local amplitude = 0.3
local speed = 2.0
local wavelength = 1.2

EffectName = "Wave Bob"

function AnimateCharacter(charIndex, vertexIndex)
    local offset = math.sin(GetTime() * speed + charIndex * wavelength) * amplitude

    for i = 0, 3 do
        local vertex = Vertices[vertexIndex + i]
        vertex.y = vertex.y + offset
        Vertices[vertexIndex + i] = vertex
    end
end
