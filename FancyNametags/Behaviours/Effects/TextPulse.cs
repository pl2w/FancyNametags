using TMPro;
using UnityEngine;

namespace FancyNametags.Effects;

public class TextPulse : BaseNameEffect
{
    public float speed = 4f;
    public float amount = 0.3f;
    public float waveFrequency = 0.3f;

    protected override bool ModifyVertices => true;
    protected override bool ModifyColors => false;

    protected override void AnimateCharacter(
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