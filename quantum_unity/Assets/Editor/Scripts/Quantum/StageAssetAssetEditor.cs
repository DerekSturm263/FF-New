using Photon.Deterministic;
using Quantum;
using Quantum.Editor;
using Quantum.Types;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(StageAssetAsset))]
public class StageAssetAssetEditor : Editor
{
    public override unsafe void OnInspectorGUI()
    {
        StageAssetAsset stage = target as StageAssetAsset;

        if (GUILayout.Button("Bake Colliders"))
        {
            GameObject stageObj = UnityDB.FindAssetForInspector(stage.Stage.value.Objects.Stage.Id).FindNestedObjectParent() as GameObject;

            fixed (ArrayStaticColliderInfo* colliders = &stage.Stage.value.Objects.Colliders)
            {
                BakeColliders(colliders, stageObj);
            }
        }

        base.OnInspectorGUI();
    }

    public static unsafe void BakeColliders(ArrayStaticColliderInfo* colliders, GameObject scene)
    {
        Debug.Assert(scene);
        Debug.Log("Baking Colliders...");

        QuantumStaticBoxCollider2D[] staticCollider2Ds = scene.GetComponentsInChildren<QuantumStaticBoxCollider2D>();

        BakeAllColliders(colliders, staticCollider2Ds);
        Debug.Log($"{staticCollider2Ds.Length} Colliders Baked");
    }

    private static unsafe void BakeAllColliders(ArrayStaticColliderInfo* colliders, QuantumStaticBoxCollider2D[] staticCollider2Ds)
    {
        for (int i = 0; i < staticCollider2Ds.Length; ++i)
        {
            ArrayHelper.Set(colliders, i, BakeCollider(staticCollider2Ds[i]));
        }

        for (int i = staticCollider2Ds.Length; i < 8; ++i)
        {
            ArrayHelper.Set(colliders, i, default);
        }
    }

    private static StaticColliderInfo BakeCollider(QuantumStaticBoxCollider2D collider)
    {
        collider.BeforeBake();

        var e = collider.Size.ToUnityVector3();
        var s = collider.transform.localScale;

        e.x *= s.x;
        e.y *= s.y;
        e.z *= s.z;

        Debug.Log($"Collider Baked: {collider.name}");

        return new()
        {
            Position = collider.transform.TransformPoint(collider.PositionOffset.ToUnityVector3()).ToFPVector2(),
            Rotation = collider.transform.rotation.ToFPRotation2D() + collider.RotationOffset.FlipRotation() * FP.Deg2Rad,
#if QUANTUM_XY
            VerticalOffset = -collider.transform.position.z.ToFP(),
            Height = collider.Height * s.z.ToFP(),
#else
        VerticalOffset = collider.transform.position.y.ToFP(),
        Height         = collider.Height * s.y.ToFP(),
#endif
            PhysicsMaterial = collider.Settings.PhysicsMaterial,
            Layer = collider.gameObject.layer,

            // polygon
            BoxExtents = e.ToFPVector2() * FP._0_50
        };
    }
}
