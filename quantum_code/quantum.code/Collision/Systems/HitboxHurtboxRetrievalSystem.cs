using Photon.Deterministic;

namespace Quantum
{
    public unsafe class HitboxHurtboxRetrievalSystem : SystemMainThread
    {
        public override void Update(Frame f)
        {
            if (!f.Global->IsMatchRunning)
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

            f.Events.OnCameraShake(hitbox.Visual.CameraShake, hitbox.Offensive.Knockback.Normalized, false);

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

                if (StatsSystem.ModifyHealth(f, defender, defenderStats, damage, true))
                {
                    if (f.Unsafe.TryGetPointer(attacker, out PlayerStats* attackerPlayerStats))
                    {
                        ++attackerPlayerStats->WinStats.Kills;
                    }

                    if (f.Unsafe.TryGetPointer(defender, out PlayerStats* defenderPlayerStats))
                    {
                        ++defenderPlayerStats->WinStats.Deaths;
                    }
                }
                else
                {
                    StatsSystem.GiveStatusEffect(f, hitbox.Offensive.StatusEffect, defender, attackerStats);
                }

                // Increase energy.
                FP multiplier = 1;

                if (f.Unsafe.TryGetPointer(attacker, out PlayerStats* attackerPlayerStats2))
                {
                    if (attackerPlayerStats2->Build.Equipment.Weapons.MainWeapon.Enhancer.Id.IsValid)
                    {
                        WeaponEnhancer weaponEnhancer = f.FindAsset<WeaponEnhancer>(attackerPlayerStats2->Build.Equipment.Weapons.MainWeapon.Enhancer.Id);
                        multiplier = (weaponEnhancer as ChargingWeaponEnhancer).Multiplier;
                    }
                }

                StatsSystem.ModifyEnergy(f, attacker, attackerStats, (hitbox.Offensive.Damage / 5) * multiplier);
            }
        }

        private void ResolveKnockback(Frame f, HitboxSettings hitbox, HurtboxSettings hurtbox, EntityRef attacker, EntityRef defender)
        {
            if (hurtbox.CanBeKnockedBack &&
                f.Unsafe.TryGetPointer(defender, out PhysicsBody2D* physicsBody) &&
                f.Unsafe.TryGetPointer(defender, out CharacterController* characterController))
            {
                characterController->KnockbackVelocityX = hitbox.Offensive.Knockback.X;
                physicsBody->Velocity.Y = hitbox.Offensive.Knockback.Y;

                characterController->KnockbackVelocityTime = 1;
                characterController->Influence = 0;
            }

            if (hurtbox.CanBeInterrupted && f.Unsafe.TryGetPointer(defender, out CustomAnimator* customAnimator))
            {
                if (hitbox.Offensive.Knockback.SqrMagnitude > 5 * 5)
                {
                    CustomAnimator.SetTrigger(f, customAnimator, "Knocked Back");
                }
                else
                {
                    CustomAnimator.SetTrigger(f, customAnimator, "Hurt");
                }
            }
        }
    }
}
