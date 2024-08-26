using Extensions.Components.Miscellaneous;
using Quantum;
using System;
using UnityEngine;
using UnityEngine.Serialization;

public class DrawHitboxHurtbox : Controller<DrawHitboxHurtbox>
{
    [SerializeField] private Mesh _circle, _box;
    [SerializeField][FormerlySerializedAs("_hitbox")] private Material _material;

    private bool _drawHitboxes;
    public void SetDrawHitboxes(bool drawHitboxes) => _drawHitboxes = drawHitboxes;

    private bool _drawHurtboxes;
    public void SetDrawHurtboxes(bool drawHurtboxes) => _drawHurtboxes = drawHurtboxes;

    private bool _drawKnockback;
    public void SetDrawKnockback(bool drawKnockback) => _drawKnockback = drawKnockback;

    private LineRenderer[] _lineRenderers;

    private void Awake()
    {
        _lineRenderers = GetComponentsInChildren<LineRenderer>();
    }

    private unsafe void LateUpdate()
    {
        if (!QuantumRunner.Default || !QuantumRunner.Default.IsRunning)
            return;

        if (_drawHitboxes)
            DrawAllHitboxes();

        if (_drawHurtboxes)
            DrawAllHurtboxes();

        foreach (var item in _lineRenderers)
        {
            item.gameObject.SetActive(_drawKnockback);   
        }

        if (_drawKnockback)
            DrawAllKnockback();
    }

    private unsafe void DrawAllHitboxes()
    {
        var filter = QuantumRunner.Default.Game.Frames.Verified.Unsafe.FilterStruct<HitboxSystem.Filter>();
        var item = default(HitboxSystem.Filter);

        MaterialPropertyBlock properties = new();
        properties.SetColor("_BaseColor", new(1, 0, 0, 0.35f));
        
        Matrix4x4 transform = Matrix4x4.identity;

        while (filter.Next(&item))
        {
            transform = Matrix4x4.TRS(item.Transform->Position.ToUnityVector3(), item.Transform->Rotation.ToUnityQuaternion(), 2 * item.HitboxInstance->Shape.BroadRadius.AsFloat * Vector3.one);
            
            Graphics.DrawMesh(_circle, transform, _material, 0, Camera.main, 0, properties);
        }
    }

    private unsafe void DrawAllHurtboxes()
    {
        var filter = QuantumRunner.Default.Game.Frames.Verified.Unsafe.FilterStruct<HurtboxSystem.Filter>();
        var item = default(HurtboxSystem.Filter);

        MaterialPropertyBlock properties = new();
        Matrix4x4 transform = Matrix4x4.identity;
        Mesh mesh = null;

        while (filter.Next(&item))
        {
            if (item.PhysicsCollider->Shape.Type == Shape2DType.Circle)
            {
                Vector3 scale = item.PhysicsCollider->Shape.Circle.Radius.AsFloat * Vector3.one;
                transform = Matrix4x4.TRS(item.Transform->Position.ToUnityVector3(), item.Transform->Rotation.ToUnityQuaternion(), 4 * scale);

                mesh = _circle;
            }
            else if (item.PhysicsCollider->Shape.Type == Shape2DType.Box)
            {
                Vector3 scale = item.PhysicsCollider->Shape.Box.Extents.ToUnityVector3();
                scale.z = 1;
                transform = Matrix4x4.TRS(item.Transform->Position.ToUnityVector3() + scale / 2, item.Transform->Rotation.ToUnityQuaternion(), 2 * scale);

                mesh = _box;
            }

            Color color = new()
            {
                r = item.HurtboxInstance->Settings.CanBeDamaged ? 1 : 0,
                g = item.HurtboxInstance->Settings.CanBeKnockedBack ? 1 : 0,
                b = item.HurtboxInstance->Settings.CanBeInterrupted ? 1 : 0,
                a = 0.35f
            };
            properties.SetColor("_BaseColor", color);

            Graphics.DrawMesh(mesh, transform, _material, 0, Camera.main, 0, properties);
        }
    }

    private unsafe void DrawAllKnockback()
    {
        var filter = QuantumRunner.Default.Game.Frames.Verified.Unsafe.FilterStruct<CharacterControllerSystem.Filter>();
        var item = default(CharacterControllerSystem.Filter);

        MaterialPropertyBlock properties = new();
        properties.SetColor("_BaseColor", new(1, 0, 0, 0.35f));

        int i = 0;
        while (filter.Next(&item))
        {
            var positions = CalculateArcPositions(20, item.CharacterController->OldKnockback.Direction.ToUnityVector2(), item.CharacterController->OriginalPosition.ToUnityVector2());
            _lineRenderers[i].positionCount = 20;
            _lineRenderers[i].SetPositions(positions);

            ++i;
        }
    }

    private static Vector3[] CalculateArcPositions(int resolution, Vector2 amount, Vector2 offset)
    {
        Vector3[] positions = new Vector3[resolution];

        for (int i = 0; i < resolution; ++i)
        {
            float t = (float)i / resolution;
            positions[i] = CalculateArcPoint(t, 20, 1, amount) + offset;
        }

        return positions;
    }

    private static Vector2 CalculateArcPoint(float t, float gravity, float scalar, Vector2 amount)
    {
        amount.x += 0.0001f;
        float angle = Mathf.Atan2(amount.y, amount.x);

        float x = t * amount.x;
        float y = x * Mathf.Tan(angle) - (gravity * x * x / (2 * amount.magnitude * amount.magnitude * Mathf.Cos(angle) * Mathf.Cos(angle)));

        return new Vector2(x, y) * scalar;
    }
}
