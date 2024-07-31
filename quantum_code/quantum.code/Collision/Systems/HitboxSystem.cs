using Photon.Deterministic;
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

            DrawHitbox(f, ref filter);
        }

        [Conditional("DEBUG")]
        private void DrawHitbox(Frame f, ref Filter filter)
        {
            if (filter.HitboxInstance->Shape.Compound.GetShapes(f, out Shape2D* shapesBuffer, out int count))
            {
                for (int i = 0; i < count; ++i)
                {
                    Shape2D* currentShape = shapesBuffer + i;
                    FPVector3 position = filter.Transform->Position + currentShape->LocalTransform.Position.XYO;

                    switch (currentShape->Type)
                    {
                        case Shape2DType.Circle:
                            Draw.Circle(position, currentShape->Circle.Radius);
                            break;

                        case Shape2DType.Box:
                            Draw.Box(position, currentShape->Box.Extents.XYO);
                            break;
                    }
                }
            }
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

        public static EntityRef SpawnHitbox(Frame f, HitboxSettings settings, Shape2DConfig shape, int lifetime, EntityRef user, EntityRef parent = default)
        {
            Log.Debug("Spawning hitbox!");

            EntityPrototype hitboxPrototype = f.FindAsset<EntityPrototype>(f.RuntimeConfig.Hitbox.Id);
            EntityRef hitboxEntity = parent == EntityRef.None ? f.Create(hitboxPrototype) : f.CreateChilded(hitboxPrototype, parent);

            if (f.Unsafe.TryGetPointer(user, out Stats* stats))
            {
                if (f.Unsafe.TryGetPointer(hitboxEntity, out HitboxInstance* hitboxInstance))
                {
                    hitboxInstance->Shape = Shape2D.CreatePersistentCompound();

                    for (int i = 0; i < shape.CompoundShapes.Length; ++i)
                    {
                        Shape2D createdShape = shape.CompoundShapes[i].CreateShape(f);
                        hitboxInstance->Shape.Compound.AddShape(f, ref createdShape);
                    }

                    hitboxInstance->Settings = settings;
                    hitboxInstance->Lifetime = lifetime;
                    hitboxInstance->Owner = user;
                }

                QList<EntityRef> hitboxLists = f.ResolveList(stats->Hitboxes);
                hitboxLists.Add(hitboxEntity);
            }

            f.Events.OnHitboxSpawnDespawn(user, hitboxEntity, true);

            return hitboxEntity;
        }
    }
}