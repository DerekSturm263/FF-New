using Quantum.Collections;
using System.Diagnostics;

namespace Quantum
{
    public unsafe class HitboxSystem : SystemMainThreadFilter<HitboxSystem.Filter>, ISignalOnComponentRemoved<HitboxInstance>
    {
        public struct Filter
        {
            public EntityRef Entity;

            public Transform3D* Transform;
            public HitboxInstance* HitboxInstance;
        }

        public override void Update(Frame f, ref Filter filter)
        {
            --filter.HitboxInstance->Lifetime;
            if (filter.HitboxInstance->Lifetime <= 0)
            {
                f.Destroy(filter.Entity);
            }

            DrawHitbox(filter.Transform, filter.HitboxInstance);
        }

        public void OnRemoved(Frame f, EntityRef entity, HitboxInstance* component)
        {
            if (f.Unsafe.TryGetPointer(component->Owner, out Stats* stats))
            {
                QList<EntityRef> hitboxes = f.ResolveList(stats->Hitboxes);
                hitboxes.Remove(entity);
            }

            component->Shape.Compound.FreePersistent(f);
            f.Events.OnHitboxSpawnDespawn(component->Owner, entity, false);
        }

        [Conditional("DEBUG")]
        public static void DrawHitbox(Transform3D* transform, HitboxInstance* hitboxInstance)
        {
            ColorRGBA color = new() { R = 255, G = 255, B = 255, A = 255 };

            switch (hitboxInstance->Shape.Type)
            {
                case Shape3DType.Sphere:
                    Draw.Sphere(transform->Position + hitboxInstance->Shape.Centroid, hitboxInstance->Shape.Sphere.Radius, color);
                    break;

                case Shape3DType.Box:
                    Draw.Box(transform->Position + hitboxInstance->Shape.Centroid, hitboxInstance->Shape.Box.Extents * 2, transform->Rotation, color);
                    break;
            }
        }

        public static void SpawnHitbox(Frame f, HitboxSettings settings, Shape2DConfig shape, int lifetime, EntityRef user, EntityRef parent = default)
        {
            Log.Debug("Spawning hitbox!");

            EntityPrototype hitboxPrototype = f.FindAsset<EntityPrototype>(f.RuntimeConfig.Hitbox.Id);

            Shape3DConfig shape3D = new()
            {
                CompoundShapes = new Shape3DConfig.CompoundShapeData3D[shape.CompoundShapes.Length]
            };

            for (int i = 0; i < shape3D.CompoundShapes.Length; ++i)
            {
                shape3D.CompoundShapes[i] = new()
                {
                    BoxExtents = shape.CompoundShapes[i].BoxExtents.XYO,
                    SphereRadius = shape.CompoundShapes[i].CircleRadius,
                    PositionOffset = shape.CompoundShapes[i].PositionOffset.OXY,
                    ShapeType = shape.CompoundShapes[i].ShapeType == Shape2DType.Circle ? Shape3DType.Sphere : Shape3DType.Box
                };
            }

            if (f.Unsafe.TryGetPointer(user, out Stats* stats))
            {
                for (int i = 0; i < shape3D.CompoundShapes.Length; ++i)
                {
                    EntityRef hitboxEntity = parent == EntityRef.None ? f.Create(hitboxPrototype) : f.CreateChilded(hitboxPrototype, parent);

                    if (f.Unsafe.TryGetPointer(hitboxEntity, out HitboxInstance* hitboxInstance))
                    {
                        hitboxInstance->Shape = shape3D.CompoundShapes[i].CreateShape(f);
                        hitboxInstance->Settings = settings;
                        hitboxInstance->Lifetime = lifetime;
                        hitboxInstance->Owner = user;

                        QList<EntityRef> hitboxLists = f.ResolveList(stats->Hitboxes);
                        hitboxLists.Add(hitboxEntity);

                        f.Events.OnHitboxSpawnDespawn(user, hitboxEntity, true);
                    }
                }
            }
        }
    }
}