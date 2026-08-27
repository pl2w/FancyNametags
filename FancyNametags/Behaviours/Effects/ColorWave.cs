using TMPro;
using UnityEngine;

namespace FancyNametags.Effects;

public class ColorWave : BaseNameEffect
{
    public float speed = 0.5f;
    public float highlightIntensity = 0.5f;

    protected internal override bool ModifyVertices => false;
    protected internal override bool ModifyColors => true;

    protected internal override void AnimateCharacter(
        int charIndex,
        int vertexIndex,
        TMP_CharacterInfo charInfo,
        Vector3[] vertices,
        Color32[] colors)
    {
        if (!Rig) return;

        var baseColor = Rig.playerColor;
        var colorMid = Color.Lerp(baseColor, Color.white, highlightIntensity);
        var colorDark = baseColor * 0.75f;
        colorDark.a = baseColor.a;

        var totalChars = NameTag.textInfo.characterCount;
        var charOffset = (float)charIndex / Mathf.Max(1, totalChars - 1);

        var wave = Mathf.Sin((Time.time * speed - charOffset) * Mathf.PI * 2f) * 0.5f + 0.5f;
        Color32 col = Color.Lerp(colorDark, colorMid, wave);

        colors[vertexIndex + 0] = col;
        colors[vertexIndex + 1] = col;
        colors[vertexIndex + 2] = col;
        colors[vertexIndex + 3] = col;
    }
}