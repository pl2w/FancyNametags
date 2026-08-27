using TMPro;
using UnityEngine;

namespace FancyNametags.Effects;

public class TextRainbow : BaseNameEffect
{
    public float speed = 0.3f;
    public float saturation = 1f;
    public float brightness = 1f;
    public float hueSpread = 1f;

    protected override bool ModifyVertices => false;
    protected override bool ModifyColors => true;

    protected override void AnimateCharacter(
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