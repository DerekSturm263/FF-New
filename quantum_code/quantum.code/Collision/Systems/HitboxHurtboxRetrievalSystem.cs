using Photon.Deterministic;
using Quantum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

                        Log.Debug($"Hitbox hit {entityHit.Index}");

                        if (hurtbox->Settings.DisableHitbox)
                            break;

                        if (hurtbox->Settings.CanBeDamaged && f.Unsafe.TryGetPointer(ownerHit, out Stats* hitStats))
                        {
                            if (f.Unsafe.TryGetPointer(ownerHit, out PlayerLink* hitPlayerLink))
                            {
                                // Grab the hit player's stats from their outfit.
                                ApparelStats apparelStats = ApparelHelper.Default;
                                {
                                    apparelStats = ApparelHelper.Add(ApparelHelper.FromApparel(f, hitStats->Build.Equipment.Outfit.Headgear), apparelStats);
                                    apparelStats = ApparelHelper.Add(ApparelHelper.FromApparel(f, hitStats->Build.Equipment.Outfit.Clothing), apparelStats);
                                    apparelStats = ApparelHelper.Add(ApparelHelper.FromApparel(f, hitStats->Build.Equipment.Outfit.Legwear), apparelStats);
                                }

                                // Apply damage.
                                if (StatsSystem.ModifyHealth(f, hitPlayerLink, hitStats, -hitbox.Hitbox->Settings.Damage * (1 / apparelStats.Defense)))
                                {
                                    if (f.Unsafe.TryGetPointer(hitbox.Hitbox->Owner, out Stats* stats))
                                        ++stats->Kills;
                                }
                            }

                            if (f.Unsafe.TryGetPointer(hitbox.Hitbox->Owner, out PlayerLink* ownerPlayerLink))
                            {
                                if (f.Unsafe.TryGetPointer(hitbox.Hitbox->Owner, out Stats* ownerStats))
                                {
                                    // Increase energy.
                                    StatsSystem.ModifyEnergy(f, ownerPlayerLink, ownerStats, hitbox.Hitbox->Settings.Damage / 5);
                                }
                            }

                            if (f.TryFindAsset(hitbox.Hitbox->Settings.StatusEffect.Id, out StatusEffect statusEffect))
                            {
                                // Apply status effect.
                                hitStats->StatusEffect = statusEffect;
                                hitStats->StatusEffectTimeLeft = statusEffect.ActiveTime;

                                statusEffect.OnApply(f, ownerHit);
                            }
                        }

                        if (hurtbox->Settings.CanBeKnockedBack && f.Unsafe.TryGetPointer(ownerHit, out PhysicsBody2D* physicsBody))
                        {
                            // Apply knockback.
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

                        f.Events.OnHitboxHurtboxCollection(f.Get<PlayerLink>(hitbox.Hitbox->Owner), f.Get<PlayerLink>(ownerHit), hitbox.Hitbox->Settings);
                    }
                }
            }
        }
    }
}
