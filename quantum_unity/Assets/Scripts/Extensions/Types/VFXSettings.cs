using UnityEngine;

[System.Serializable]
public struct VFXSettings
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
    public bool DoesFollowParent;

    public Vector3 Offset;
    public Vector3 Direction;
    public Vector3 ScaleMultiplier;

    public static VFXSettings Lerp(VFXSettings a, VFXSettings b, float t)
    {
        return new()
        {
            VFXObject = a.VFXObject,
            Parent = a.Parent,
            DoesFollowParent = a.DoesFollowParent,
            Offset = Vector3.Lerp(a.Offset, b.Offset, t),
            Direction = Vector3.Lerp(a.Direction, b.Direction, t),
            ScaleMultiplier = Vector3.Lerp(a.ScaleMultiplier, b.ScaleMultiplier, t)
        };
    }
}
