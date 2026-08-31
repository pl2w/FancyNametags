using TMPro;
using UnityEngine;

namespace FancyNametags.Effects;

public class TextPulse : BaseNameEffect
{
    [EffectConfig("Speed of the pulse animation")]
    public float speed = 4f;

    [EffectConfig("Maximum scale increase/decrease of the pulse")]
    public float amount = 0.3f;

    [EffectConfig("How tight the pulse wave is across the characters")]
    public float waveFrequency = 0.3f;

    protected internal override bool ModifyVertices => true;
    protected internal override bool ModifyColors => false;

    protected internal override void AnimateCharacter(
        int charIndex,
        int vertexIndex,
        TMP_CharacterInfo charInfo,
        Vector3[] vertices,
        Color32[] colors)
    {
        var pulse = Mathf.Sin(Time.time * speed - charIndex * waveFrequency);
        var scale = 1f + pulse * amount;

        var centroid = (
            vertices[vertexIndex + 0] +
            vertices[vertexIndex + 1] +
            vertices[vertexIndex + 2] +
            vertices[vertexIndex + 3]
        ) / 4f;

        for (var v = 0; v < 4; v++)
        {
            var localOffset = vertices[vertexIndex + v] - centroid;
            vertices[vertexIndex + v] = centroid + localOffset * scale;
        }
    }
}