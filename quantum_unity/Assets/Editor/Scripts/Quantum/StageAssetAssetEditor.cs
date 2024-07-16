using Photon.Deterministic;
using Quantum;
using Quantum.Editor;
using Quantum.Types;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(StageAssetAsset))]
public class StageAssetAssetEditor : Editor
{
    public override void OnInspectorGUI()
    {
        StageAssetAsset stage = target as StageAssetAsset;

        if (GUILayout.Button("Bake Colliders"))
        {
            GameObject stageObj = UnityDB.FindAssetForInspector(stage.Stage.Value.Objects.Stage.Id).FindNestedObjectParent() as GameObject;
            BakeColliders(ref stage.Stage.Value.Objects.Colliders, stageObj);
        }

        base.OnInspectorGUI();
    }

    public static void BakeColliders(ref ArrayStaticColliderInfo colliders, GameObject scene)
    {
        Debug.Assert(scene);

        QuantumStaticBoxCollider2D[] staticCollider2Ds = scene.GetComponentsInChildren<QuantumStaticBoxCollider2D>();

        BakeAllColliders(ref colliders, staticCollider2Ds);
    }

    private static unsafe void BakeAllColliders(ref ArrayStaticColliderInfo colliders, QuantumStaticBoxCollider2D[] staticCollider2Ds)
    {
        for (int i = 0; i < staticCollider2Ds.Length; ++i)
        {
            StaticColliderInfo* col = ArrayHelper.GetPointer(colliders, i);
            *col = BakeCollider(staticCollider2Ds[i]);
        }

        for (int i = staticCollider2Ds.Length; i < 8; ++i)
        {
            StaticColliderInfo* col = ArrayHelper.GetPointer(colliders, i);
            *col = default;
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
