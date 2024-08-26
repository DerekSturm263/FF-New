using Photon.Deterministic;
using Quantum.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace Quantum
{
    public unsafe class HitboxSystem : SystemMainThreadFilter<HitboxSystem.Filter>, ISignalOnComponentRemoved<HitboxInstance>
    {
        public struct Filter
        {
            public EntityRef Entity;

            public Transform2D* Transform;
            public HitboxInstance* HitboxInstance;
        }

        public override void Update(Frame f, ref Filter filter)
        {
            var list = f.ResolveList(filter.HitboxInstance->Positions);

            if (list.Count == 1)
                filter.Transform->Position = list[0];
            else if (filter.HitboxInstance->Lifetime > 0 && filter.HitboxInstance->Lifetime < list.Count)
                filter.Transform->Position = list[filter.HitboxInstance->Lifetime - 1];

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

            f.FreeList(component->Positions);
            component->Positions = default;
        }

        [Conditional("DEBUG")]
        public static void DrawHitbox(Transform2D* transform, HitboxInstance* hitboxInstance)
        {
            ColorRGBA color = new() { R = 255, G = 255, B = 255, A = 255 };

            switch (hitboxInstance->Shape.Type)
            {
                case Shape2DType.Circle:
                    Draw.Circle(transform->Position + hitboxInstance->Shape.Centroid, hitboxInstance->Shape.Circle.Radius, color);
                    break;

                case Shape2DType.Box:
                    Draw.Rectangle(transform->Position + hitboxInstance->Shape.Centroid, hitboxInstance->Shape.Box.Extents * 2, transform->Rotation, color);
                    break;
            }
        }

        public static void SpawnHitbox(Frame f, HitboxSettings settings, Shape2D shape, int lifetime, EntityRef user, List<FPVector2> positions, bool parentToUser)
        {
            Log.Debug("Spawning hitbox!");

            EntityPrototype hitboxPrototype = f.FindAsset<EntityPrototype>(f.RuntimeConfig.Hitbox.Id);

            if (!settings.Visual.OnlyShakeOnHit)
                f.Events.OnCameraShake(settings.Visual.CameraShake, settings.Offensive.Knockback.Normalized, true, EntityRef.None);

            if (f.Unsafe.TryGetPointer(user, out Stats* stats))
            {
                EntityRef hitboxEntity = f.Create(hitboxPrototype);

                if (f.Unsafe.TryGetPointer(hitboxEntity, out HitboxInstance* hitboxInstance) && f.Unsafe.TryGetPointer(user, out CharacterController* characterController))
                {
                    hitboxInstance->Shape = shape;
                    hitboxInstance->Shape.LocalTransform = Transform2D.Create();

                    hitboxInstance->Settings = settings;
                    hitboxInstance->Lifetime = lifetime;
                    hitboxInstance->Owner = user;

                    hitboxInstance->Positions = f.AllocateList<FPVector2>();
                    QList<FPVector2> positionsComponent = f.ResolveList(hitboxInstance->Positions);

                    FPVector2 offset = default;

                    if (parentToUser && f.Unsafe.TryGetPointer(user, out Transform2D* transform))
                        offset = transform->Position;

                    for (int i = 0; i < positions.Count; ++i)
                    {
                        positionsComponent.Add(offset + new FPVector2(positions[i].X * (parentToUser ? characterController->MovementDirection : 1), positions[i].Y));
                    }

                    QList<EntityRef> hitboxLists = f.ResolveList(stats->Hitboxes);
                    hitboxLists.Add(hitboxEntity);

                    f.Events.OnHitboxSpawnDespawn(user, hitboxEntity, true);
                }
            }
        }
    }
}