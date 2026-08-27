using FancyNametags.Effects;
using TMPro;
using UnityEngine;

namespace FancyNametags.Behaviours;

public class NameEffectController : MonoBehaviour
{
    private TMP_Text _nameTag;
    private VRRig _rig;

    private BaseNameEffect _vertexEffect;
    private BaseNameEffect _colorEffect;

    public void Initialize(TMP_Text nameTag, VRRig rig)
    {
        ClearAllEffects();
        
        _nameTag = nameTag;
        _rig = rig;
    }

    public void SetVertexEffect(BaseNameEffect effect)
    {
        if (effect != null && !effect.ModifyVerticesInternal)
        {
            Plugin.Log.LogWarning($"{effect.GetType().Name} does not modify vertices, and cannot be used as a vertex effect.");
            return;
        }

        if (_vertexEffect != null && _vertexEffect != effect && _vertexEffect != _colorEffect)
            Destroy(_vertexEffect);

        _vertexEffect = effect;
        effect?.Initialize(_nameTag, _rig);
    }

    public void SetColorEffect(BaseNameEffect effect)
    {
        if (effect != null && !effect.ModifyColorsInternal)
        {
            Plugin.Log.LogWarning($"{effect.GetType().Name} does not modify colors, and cannot be used as a color effect.");
            return;
        }

        if (_colorEffect != null && _colorEffect != effect && _colorEffect != _vertexEffect)
            Destroy(_colorEffect);

        _colorEffect = effect;
        effect?.Initialize(_nameTag, _rig);
    }

    public void ClearVertexEffect()
    {
        if (_vertexEffect != null && _vertexEffect != _colorEffect)
            Destroy(_vertexEffect);

        _vertexEffect = null;
        ResetMesh();
    }

    public void ClearColorEffect()
    {
        if (_colorEffect != null && _colorEffect != _vertexEffect)
            Destroy(_colorEffect);

        _colorEffect = null;
        ResetMesh();
    }

    public void ClearAllEffects()
    {
        if (_vertexEffect != null) Destroy(_vertexEffect);
        if (_colorEffect != null && _colorEffect != _vertexEffect) Destroy(_colorEffect);

        _vertexEffect = null;
        _colorEffect = null;
        ResetMesh();
    }

    private void LateUpdate()
    {
        if (!_nameTag) return;
        if (!_vertexEffect && !_colorEffect) return;

        var sameEffect = _vertexEffect && _vertexEffect == _colorEffect;

        var runVertex = _vertexEffect && _vertexEffect.ShouldAnimateThisFrame();
        var runColor = sameEffect ? runVertex : (_colorEffect && _colorEffect.ShouldAnimateThisFrame());

        if (!runVertex && !runColor) return;

        _nameTag.ForceMeshUpdate();

        var textInfo = _nameTag.textInfo;
        if (textInfo.characterCount == 0) return;

        for (var i = 0; i < textInfo.characterCount; i++)
        {
            var charInfo = textInfo.characterInfo[i];
            if (!charInfo.isVisible) continue;

            var vertIdx = charInfo.vertexIndex;
            var matIdx = charInfo.materialReferenceIndex;
            var verts = textInfo.meshInfo[matIdx].vertices;
            var colors = textInfo.meshInfo[matIdx].colors32;

            if (runVertex)
                _vertexEffect.AnimateCharacterInternal(i, vertIdx, charInfo, verts, colors);

            if (runColor && !sameEffect)
                _colorEffect.AnimateCharacterInternal(i, vertIdx, charInfo, verts, colors);
        }

        var flags = TMP_VertexDataUpdateFlags.None;
        if (runVertex) flags |= TMP_VertexDataUpdateFlags.Vertices;
        if (runColor) flags |= TMP_VertexDataUpdateFlags.Colors32;
        _nameTag.UpdateVertexData(flags);
    }

    private void ResetMesh()
    {
        if (!_nameTag) return;
        _nameTag.ForceMeshUpdate();
        _nameTag.UpdateVertexData(TMP_VertexDataUpdateFlags.Vertices | TMP_VertexDataUpdateFlags.Colors32);
    }

    private void OnDisable() => ClearAllEffects();
    private void OnDestroy() => ClearAllEffects();
}