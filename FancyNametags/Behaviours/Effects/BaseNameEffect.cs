using TMPro;
using UnityEngine;

namespace FancyNametags.Effects;

public abstract class BaseNameEffect : MonoBehaviour
{
    protected TMP_Text NameTag;
    protected VRRig Rig;

    protected abstract bool ModifyVertices { get; }
    protected abstract bool ModifyColors { get; }

    public virtual void Initialize(TMP_Text nametag, VRRig rig)
    {
        NameTag = nametag;
        Rig = rig;
    }

    protected abstract void AnimateCharacter(
        int charIndex, 
        int vertexIndex, 
        TMP_CharacterInfo charInfo, 
        Vector3[] vertices, 
        Color32[] colors
    );
    
    protected virtual void Update()
    {
        if (!NameTag) 
            return;

        if (ModifyVertices)
            NameTag.ForceMeshUpdate();

        var textInfo = NameTag.textInfo;
        if (textInfo.characterCount == 0) 
            return;

        for (var i = 0; i < textInfo.characterCount; i++)
        {
            var charInfo = textInfo.characterInfo[i];
            if (!charInfo.isVisible) continue;

            var vertIdx = charInfo.vertexIndex;
            var matIdx = charInfo.materialReferenceIndex;

            var verts = textInfo.meshInfo[matIdx].vertices;
            var colors = textInfo.meshInfo[matIdx].colors32;

            AnimateCharacter(i, vertIdx, charInfo, verts, colors);
        }

        if (ModifyVertices && ModifyColors)
            NameTag.UpdateVertexData(TMP_VertexDataUpdateFlags.Vertices | TMP_VertexDataUpdateFlags.Colors32);
        else if (ModifyVertices)
            NameTag.UpdateVertexData(TMP_VertexDataUpdateFlags.Vertices);
        else if (ModifyColors)
            NameTag.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
    }
}