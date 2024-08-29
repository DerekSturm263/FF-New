using Quantum;
using UnityEngine;

public unsafe partial class SpawnHitboxEventAsset
{
    public VFXSettings VFXOnHit;
    public ClipSettings AudioOnHit;

    public void InitForBaking()
    {
        Settings.BakedShapePositions = new ListHelper<Photon.Deterministic.FPVector2>[Settings.Shape.CompoundShapes.Length];

        for (int i = 0; i < Settings.Shape.CompoundShapes.Length; ++i)
        {
            Settings.BakedShapePositions[i].List = new();
        }
    }

    public void Bake(int frameIndex, GameObject player, GameObject mainWeapon)
    {
        for (int i = 0; i < Settings.BakedShapePositions.Length; ++i)
        {
            Vector3 position = Settings.Parent == SpawnHitboxEvent.ParentType.Player ? player .transform.position : mainWeapon.transform.position;

            if (Settings.Parent == SpawnHitboxEvent.ParentType.Weapon)
                position += mainWeapon.transform.up * Settings.Shape.CompoundShapes[i].PositionOffset.Y.AsFloat + mainWeapon.transform.right * Settings.Shape.CompoundShapes[i].PositionOffset.X.AsFloat;
            else
                position += new Vector3(Settings.Shape.CompoundShapes[i].PositionOffset.X.AsFloat, Settings.Shape.CompoundShapes[i].PositionOffset.Y.AsFloat);

            Settings.BakedShapePositions[i].List.Add(position.ToFPVector2());
        }

        Debug.Log($"Frame {frameIndex} baked successfully.");
    }
}
