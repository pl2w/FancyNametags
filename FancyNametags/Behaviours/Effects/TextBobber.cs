using TMPro;
using UnityEngine;

namespace FancyNametags.Effects;

public class TextBobber : BaseNameEffect
{
    public float speed = 3f;
    public float bounceHeight = 4f;
    public float waveFrequency = 0.4f;

    protected override bool ModifyVertices => true;
    protected override bool ModifyColors => false;

    protected override void AnimateCharacter(
        int charIndex,
        int vertexIndex,
        TMP_CharacterInfo charInfo,
        Vector3[] vertices,
        Color32[] colors)
    {
        var yOffset = Mathf.Sin(Time.time * speed - (charIndex * waveFrequency)) * bounceHeight;
        var offset = new Vector3(0, yOffset, 0);

        vertices[vertexIndex + 0] += offset;
        vertices[vertexIndex + 1] += offset;
        vertices[vertexIndex + 2] += offset;
        vertices[vertexIndex + 3] += offset;
    }
}