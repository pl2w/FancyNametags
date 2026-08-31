shimmerSpeed = GetConfig("shimmerSpeed", 1.5, "Speed of the light band sweeping across text")
shimmerWidth = GetConfig("shimmerWidth", 2.5, "Width of the shimmer highlight band in characters")

EffectName = "Shimmer"

function AnimateCharacter(charIndex, vertexIndex)
    local total = math.max(1, GetCharacterCount())
    local center = (GetTime() * shimmerSpeed) % total
    local dist = math.abs(charIndex - center)

    local intensity = 1 - math.min(1, dist / shimmerWidth)
    if intensity < 0 then
        intensity = 0
    end

    local r = math.floor(40 + 215 * intensity)
    local g = math.floor(180 + 75 * intensity)
    local b = math.floor(220 + 35 * intensity)
    local color = Color32(r, g, b, 255)

    Colors[vertexIndex + 0] = color
    Colors[vertexIndex + 1] = color
    Colors[vertexIndex + 2] = color
    Colors[vertexIndex + 3] = color
end