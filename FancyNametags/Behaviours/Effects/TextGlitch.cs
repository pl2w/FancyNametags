using TMPro;
using UnityEngine;

namespace FancyNametags.Effects;

public class TextGlitch : BaseNameEffect
{
    public float glitchChance = 0.12f;
    public float horizontalOffset = 7f;
    public float verticalOffset = 1.5f;
    public float glitchInterval = 0.05f;

    private float _nextGlitchTime;

    protected override bool ModifyVertices => true;
    protected override bool ModifyColors => true;

    protected override void Update()
    {
        if (Time.time < _nextGlitchTime) return;
        _nextGlitchTime = Time.time + glitchInterval;

        base.Update();
    }

    protected override void AnimateCharacter(
        int charIndex, 
        int vertexIndex, 
        TMP_CharacterInfo charInfo, 
        Vector3[] vertices, 
        Color32[] colors)
    {
        if (Random.value >= glitchChance) return;

        var jitter = new Vector3(
            Random.Range(-horizontalOffset, horizontalOffset),
            Random.Range(-verticalOffset, verticalOffset),
            0
        );

        vertices[vertexIndex + 0] += jitter;
        vertices[vertexIndex + 1] += jitter;
        vertices[vertexIndex + 2] += jitter;
        vertices[vertexIndex + 3] += jitter;

        Color32 glitchCol = Random.value > 0.5f ? Color.cyan : Color.magenta;
        colors[vertexIndex + 0] = glitchCol;
        colors[vertexIndex + 1] = glitchCol;
        colors[vertexIndex + 2] = glitchCol;
        colors[vertexIndex + 3] = glitchCol;
    }
}