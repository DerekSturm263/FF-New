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

                if (!f.Unsafe.ComponentGetter<CharacterControllerSystem.Filter>().TryGet(f, attacker, out CharacterControllerSystem.Filter playerFilter))
                    continue;

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
                            defenderStats.Index.Team == playerFilter.PlayerStats->Index.Team)
                            continue;

                        Log.Debug($"{attacker.Index} hit {defender.Index}");

                        FPVector2 positionHit = (hitbox.Transform->Position + f.Get<Transform2D>(entityHit).Position) / 2;

                        HitboxSettings adjusted = HitboxSystem.AdjustForSettingsAndDirection(f, hitbox.HitboxInstance->Settings, ref playerFilter, defender, hitbox.Transform->Position);
                        HitboxSystem.ResolveHit(f, adjusted, hurtbox->Settings, ref playerFilter, defender, positionHit, hitbox.HitboxInstance->Lifetime, hitbox.HitboxInstance->Event);
                    }
                }
            }
        }
    }
}
