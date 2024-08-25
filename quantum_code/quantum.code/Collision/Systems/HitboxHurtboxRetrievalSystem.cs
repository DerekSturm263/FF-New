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

                        ResolveHit(f, hitbox.HitboxInstance->Settings, hurtbox->Settings, hitbox.HitboxInstance->Owner, defender);
                    }
                }
            }
        }

        private void ResolveHit(Frame f, HitboxSettings hitbox, HurtboxSettings hurtbox, EntityRef attacker, EntityRef defender)
        {
            Log.Debug($"{attacker.Index} hit {defender.Index}");

            ResolveDamage(f, hitbox, hurtbox, attacker, defender);
            ResolveKnockback(f, hitbox, hurtbox, attacker, defender);

            if (hitbox.Visual.OnlyShakeOnHit)
                f.Events.OnCameraShake(hitbox.Visual.CameraShake, hitbox.Offensive.Knockback.Normalized, false, defender);

            if (f.TryGet(attacker, out PlayerStats attackerStats) && f.TryGet(defender, out PlayerStats defenderStats))
            {
                f.Events.OnHitboxHurtboxCollision(attacker, attackerStats.Index, defender, defenderStats.Index, hitbox);
            }
        }

        private void ResolveDamage(Frame f, HitboxSettings hitbox, HurtboxSettings hurtbox, EntityRef attacker, EntityRef defender)
        {
            if (hurtbox.CanBeDamaged &&
                f.Unsafe.TryGetPointer(defender, out Stats* defenderStats) &&
                f.Unsafe.TryGetPointer(attacker, out Stats* attackerStats))
            {
                // Grab the hit player's stats from their outfit.
                ApparelStats apparelStats = ApparelHelper.Default;

                if (f.Unsafe.TryGetPointer(attacker, out PlayerStats* playerStats))
                    apparelStats = ApparelHelper.FromStats(f, playerStats);

                // Apply damage.
                FP damage = -hitbox.Offensive.Damage * (1 / apparelStats.Defense);

                if (f.Unsafe.TryGetPointer(attacker, out PlayerStats* playerStats2))
                {
                    f.Events.OnPlayerHit(defender, playerStats2->Index, damage);
                }

                if (f.Unsafe.TryGetPointer(attacker, out PlayerStats* attackerPlayerStats))
                {
                    attackerPlayerStats->Stats.TotalDamageDealt += damage;

                    if (StatsSystem.ModifyHealth(f, defender, defenderStats, damage, true))
                    {
                        ++attackerPlayerStats->Stats.Kills;

                        if (f.Unsafe.TryGetPointer(defender, out PlayerStats* defenderPlayerStats))
                        {
                            ++defenderPlayerStats->Stats.Deaths;
                        }
                    }
                    else
                    {
                        StatsSystem.GiveStatusEffect(f, hitbox.Offensive.StatusEffect, defender, attackerStats);
                    }

                    // Increase energy.
                    FP multiplier = 1;

                    if (attackerPlayerStats->Build.Gear.MainWeapon.Enhancer.Id.IsValid)
                    {
                        WeaponEnhancer weaponEnhancer = f.FindAsset<WeaponEnhancer>(attackerPlayerStats->Build.Gear.MainWeapon.Enhancer.Id);

                        if (weaponEnhancer is ChargingWeaponEnhancer chargingWeaponEnhancer)
                            multiplier = chargingWeaponEnhancer.Multiplier;
                    }

                    StatsSystem.ModifyEnergy(f, attacker, attackerStats, (hitbox.Offensive.Damage / 5) * multiplier);
                }
            }
        }

        private void ResolveKnockback(Frame f, HitboxSettings hitbox, HurtboxSettings hurtbox, EntityRef attacker, EntityRef defender)
        {
            if (hurtbox.CanBeKnockedBack && f.Unsafe.TryGetPointer(attacker, out CharacterController* characterController2))
            {
                FPVector2 updatedDirection = new(hitbox.Offensive.Knockback.X * characterController2->MovementDirection, hitbox.Offensive.Knockback.Y);

                ShakeableSystem.Shake(f, attacker, hitbox.Visual.TargetShake, updatedDirection, hitbox.Delay.UserFreezeFrames, 0);
                ShakeableSystem.Shake(f, defender, hitbox.Visual.TargetShake, updatedDirection, hitbox.Delay.TargetFreezeFrames, hitbox.Delay.TargetShakeStrength);

                if (f.Unsafe.TryGetPointer(defender, out PhysicsBody2D* physicsBody) &&
                    f.Unsafe.TryGetPointer(defender, out CharacterController* characterController) &&
                    f.Unsafe.TryGetPointer(defender, out Transform2D* transform))
                {
                    characterController->DeferredKnockback = new() { Direction = updatedDirection, Time = hitbox.Offensive.Knockback.Magnitude / 12 };
                    characterController->OldKnockback = characterController->DeferredKnockback;
                    characterController->OriginalPosition = transform->Position;

                    characterController->Influence = 0;
                }
            }

            if (hurtbox.CanBeInterrupted && f.Unsafe.TryGetPointer(defender, out CustomAnimator* customAnimator))
            {
                if (hitbox.Offensive.Knockback.SqrMagnitude > 5 * 5)
                {
                    CustomAnimator.SetTrigger(f, customAnimator, "Knocked Back");
                }
                else
                {
                    CustomAnimator.SetTrigger(f, customAnimator, "Hit");
                }
            }
        }
    }
}
