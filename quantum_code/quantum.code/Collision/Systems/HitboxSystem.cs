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

            if (filter.HitboxInstance->Parent.IsValid)
                filter.Transform->Position += f.Unsafe.GetPointer<Transform2D>(filter.HitboxInstance->Parent)->Position;

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

        public static EntityRef SpawnHitbox(Frame f, HitboxSettings settings, Shape2D shape, int lifetime, ref CharacterControllerSystem.Filter user, List<FPVector2> positions, EntityRef parent, SpawnHitboxEvent eventParent)
        {
            Log.Debug("Spawning hitbox!");

            EntityPrototype hitboxPrototype = f.FindAsset<EntityPrototype>(f.RuntimeConfig.Hitbox.Id);

            if (!settings.Visual.OnlyShakeOnHit)
                f.Events.OnCameraShake(settings.Visual.CameraShake, settings.Offensive.Knockback.Normalized, true, EntityRef.None);

            EntityRef hitboxEntity = f.Create(hitboxPrototype);

            if (f.Unsafe.TryGetPointer(hitboxEntity, out HitboxInstance* hitboxInstance))
            {
                hitboxInstance->Shape = shape;
                hitboxInstance->Shape.LocalTransform = Transform2D.Create();

                hitboxInstance->Settings = settings;
                hitboxInstance->Lifetime = lifetime;
                hitboxInstance->Owner = user.Entity;
                hitboxInstance->Parent = parent;
                hitboxInstance->Event = eventParent;

                hitboxInstance->Positions = f.AllocateList<FPVector2>();
                QList<FPVector2> positionsComponent = f.ResolveList(hitboxInstance->Positions);

                for (int i = 0; i < positions.Count; ++i)
                {
                    positionsComponent.Add(new FPVector2(positions[i].X * (parent == user.Entity ? user.CharacterController->MovementDirection : 1), positions[i].Y));
                }

                QList<EntityRef> hitboxLists = f.ResolveList(user.Stats->Hitboxes);
                hitboxLists.Add(hitboxEntity);

                f.Events.OnHitboxSpawnDespawn(user.Entity, hitboxEntity, true);
            }

            return hitboxEntity;
        }

        public static HitboxSettings AdjustForSettingsAndDirection(Frame f, HitboxSettings settings, ref CharacterControllerSystem.Filter attacker, EntityRef defender, FPVector2 hitboxPosition)
        {
            // Grab the players' stats from their outfits.
            ApparelStats apparelStats = f.Unsafe.TryGetPointer(defender, out PlayerStats* defenderPlayerStats) ? ApparelHelper.FromStats(f, defenderPlayerStats) : ApparelHelper.Default;
            WeaponStats weaponStats = WeaponHelper.FromStats(f, attacker.PlayerStats);

            // Update damage.
            FP updatedDamage = settings.Offensive.Damage * (1 / apparelStats.Defense) * weaponStats.Damage;

            // Update direction.
            int directionMultiplier = 1;
            if (f.TryGet(defender, out Transform2D transform))
            {
                if (settings.Offensive.AlignKnockbackToPlayerDirection)
                {
                    directionMultiplier = attacker.CharacterController->MovementDirection;
                }
                else
                {
                    if (hitboxPosition.X > transform.Position.X)
                        directionMultiplier = -1;
                    else
                        directionMultiplier = 1;
                }
            }

            FP newX = settings.Offensive.Knockback.X * directionMultiplier;
            FP newY = settings.Offensive.Knockback.Y;

            FPVector2 updatedDirection = FPVector2.Scale(new(newX, newY), f.Unsafe.GetPointer<CharacterController>(defender)->KnockbackMultiplier);
            updatedDirection *= weaponStats.Knockback;

            settings.Offensive.Damage = updatedDamage;
            settings.Offensive.Knockback = updatedDirection;

            return settings;
        }

        public static void ResolveHit(Frame f, HitboxSettings hitbox, HurtboxSettings hurtbox, ref CharacterControllerSystem.Filter attacker, EntityRef defender, FPVector2 position, int hitboxLifetime, AssetRefSpawnHitboxEvent eventParent)
        {
            if (f.Unsafe.TryGetPointer(defender, out Stats* stats) && stats->IFrameTime > 0)
                return;

            if (hurtbox.CanBeDamaged)
            {
                ResolveDamage(f, hitbox, hurtbox, ref attacker, defender);
            }

            if (hurtbox.CanBeKnockedBack)
            {
                CharacterControllerSystem.ApplyKnockback(f, hitbox, ref attacker, defender, 1, hitboxLifetime);

                if (hitbox.Visual.OnlyShakeOnHit && hitbox.Offensive.Knockback != default)
                    f.Events.OnCameraShake(hitbox.Visual.CameraShake, hitbox.Offensive.Knockback.Normalized, false, defender);
            }

            if (hurtbox.CanBeDamaged && hurtbox.CanBeKnockedBack)
            {
                if (f.TryGet(defender, out PlayerStats defenderStats))
                    f.Events.OnHitboxHurtboxCollision(attacker.Entity, attacker.PlayerStats->Index, defender, defenderStats.Index, hitbox, position, eventParent);
                else
                    f.Events.OnHitboxHurtboxCollision(attacker.Entity, attacker.PlayerStats->Index, defender, FighterIndex.Invalid, hitbox, position, eventParent);
            }

            if (attacker.CharacterController->DoResetBuffs)
            {
                attacker.PlayerStats->ApparelStatsMultiplier = ApparelHelper.Default;
                attacker.PlayerStats->WeaponStatsMultiplier = WeaponHelper.Default;

                attacker.CharacterController->DoResetBuffs = false;
            }
        }

        private static void ResolveDamage(Frame f, HitboxSettings hitbox, HurtboxSettings hurtbox, ref CharacterControllerSystem.Filter attacker, EntityRef defender)
        {
            if (!f.Unsafe.TryGetPointer(defender, out Stats* defenderStats))
                return;

            if (f.Unsafe.TryGetPointer(defender, out PlayerStats* defenderPlayerStats))
                f.Events.OnPlayerHit(defender, defenderPlayerStats->Index, -hitbox.Offensive.Damage);

            attacker.PlayerStats->Stats.TotalDamageDealt += hitbox.Offensive.Damage;

            if (StatsSystem.ModifyHealth(f, defender, defenderStats, -hitbox.Offensive.Damage, true))
            {
                ++attacker.PlayerStats->Stats.Kills;

                if (f.Unsafe.TryGetPointer(defender, out PlayerStats* defenderPlayerStats2))
                {
                    ++defenderPlayerStats2->Stats.Deaths;
                }
            }
            else
            {
                StatsSystem.GiveStatusEffect(f, hitbox.Offensive.StatusEffect, defender, defenderStats);
            }

            if (attacker.PlayerStats->ActiveWeapon == ActiveWeaponType.Primary)
            {
                if (attacker.PlayerStats->Build.Gear.MainWeapon.Enhancer.Id.IsValid)
                {
                    WeaponEnhancer weaponEnhancer = f.FindAsset<WeaponEnhancer>(attacker.PlayerStats->Build.Gear.MainWeapon.Enhancer.Id);
                    weaponEnhancer.OnHit(f, ref attacker, defender, hitbox);
                }
            }
            else
            {
                if (attacker.PlayerStats->Build.Gear.AltWeapon.Enhancer.Id.IsValid)
                {
                    WeaponEnhancer weaponEnhancer = f.FindAsset<WeaponEnhancer>(attacker.PlayerStats->Build.Gear.AltWeapon.Enhancer.Id);
                    weaponEnhancer.OnHit(f, ref attacker, defender, hitbox);
                }
            }

            StatsSystem.ModifyEnergy(f, attacker.Entity, attacker.Stats, hitbox.Offensive.Damage / 2);
        }
    }
}