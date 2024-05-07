using Photon.Deterministic;

namespace Quantum
{
    public unsafe class StatsSystem : SystemMainThreadFilter<StatsSystem.Filter>, ISignalOnComponentAdded<Stats>, ISignalOnComponentRemoved<Stats>
    {
        public struct Filter
        {
            public EntityRef Entity;
            public Stats* Stats;
        }

        public struct PlayerLinkStatsFilter
        {
            public EntityRef Entity;
            
            public PlayerLink* PlayerLink;
            public Stats* Stats;
        }

        public override void Update(Frame f, ref Filter filter)
        {
            Input input = default;
            PlayerRef playerRef = default;
            if (f.Unsafe.TryGetPointer(filter.Entity, out PlayerLink* playerLink))
            {
                playerRef = playerLink->Player;
                input = *f.GetPlayerInput(playerRef);
            }

            if (f.TryFindAsset(filter.Stats->Build.Equipment.Badge.Id, out Badge badge))
            {
                badge.OnUpdate(f, filter.Entity);
            }

            UpdateStatusEffect(f, filter.Entity, filter.Stats);
        }

        private void UpdateStatusEffect(Frame f, EntityRef entity, Stats* stats)
        {
            if (stats->StatusEffectTimeLeft > 0)
            {
                stats->StatusEffectTimeLeft--;
                StatusEffect statusEffect = f.FindAsset<StatusEffect>(stats->StatusEffect.Id);

                if (stats->StatusEffectTimeLeft == 0)
                {
                    statusEffect.OnRemove(f, entity);
                }
                else if (stats->StatusEffectTimeLeft % statusEffect.TickRate == 0)
                {
                    statusEffect.OnTick(f, entity);
                }
            }
        }

        public void OnAdded(Frame f, EntityRef entity, Stats* component)
        {
            component->Hitboxes = f.AllocateList<EntityRef>();
            component->Hurtboxes = f.AllocateDictionary<HurtboxType, EntityRef>();

            for (int i = 0; i < 15; ++i)
            {
                EntityRef hurtbox = f.Create(component->Hurtbox);

                if (f.Unsafe.TryGetPointer(hurtbox, out HurtboxInstance* hurtboxInstance))
                {
                    SetHurtbox(f, entity, hurtboxInstance, hurtbox, i);
                }
            }

            ApplyBuild(f, entity, component, component->Build);
        }

        public void OnRemoved(Frame f, EntityRef entity, Stats* component)
        {
            RemoveBuild(f, entity, component);

            f.FreeList(component->Hitboxes);
            component->Hitboxes = default;

            f.FreeDictionary(component->Hurtboxes);
            component->Hurtboxes = default;
        }

        public static void SetHurtbox(Frame f, EntityRef owner, HurtboxInstance* hurtbox, EntityRef hurtboxEntity, int index)
        {
            if (f.Unsafe.TryGetPointer(owner, out Stats* stats))
            {
                hurtbox->Owner = owner;
                hurtbox->Index = index;

                var hurtboxes = f.ResolveDictionary(stats->Hurtboxes);
                hurtboxes[(HurtboxType)(1 << index)] = hurtboxEntity;
            }
        }

        public static bool ModifyHealth(Frame f, PlayerLink* playerLink, Stats* stats, FP amount)
        {
            return SetHealth(f, playerLink, stats, stats->CurrentHealth + amount * stats->HealthModifyMultiplier);
        }

        public static bool SetHealth(Frame f, PlayerLink* playerLink, Stats* stats, FP amount)
        {
            bool didDie = false;

            FP oldHealth = stats->CurrentHealth;
            stats->CurrentHealth = amount;

            if (f.Unsafe.TryGetPointerSingleton(out MatchInstance* matchInstance))
            {
                if (stats->CurrentHealth > matchInstance->Match.Ruleset.Players.MaxHealth)
                    stats->CurrentHealth = matchInstance->Match.Ruleset.Players.MaxHealth;

                f.Events.OnPlayerModifyHealth(*playerLink, oldHealth, stats->CurrentHealth, matchInstance->Match.Ruleset.Players.MaxHealth);

                if (stats->CurrentHealth <= 0)
                {
                    ModifyStocks(f, playerLink, stats, -1);
                    stats->CurrentHealth = matchInstance->Match.Ruleset.Players.MaxHealth;

                    didDie = true;
                    ++stats->Deaths;
                }
            }

            return didDie;

            /*if (stats->CurrentHealth <= stats->MaxHealth / 5)
                f.Events.OnPlayerLowHealth(*playerLink);*/
        }

        public static bool TryModifyEnergy(Frame f, PlayerLink* playerLink, Stats* stats, FP amount)
        {
            if (stats->CurrentEnergy < amount)
                return false;

            ModifyEnergy(f, playerLink, stats, amount);
            return true;
        }

        public static void ModifyEnergy(Frame f, PlayerLink* playerLink, Stats* stats, FP amount)
        {
            if (f.Unsafe.TryGetPointerSingleton(out MatchInstance* matchInstance))
            {
                SetEnergy(f, playerLink, stats, stats->CurrentEnergy + amount * stats->EnergyModifyMultiplier * matchInstance->Match.Ruleset.Players.EnergyChargeRate);
            }
        }

        public static void SetEnergy(Frame f, PlayerLink* playerLink, Stats* stats, FP amount)
        {
            FP oldEnergy = stats->CurrentEnergy;
            stats->CurrentEnergy = amount;

            if (f.Unsafe.TryGetPointerSingleton(out MatchInstance* matchInstance))
            {
                if (stats->CurrentEnergy > matchInstance->Match.Ruleset.Players.MaxEnergy)
                    stats->CurrentEnergy = matchInstance->Match.Ruleset.Players.MaxEnergy;
                else if (stats->CurrentEnergy < 0)
                    stats->CurrentEnergy = 0;

                f.Events.OnPlayerModifyEnergy(*playerLink, oldEnergy, stats->CurrentEnergy, matchInstance->Match.Ruleset.Players.MaxEnergy);
            }
        }

        public static void ModifyStocks(Frame f, PlayerLink* playerLink, Stats* stats, int amount)
        {
            SetStocks(f, playerLink, stats, stats->CurrentStocks + amount);
        }

        public static void SetStocks(Frame f, PlayerLink* playerLink, Stats* stats, int amount)
        {
            int oldStocks = stats->CurrentStocks;
            stats->CurrentStocks = amount;

            if (f.Unsafe.TryGetPointerSingleton(out MatchInstance* matchInstance))
            {
                if (stats->CurrentStocks > matchInstance->Match.Ruleset.Players.StockCount)
                    stats->CurrentStocks = matchInstance->Match.Ruleset.Players.StockCount;
                else if (stats->CurrentStocks < 0)
                    stats->CurrentStocks = 0;

                f.Events.OnPlayerModifyStocks(*playerLink, oldStocks, stats->CurrentStocks, matchInstance->Match.Ruleset.Players.StockCount);
            }
        }

        public static void GiveStatusEffect(Frame f, AssetRefStatusEffect statusEffectAsset, EntityRef entity, Stats* stats)
        {
            if (f.TryFindAsset(statusEffectAsset.Id, out StatusEffect statusEffect))
            {
                // Apply status effect.
                stats->StatusEffect = statusEffect;
                stats->StatusEffectTimeLeft = statusEffect.ActiveTime;

                statusEffect.OnApply(f, entity);
            }
        }

        public static void ApplyBuild(Frame f, EntityRef user, Stats* stats, Build build)
        {
            stats->Build = build;

            if (f.TryFindAsset(build.Equipment.Badge.Id, out Badge badge1))
                badge1.OnApply(f, user);
        }

        public static void RemoveBuild(Frame f, EntityRef user, Stats* stats)
        {
            if (f.TryFindAsset(stats->Build.Equipment.Badge.Id, out Badge badge1))
                badge1.OnRemove(f, user);

            stats->Build = default;
        }
    }
}
