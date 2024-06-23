using Photon.Deterministic;

namespace Quantum
{
    public unsafe class HitboxHurtboxRetrievalSystem : SystemMainThread
    {
        public override void Update(Frame f)
        {
            var hitboxFilter = f.Unsafe.FilterStruct<HitboxHurtboxQueryInjectionSystem.Filter>();
            var hitbox = default(HitboxHurtboxQueryInjectionSystem.Filter);

            while (hitboxFilter.Next(&hitbox))
            {
                Physics2D.HitCollection hits = f.Physics2D.GetQueryHits(hitbox.Hitbox->PathQueryIndex);

                for (int i = 0; i < hits.Count; ++i)
                {
                    EntityRef entityHit = hits[i].Entity;

                    if (f.Unsafe.TryGetPointer(entityHit, out HurtboxInstance* hurtbox))
                    {
                        EntityRef ownerHit = hurtbox->Owner;

                        if (ownerHit == hitbox.Hitbox->Owner)
                            continue;

                        f.Events.OnCameraShake(hitbox.Hitbox->Settings.HitShake, hitbox.Hitbox->Settings.Knockback.Normalized);

                        Log.Debug($"Hitbox hit {entityHit.Index}");

                        if (hurtbox->Settings.DisableHitbox)
                            break;

                        if (hurtbox->Settings.CanBeDamaged && f.Unsafe.TryGetPointer(ownerHit, out Stats* hitStats))
                        {
                            // Grab the hit player's stats from their outfit.
                            ApparelStats apparelStats = ApparelHelper.Default;
                            {
                                apparelStats = ApparelHelper.Add(ApparelHelper.FromApparel(f, hitStats->Build.Equipment.Outfit.Headgear), apparelStats);
                                apparelStats = ApparelHelper.Add(ApparelHelper.FromApparel(f, hitStats->Build.Equipment.Outfit.Clothing), apparelStats);
                                apparelStats = ApparelHelper.Add(ApparelHelper.FromApparel(f, hitStats->Build.Equipment.Outfit.Legwear), apparelStats);
                            }

                            // Apply damage.
                            FP damage = -hitbox.Hitbox->Settings.Damage * (1 / apparelStats.Defense);
                            f.Events.OnPlayerHit(ownerHit, hitStats->GetIndex(f, ownerHit), damage);

                            if (StatsSystem.ModifyHealth(f, ownerHit, hitStats, damage))
                            {
                                if (f.Unsafe.TryGetPointer(hitbox.Hitbox->Owner, out Stats* stats))
                                    ++stats->Kills;
                            }
                            else
                            {
                                StatsSystem.GiveStatusEffect(f, hitbox.Hitbox->Settings.StatusEffect, ownerHit, hitStats);
                            }

                            // Increase energy.
                            if (f.Unsafe.TryGetPointer(hitbox.Hitbox->Owner, out Stats* ownerStats))
                            {
                                StatsSystem.ModifyEnergy(f, hitbox.Hitbox->Owner, ownerStats, hitbox.Hitbox->Settings.Damage / 5);
                            }
                        }

                        // Apply knockback.
                        if (hurtbox->Settings.CanBeKnockedBack && f.Unsafe.TryGetPointer(ownerHit, out PhysicsBody2D* physicsBody))
                        {
                            physicsBody->Velocity = hitbox.Hitbox->Settings.Knockback;
                        }

                        if (hurtbox->Settings.CanBeInterrupted && f.Unsafe.TryGetPointer(ownerHit, out CustomAnimator* customAnimator))
                        {
                            if (hitbox.Hitbox->Settings.Knockback.SqrMagnitude > 5 * 5)
                            {
                                CustomAnimator.SetTrigger(f, customAnimator, "Knocked Back");
                            }
                            else
                            {
                                CustomAnimator.SetTrigger(f, customAnimator, "Hurt");
                            }
                        }

                        f.Events.OnHitboxHurtboxCollection(hitbox.Hitbox->Owner, ownerHit, hitbox.Hitbox->Settings);
                    }
                }
            }
        }
    }
}
