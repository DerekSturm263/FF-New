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
            public DynamicHitboxInstance* HitboxInstance;
        }

        public override void Update(Frame f, ref Filter filter)
        {
            --filter.HitboxInstance->Lifetime;
            if (filter.HitboxInstance->Lifetime <= 0)
            {
                KillHitbox(f, filter.HitboxInstance->Owner, filter.Entity);
            }

            if (f.Unsafe.TryGetPointer(filter.HitboxInstance->Owner, out Transform2D* transform))
            {
                filter.Transform->Position = transform->Position;
            }

            DrawHitbox(f, filter.Transform, filter.HitboxInstance, filter.HitboxInstance->Shape);
        }

        [Conditional("DEBUG")]
        private void DrawHitbox(Frame f, Transform2D* transform, DynamicHitboxInstance* hitboxInstance, Shape2D shape)
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

            if (f.Unsafe.TryGetPointer(hitbox, out DynamicHitboxInstance* instance))
            {
                instance->Shape.Compound.FreePersistent(f);
                f.Events.OnHitboxSpawnDespawn(owner, hitbox, false);
            }

            f.Destroy(hitbox);
        }

        public static EntityRef SpawnDynamicHitbox(Frame f, HitboxSettings settings, Shape2DConfig shape, int lifetime, EntityRef user)
        {
            Log.Debug("Spawning dynamic hitbox!");

            EntityPrototype hitboxPrototype = f.FindAsset<EntityPrototype>(f.RuntimeConfig.Hitbox.Id);
            EntityRef hitboxEntity = f.Create(hitboxPrototype);

            if (f.Unsafe.TryGetPointer(user, out Stats* stats))
            {
                if (f.Unsafe.TryGetPointer(hitboxEntity, out DynamicHitboxInstance* hitbox))
                {
                    hitbox->Shape = Shape2D.CreatePersistentCompound();

                    for (int i = 0; i < shape.CompoundShapes.Length; ++i)
                    {
                        Shape2D createdShape = shape.CompoundShapes[i].CreateShape(f);
                        hitbox->Shape.Compound.AddShape(f, ref createdShape);
                    }

                    hitbox->Settings = settings;
                    hitbox->Lifetime = lifetime;
                    hitbox->Owner = user;
                }

                QList<EntityRef> hitboxLists = f.ResolveList(stats->Hitboxes);
                hitboxLists.Add(hitboxEntity);
            }

            f.Events.OnCameraShake(settings.SpawnShake, new FPVector2(f.Global->RngSession.Next(-FP._1, FP._1), f.Global->RngSession.Next(-FP._1, FP._1)), true);
            f.Events.OnHitboxSpawnDespawn(user, hitboxEntity, true);

            return hitboxEntity;
        }

        public static void SetStaticHitboxEnabled(Frame f, HitboxSettings hitboxSettings, EntityRef user, bool isActive)
        {
            if (f.Unsafe.TryGetPointer(user, out PlayerStats* playerStats))
            {
                if (!isActive)
                    playerStats->CurrentActiveWeapon = WeaponType.None;

                switch (playerStats->ActiveWeaponType)
                {
                    case WeaponType.Main:
                        playerStats->CurrentActiveWeapon = WeaponType.Main;
                        break;

                    case WeaponType.Alt:
                        playerStats->CurrentActiveWeapon = WeaponType.Alt;
                        break;
                };
            }

            /*if (settings.Parent == ParentType.MainWeapon)
            {
                settings.Damage *= stats->WeaponStatsMultiplier.Damage;
                settings.Knockback *= stats->WeaponStatsMultiplier.Knockback;
            }*/
        }
    }
}