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
            DrawHitbox(filter.Transform, filter.HitboxInstance);

            --filter.HitboxInstance->Lifetime;
            if (filter.HitboxInstance->Lifetime <= 0)
            {
                KillHitbox(f, filter.HitboxInstance->Owner, filter.Entity);
            }
        }

        [Conditional("DEBUG")]
        private void DrawHitbox(Transform2D* transform, HitboxInstance* hitboxInstance)
        {
            Draw.Circle(transform->Position + hitboxInstance->Settings.Offset, hitboxInstance->Settings.Radius);
        }

        private void KillHitbox(Frame f, EntityRef owner, EntityRef hitbox)
        {
            if (f.Unsafe.TryGetPointer(owner, out Stats* stats))
            {
                QList<EntityRef> hitboxes = f.ResolveList(stats->Hitboxes);
                hitboxes.Remove(hitbox);
            }

            f.Destroy(hitbox);
        }

        public static EntityRef SpawnHitbox(Frame f, HitboxSettings settings, int lifetime, EntityRef user)
        {
            Log.Debug("Spawning hitbox!");

            EntityPrototype hitboxPrototype = f.FindAsset<EntityPrototype>(settings.Prototype.Id);
            EntityRef hitboxEntity = f.Create(hitboxPrototype);

            if (f.Unsafe.TryGetPointer(user, out Stats* stats))
            {
                if (f.Unsafe.TryGetPointer(hitboxEntity, out HitboxInstance* hitbox))
                {
                    if (settings.Parent == ParentType.MainWeapon)
                    {
                        settings.Damage *= stats->WeaponStatsMultiplier.Damage;
                        settings.Knockback *= stats->WeaponStatsMultiplier.Knockback;
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