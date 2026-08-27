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

    protected internal virtual bool ShouldAnimateThisFrame() => true;

    internal bool ModifyVerticesInternal => ModifyVertices;
    internal bool ModifyColorsInternal => ModifyColors;

    internal void AnimateCharacterInternal(
        int charIndex,
        int vertexIndex,
        TMP_CharacterInfo charInfo,
        Vector3[] vertices,
        Color32[] colors)
        => AnimateCharacter(charIndex, vertexIndex, charInfo, vertices, colors);
}