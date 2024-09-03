using Photon.Deterministic;

namespace Quantum
{
    public unsafe class HitboxHurtboxRetrievalSystem : SystemMainThread
    {
        public override void Update(Frame f)
        {
            if (!f.Global->IsMatchRunning && f.Global->CurrentMatch.Ruleset.Match.Time != -1)
                return;

            var hitboxFilter = f.Unsafe.FilterStruct<HitboxHurtboxQueryInjectionSystem.Filter>();
            var hitbox = default(HitboxHurtboxQueryInjectionSystem.Filter);

            while (hitboxFilter.Next(&hitbox))
            {
                Physics2D.HitCollection hits = f.Physics2D.GetQueryHits(hitbox.HitboxInstance->PathQueryIndex);

                for (int i = 0; i < hits.Count; ++i)
                {
                    EntityRef entityHit = hits[i].Entity;

                    if (f.Unsafe.TryGetPointer(entityHit, out HurtboxInstance* hurtbox))
                    {
                        EntityRef defender = hurtbox->Owner;

                        if (defender == hitbox.HitboxInstance->Owner)
                            continue;

                        if (f.Global->CurrentMatch.Ruleset.Players.AllowFriendlyFire &&
                            f.TryGet(defender, out PlayerStats defenderStats) &&
                            f.TryGet(hitbox.HitboxInstance->Owner, out PlayerStats attackerStats) &&
                            defenderStats.Index.Team == attackerStats.Index.Team)
                            continue;

                        FPVector2 positionHit = (hitbox.Transform->Position + f.Get<Transform2D>(entityHit).Position) / 2;

                        ResolveHit(f, hitbox.Transform->Position, hitbox.HitboxInstance->Settings, hurtbox->Settings, hitbox.HitboxInstance->Owner, defender, positionHit, hitbox.HitboxInstance->Settings.Offensive.Knockback, hitbox.HitboxInstance->Lifetime);
                    }
                }
            }
        }

        private void ResolveHit(Frame f, FPVector2 hitboxPosition, HitboxSettings hitbox, HurtboxSettings hurtbox, EntityRef attacker, EntityRef defender, FPVector2 position, FPVector2 direction, int hitboxLifetime)
        {
            if (f.TryGet(defender, out Stats stats) && stats.IFrameTime > 0)
                return;

            Log.Debug($"{attacker.Index} hit {defender.Index}");

            // Grab the hit player's stats from their outfit.
            ApparelStats apparelStats = ApparelHelper.Default;
            WeaponStats weaponStats = WeaponHelper.Default;

            if (f.Unsafe.TryGetPointer(defender, out PlayerStats* defenderPlayerStats))
                apparelStats = ApparelHelper.FromStats(f, defenderPlayerStats);

            if (f.Unsafe.TryGetPointer(attacker, out PlayerStats* attackerPlayerStats))
                weaponStats = WeaponHelper.FromStats(f, attackerPlayerStats);

            ResolveDamage(f, hitbox, hurtbox, attacker, defender, apparelStats, weaponStats);
            ResolveKnockback(f, hitboxPosition, hitbox, hurtbox, attacker, defender, apparelStats, weaponStats, hitboxLifetime);

            if (hurtbox.CanBeDamaged)
            {
                if (hitbox.Visual.OnlyShakeOnHit)
                    f.Events.OnCameraShake(hitbox.Visual.CameraShake, hitbox.Offensive.Knockback.Normalized, false, defender);

                if (f.TryGet(attacker, out PlayerStats attackerStats))
                {
                    if (f.TryGet(defender, out PlayerStats defenderStats))
                        f.Events.OnHitboxHurtboxCollision(attacker, attackerStats.Index, defender, defenderStats.Index, hitbox, position, direction);
                    else
                        f.Events.OnHitboxHurtboxCollision(attacker, attackerStats.Index, defender, FighterIndex.Invalid, hitbox, position, direction);
                }
            }
        }

        private void ResolveDamage(Frame f, HitboxSettings hitbox, HurtboxSettings hurtbox, EntityRef attacker, EntityRef defender, ApparelStats apparelStats, WeaponStats weaponStats)
        {
            if (hurtbox.CanBeDamaged &&
                f.Unsafe.TryGetPointer(defender, out Stats* defenderStats) &&
                f.Unsafe.TryGetPointer(attacker, out Stats* attackerStats) &&
                f.Unsafe.TryGetPointer(attacker, out PlayerStats* attackerPlayerStats))
            {
                // Apply damage.
                FP damage = -hitbox.Offensive.Damage * (1 / apparelStats.Defense) * weaponStats.Damage;

                if (f.Unsafe.TryGetPointer(defender, out PlayerStats* defenderPlayerStats))
                    f.Events.OnPlayerHit(defender, defenderPlayerStats->Index, damage);

                attackerPlayerStats->Stats.TotalDamageDealt -= damage;

                if (StatsSystem.ModifyHealth(f, defender, defenderStats, damage, true))
                {
                    ++attackerPlayerStats->Stats.Kills;

                    if (f.Unsafe.TryGetPointer(defender, out PlayerStats* defenderPlayerStats2))
                    {
                        ++defenderPlayerStats2->Stats.Deaths;
                    }
                }
                else
                {
                    StatsSystem.GiveStatusEffect(f, hitbox.Offensive.StatusEffect, defender, defenderStats);
                }

                if (f.Unsafe.TryGetPointer(defender, out PlayerStats* defenderPlayerStats3))
                {
                    if (defenderPlayerStats3->Build.Gear.MainWeapon.Enhancer.Id.IsValid)
                    {
                        WeaponEnhancer weaponEnhancer = f.FindAsset<WeaponEnhancer>(defenderPlayerStats3->Build.Gear.MainWeapon.Enhancer.Id);

                        if (f.Unsafe.TryGetPointer(attacker, out CharacterController* characterController))
                            weaponEnhancer.OnHit(f, attacker, defender, hitbox, characterController->HoldLevel);
                    }
                }

                StatsSystem.ModifyEnergy(f, attacker, attackerStats, hitbox.Offensive.Damage / 2);
            }
        }

        private void ResolveKnockback(Frame f, FPVector2 hitboxPosition, HitboxSettings hitbox, HurtboxSettings hurtbox, EntityRef attacker, EntityRef defender, ApparelStats apparelStats, WeaponStats weaponStats, int hitboxLifetime)
        {
            hitbox.Offensive.Knockback *= weaponStats.Knockback;

            if (hurtbox.CanBeKnockedBack && f.Unsafe.TryGetPointer(attacker, out CharacterController* characterController) && f.TryGet(defender, out Transform2D transform))
            {
                int directionMultiplier;

                if (hitbox.Offensive.AlignKnockbackToPlayerDirection)
                {
                    directionMultiplier = characterController->MovementDirection;
                }
                else
                {
                    if (hitboxPosition.X > transform.Position.X)
                        directionMultiplier = -1;
                    else
                        directionMultiplier = 1;
                }

                CharacterControllerSystem.ApplyKnockback(f, hitbox, attacker, defender, directionMultiplier, 1, hitboxLifetime);
            }
        }
    }
}
