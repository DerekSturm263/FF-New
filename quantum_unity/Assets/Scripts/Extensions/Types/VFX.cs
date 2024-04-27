using UnityEngine;
using Photon.Deterministic;
using Quantum;

[CreateAssetMenu(menuName = "Quantum/ClientAsset/VFX")]
public class VFX : ScriptableObject
{
    public enum ParentType
    {
        None,
        User,
        Target,
        Weapon,
        SubWeapon
    }

    public GameObject VFXObject;
    public ParentType Parent;

    public FPVector3 Offset;
    public FPVector3 Direction;
    public FPVector2 ScaleMultiplier;

    public bool DoesFollowParent;
}
