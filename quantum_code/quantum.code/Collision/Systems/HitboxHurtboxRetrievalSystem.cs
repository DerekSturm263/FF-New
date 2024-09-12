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
                EntityRef attacker = hitbox.HitboxInstance->Owner;

                for (int i = 0; i < hits.Count; ++i)
                {
                    EntityRef entityHit = hits[i].Entity;

                    if (f.Unsafe.TryGetPointer(entityHit, out HurtboxInstance* hurtbox))
                    {
                        EntityRef defender = hurtbox->Owner;

                        if (defender == attacker)
                            continue;

                        if (f.Global->CurrentMatch.Ruleset.Players.AllowFriendlyFire &&
                            f.TryGet(defender, out PlayerStats defenderStats) &&
                            f.TryGet(attacker, out PlayerStats attackerStats) &&
                            defenderStats.Index.Team == attackerStats.Index.Team)
                            continue;

                        Log.Debug($"{attacker.Index} hit {defender.Index}");

                        FPVector2 positionHit = (hitbox.Transform->Position + f.Get<Transform2D>(entityHit).Position) / 2;

                        HitboxSettings adjusted = AdjustForSettingsAndDirection(f, hitbox.HitboxInstance->Settings, attacker, defender, hitbox.Transform->Position);
                        ResolveHit(f, adjusted, hurtbox->Settings, attacker, defender, positionHit, hitbox.HitboxInstance->Lifetime, hitbox.HitboxInstance->Event);
                    }
                }
            }
        }

        private void ResolveHit(Frame f, HitboxSettings hitbox, HurtboxSettings hurtbox, EntityRef attacker, EntityRef defender, FPVector2 position, int hitboxLifetime, AssetRefSpawnHitboxEvent eventParent)
        {
            if (f.Unsafe.TryGetPointer(defender, out Stats* stats) && stats->IFrameTime > 0)
                return;

            if (hurtbox.CanBeDamaged)
            {
                ResolveDamage(f, hitbox, hurtbox, attacker, defender);
            }

            if (hurtbox.CanBeKnockedBack)
            {
                CharacterControllerSystem.ApplyKnockback(f, hitbox, attacker, defender, 1, hitboxLifetime);

                if (hitbox.Visual.OnlyShakeOnHit && hitbox.Offensive.Knockback != default)
                    f.Events.OnCameraShake(hitbox.Visual.CameraShake, hitbox.Offensive.Knockback.Normalized, false, defender);
            }

            if (hurtbox.CanBeDamaged && hurtbox.CanBeKnockedBack && f.TryGet(attacker, out PlayerStats attackerStats))
            {
                if (f.TryGet(defender, out PlayerStats defenderStats))
                    f.Events.OnHitboxHurtboxCollision(attacker, attackerStats.Index, defender, defenderStats.Index, hitbox, position, eventParent);
                else
                    f.Events.OnHitboxHurtboxCollision(attacker, attackerStats.Index, defender, FighterIndex.Invalid, hitbox, position, eventParent);
            }
        }

        private HitboxSettings AdjustForSettingsAndDirection(Frame f, HitboxSettings settings, EntityRef attacker, EntityRef defender, FPVector2 hitboxPosition)
        {
            // Grab the players' stats from their outfits.
            ApparelStats apparelStats = f.Unsafe.TryGetPointer(defender, out PlayerStats* defenderPlayerStats) ? ApparelHelper.FromStats(f, defenderPlayerStats) : ApparelHelper.Default;
            WeaponStats weaponStats = f.Unsafe.TryGetPointer(attacker, out PlayerStats* attackerPlayerStats) ? WeaponHelper.FromStats(f, attackerPlayerStats) : WeaponHelper.Default;

            // Update damage.
            FP updatedDamage = settings.Offensive.Damage * (1 / apparelStats.Defense) * weaponStats.Damage;

            // Update direction.
            int directionMultiplier = 1;
            if (f.Unsafe.TryGetPointer(attacker, out CharacterController* characterController) && f.TryGet(defender, out Transform2D transform))
            {
                if (settings.Offensive.AlignKnockbackToPlayerDirection)
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
            }

            FP newX = settings.Offensive.Knockback.X * directionMultiplier;
            FP newY = settings.Offensive.Knockback.Y;

            FPVector2 updatedDirection = FPVector2.Scale(new(newX, newY), f.Unsafe.GetPointer<CharacterController>(defender)->KnockbackMultiplier);
            updatedDirection *= weaponStats.Knockback;

            settings.Offensive.Damage = updatedDamage;
            settings.Offensive.Knockback = updatedDirection;

            return settings;
        }

        private void ResolveDamage(Frame f, HitboxSettings hitbox, HurtboxSettings hurtbox, EntityRef attacker, EntityRef defender)
        {
            if (f.Unsafe.TryGetPointer(defender, out Stats* defenderStats) &&
                f.Unsafe.TryGetPointer(attacker, out Stats* attackerStats) &&
                f.Unsafe.TryGetPointer(attacker, out PlayerStats* attackerPlayerStats))
            {
                if (f.Unsafe.TryGetPointer(defender, out PlayerStats* defenderPlayerStats))
                    f.Events.OnPlayerHit(defender, defenderPlayerStats->Index, -hitbox.Offensive.Damage);

                attackerPlayerStats->Stats.TotalDamageDealt += hitbox.Offensive.Damage;

                if (StatsSystem.ModifyHealth(f, defender, defenderStats, -hitbox.Offensive.Damage, true))
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

                if (f.Unsafe.ComponentGetter<CharacterControllerSystem.Filter>().TryGet(f, attacker, out CharacterControllerSystem.Filter filter))
                {
                    if (attackerPlayerStats->ActiveWeapon == ActiveWeaponType.Primary)
                    {
                        if (attackerPlayerStats->Build.Gear.MainWeapon.Enhancer.Id.IsValid)
                        {
                            WeaponEnhancer weaponEnhancer = f.FindAsset<WeaponEnhancer>(attackerPlayerStats->Build.Gear.MainWeapon.Enhancer.Id);
                            weaponEnhancer.OnHit(f, ref filter, defender, hitbox);
                        }
                    }
                    else
                    {
                        if (attackerPlayerStats->Build.Gear.AltWeapon.Enhancer.Id.IsValid)
                        {
                            WeaponEnhancer weaponEnhancer = f.FindAsset<WeaponEnhancer>(attackerPlayerStats->Build.Gear.AltWeapon.Enhancer.Id);
                            weaponEnhancer.OnHit(f, ref filter, defender, hitbox);
                        }
                    }
                }

                StatsSystem.ModifyEnergy(f, attacker, attackerStats, hitbox.Offensive.Damage / 2);
            }
        }
    }
}
