using FancyNametags.Effects;
using TMPro;
using UnityEngine;

namespace FancyNametags.Behaviours;

public class NameEffectController : MonoBehaviour
{
    private TMP_Text _nameTag;
    private VRRig _rig;

    public BaseNameEffect VertexEffect;
    public BaseNameEffect ColorEffect;

    public void Initialize(TMP_Text nameTag, VRRig rig)
    {
        ClearAllEffects();

        _nameTag = nameTag;
        _rig = rig;
    }

    public void SetVertexEffect(BaseNameEffect effect, object effectData)
    {
        if (effect != null && !effect.ModifyVertices)
        {
            string message = $"{effect.GetType().Name} does not modify vertices, and cannot be used as a vertex effect.";
            Views.SelectView.Instance.ActiveError = message;
            Plugin.Log.LogWarning(message);
            return;
        }

        if (VertexEffect != null && VertexEffect != effect && VertexEffect != ColorEffect)
            Destroy(VertexEffect);

        VertexEffect = effect;
        effect?.Initialize(_nameTag, _rig, effectData);
    }

    public void SetColorEffect(BaseNameEffect effect, object effectData)
    {
        if (effect != null && !effect.ModifyColors)
        {
            string message = $"{effect.GetType().Name} does not modify colors, and cannot be used as a color effect.";
            Views.SelectView.Instance.ActiveError = message;
            Plugin.Log.LogWarning(message);
            return;
        }

        if (ColorEffect != null && ColorEffect != effect && ColorEffect != VertexEffect)
            Destroy(ColorEffect);

        ColorEffect = effect;
        effect?.Initialize(_nameTag, _rig, effectData);
    }

    public void ClearVertexEffect()
    {
        if (VertexEffect != null && VertexEffect != ColorEffect)
            Destroy(VertexEffect);

        VertexEffect = null;
        ResetMesh();
    }

    public void ClearColorEffect()
    {
        if (ColorEffect != null && ColorEffect != VertexEffect)
            Destroy(ColorEffect);

        ColorEffect = null;
        ResetMesh();
    }

    public void ClearAllEffects()
    {
        if (VertexEffect != null) Destroy(VertexEffect);
        if (ColorEffect != null && ColorEffect != VertexEffect) Destroy(ColorEffect);

        VertexEffect = null;
        ColorEffect = null;
        ResetMesh();
    }

    private void LateUpdate()
    {
        if (!_nameTag) return;
        if (!VertexEffect && !ColorEffect) return;

        var sameEffect = VertexEffect && VertexEffect == ColorEffect;

        var runVertex = VertexEffect && VertexEffect.ShouldAnimateThisFrame();
        var runColor = sameEffect ? runVertex : (ColorEffect && ColorEffect.ShouldAnimateThisFrame());

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
                VertexEffect.AnimateCharacter(i, vertIdx, charInfo, verts, colors);

            if (runColor && !sameEffect)
                ColorEffect.AnimateCharacter(i, vertIdx, charInfo, verts, colors);

            if (runColor && !sameEffect)
                ColorEffect.AnimateCharacter(i, vertIdx, charInfo, verts, colors);
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
