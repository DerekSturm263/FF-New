using Quantum.Collections;
using System.Runtime;

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
            Draw.Circle(filter.Transform->Position + filter.HitboxInstance->Settings.Offset, filter.HitboxInstance->Settings.Radius);

            --filter.HitboxInstance->Lifetime;

            if (filter.HitboxInstance->Lifetime <= 0)
            {
                if (f.Unsafe.TryGetPointer(filter.HitboxInstance->Owner, out Stats* stats))
                {
                    QList<EntityRef> hitboxes = f.ResolveList(stats->Hitboxes);
                    hitboxes.Remove(filter.Entity);
                }

                f.Destroy(filter.Entity);
            }
        }

        public static void SpawnHitbox(Frame f, HitboxSettings settings, int lifetime, EntityRef user)
        {
            Log.Debug("Spawning hitbox!");

            EntityPrototype hitboxPrototype = f.FindAsset<EntityPrototype>(settings.Prototype.Id);
            EntityRef hitboxEntity = f.Create(hitboxPrototype);

            if (f.Unsafe.TryGetPointer(hitboxEntity, out HitboxInstance* hitbox))
            {
                if (settings.Parent == ParentType.MainWeapon)
                {
                    if (f.Unsafe.TryGetPointer(user, out Stats* stats))
                    {
                        settings.Damage *= stats->MainWeaponStatsMultiplier.Damage;
                        settings.Knockback *= stats->MainWeaponStatsMultiplier.Knockback;
                    }
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

            {
                if (f.Unsafe.TryGetPointer(user, out Stats* stats))
                {
                    QList<EntityRef> hitboxLists = f.ResolveList(stats->Hitboxes);
                    hitboxLists.Add(hitboxEntity);
                }
            }
        }
    }
}