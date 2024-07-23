using Photon.Deterministic;
using Quantum.Collections;
using System.Diagnostics;

namespace Quantum
{
    public unsafe class HitboxSystem : SystemMainThreadFilter<HitboxSystem.Filter>
    {
        public struct Filter
        {
            public EntityRef Entity;

            public Transform2D* Transform;
            public HitboxInstance* HitboxInstance;
        }

        public override void Update(Frame f, ref Filter filter)
        {
            DrawHitbox(f, filter.Transform, filter.HitboxInstance, filter.HitboxInstance->Shape);

            --filter.HitboxInstance->Lifetime;
            if (filter.HitboxInstance->Lifetime <= 0)
            {
                KillHitbox(f, filter.HitboxInstance->Owner, filter.Entity);
            }

            if (f.Unsafe.TryGetPointer(filter.HitboxInstance->Owner, out Transform2D* transform))
            {
                filter.Transform->Position = transform->Position;
            }
        }

        [Conditional("DEBUG")]
        private void DrawHitbox(Frame f, Transform2D* transform, HitboxInstance* hitboxInstance, Shape2D shape)
        {
            if (shape.Compound.GetShapes(f, out Shape2D* shapesBuffer, out int count))
            {
                for (int i = 0; i < count; ++i)
                {
                    Shape2D* currentShape = shapesBuffer + i;

                    switch (currentShape->Type)
                    {
                        case Shape2DType.Circle:
                            Draw.Circle(transform->Position + currentShape->LocalTransform.Position, currentShape->Circle.Radius);
                            break;

                        case Shape2DType.Box:
                            Draw.Box((transform->Position + currentShape->LocalTransform.Position).XYO, currentShape->Box.Extents.XYO);
                            break;
                    }
                }
            }
        }

        private void KillHitbox(Frame f, EntityRef owner, EntityRef hitbox)
        {
            if (f.Unsafe.TryGetPointer(owner, out Stats* stats))
            {
                QList<EntityRef> hitboxes = f.ResolveList(stats->Hitboxes);
                hitboxes.Remove(hitbox);
            }

            if (f.Unsafe.TryGetPointer(hitbox, out HitboxInstance* instance))
            {
                instance->Shape.Compound.FreePersistent(f);
            }

            f.Destroy(hitbox);
        }

        public static EntityRef SpawnHitbox(Frame f, HitboxSettings settings, Shape2DConfig shape, int lifetime, EntityRef user)
        {
            Log.Debug("Spawning hitbox!");

            EntityPrototype hitboxPrototype = f.FindAsset<EntityPrototype>(f.RuntimeConfig.Hitbox.Id);
            EntityRef hitboxEntity = f.Create(hitboxPrototype);

            f.Events.OnCameraShake(settings.SpawnShake, new FPVector2(f.Global->RngSession.Next(-FP._1, FP._1), f.Global->RngSession.Next(-FP._1, FP._1)));

            if (f.Unsafe.TryGetPointer(user, out Stats* stats))
            {
                if (f.Unsafe.TryGetPointer(hitboxEntity, out HitboxInstance* hitbox))
                {
                    if (settings.Parent == ParentType.MainWeapon)
                    {
                        settings.Damage *= stats->WeaponStatsMultiplier.Damage;
                        settings.Knockback *= stats->WeaponStatsMultiplier.Knockback;
                    }

                    hitbox->Shape = Shape2D.CreatePersistentCompound();
                    
                    for (int i = 0; i < shape.CompoundShapes.Length; ++i)
                    {
                        Shape2D createdShape = shape.CompoundShapes[i].CreateShape(f);
                        hitbox->Shape.Compound.AddShape(f, ref createdShape);
                    }

                    hitbox->Settings = settings;
                    hitbox->Lifetime = lifetime;
                    hitbox->Owner = user;

                    hitbox->Parent = settings.Parent switch
                    {
                        ParentType.None => EntityRef.None,
                        ParentType.User => user,
                        ParentType.MainWeapon => user,
                        ParentType.SubWeapon => user,
                        _ => user
                    };
                }

                QList<EntityRef> hitboxLists = f.ResolveList(stats->Hitboxes);
                hitboxLists.Add(hitboxEntity);
            }

            return hitboxEntity;
        }
    }
}