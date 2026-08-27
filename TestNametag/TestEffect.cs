using TMPro;
using UnityEngine;

namespace FancyNametags.Effects;

public class TestEffect : BaseNameEffect
{
    protected override bool ModifyVertices => true;
    protected override bool ModifyColors => true;

    private const float WaveSpeed = 2f;
    private const float WaveHeight = 5f;
    private const float ColorPulseSpeed = 3f;

    protected override void AnimateCharacter(
        int charIndex,
        int vertexIndex,
        TMP_CharacterInfo charInfo,
        Vector3[] vertices,
        Color32[] colors)
    {
        var offset = Mathf.Sin(Time.time * WaveSpeed + charIndex * 0.5f) * WaveHeight;

        vertices[vertexIndex + 0] += new Vector3(0, offset, 0);
        vertices[vertexIndex + 1] += new Vector3(0, offset, 0);
        vertices[vertexIndex + 2] += new Vector3(0, offset, 0);
        vertices[vertexIndex + 3] += new Vector3(0, offset, 0);

        var t = (Mathf.Sin(Time.time * ColorPulseSpeed + charIndex * 0.3f) + 1f) * 0.5f;
        var pulseColor = Color32.Lerp(Color.white, Color.cyan, t);

        colors[vertexIndex + 0] = pulseColor;
        colors[vertexIndex + 1] = pulseColor;
        colors[vertexIndex + 2] = pulseColor;
        colors[vertexIndex + 3] = pulseColor;
    }
}