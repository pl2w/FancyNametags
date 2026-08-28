using TMPro;
using UnityEngine;

namespace FancyNametags.Effects;

public abstract class BaseNameEffect : MonoBehaviour
{
    protected TMP_Text NameTag;
    protected VRRig Rig;

    protected internal abstract bool ModifyVertices { get; }
    protected internal abstract bool ModifyColors { get; }

    public virtual void Initialize(TMP_Text nametag, VRRig rig, object data = null)
    {
        NameTag = nametag;
        Rig = rig;
    }

    protected internal abstract void AnimateCharacter(
        int charIndex,
        int vertexIndex,
        TMP_CharacterInfo charInfo,
        Vector3[] vertices,
        Color32[] colors
    );

    protected internal virtual bool ShouldAnimateThisFrame() => true;
}
