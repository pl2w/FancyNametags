using TMPro;
using UnityEngine;

namespace FancyNametags.Effects;

public class TextRainbow : BaseNameEffect
{
    [EffectConfig("Speed of the rainbow color cycle")]
    public float speed = 0.3f;

    [EffectConfig("Saturation of the colors (0 = grayscale, 1 = full color)")]
    public float saturation = 1f;

    [EffectConfig("Brightness of the rainbow colors")]
    public float brightness = 1f;

    [EffectConfig("Spread of the rainbow gradient across the characters")]
    public float hueSpread = 1f;

    protected internal override bool ModifyVertices => false;
    protected internal override bool ModifyColors => true;

    protected internal override void AnimateCharacter(
        int charIndex,
        int vertexIndex,
        TMP_CharacterInfo charInfo,
        Vector3[] vertices,
        Color32[] colors)
    {
        var totalChars = NameTag.textInfo.characterCount;
        var charOffset = (float)charIndex / Mathf.Max(1, totalChars - 1);

        var hue = (Time.time * speed - charOffset * hueSpread) % 1f;
        if (hue < 0f) hue += 1f;

        Color32 col = Color.HSVToRGB(hue, saturation, brightness);

        colors[vertexIndex + 0] = col;
        colors[vertexIndex + 1] = col;
        colors[vertexIndex + 2] = col;
        colors[vertexIndex + 3] = col;
    }
}