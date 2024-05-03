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

        public override void Update(Frame f, ref Filter filter)
        {
            Input input = default;
            PlayerRef playerRef = default;
            if (f.Unsafe.TryGetPointer(filter.Entity, out PlayerLink* playerLink))
            {
                playerRef = playerLink->Player;
                input = *f.GetPlayerInput(playerRef);

                if (input.Emote)
                {
                    ModifyHealth(f, playerLink, filter.Stats, -10);
                }

                if (input.Block)
                {
                    ModifyEnergy(f, playerLink, filter.Stats, 10);
                }
            }

            if (filter.Stats->StatusEffectTimeLeft > 0)
            {
                filter.Stats->StatusEffectTimeLeft--;
                StatusEffect statusEffect = f.FindAsset<StatusEffect>(filter.Stats->StatusEffect.Id);

                if (filter.Stats->StatusEffectTimeLeft == 0)
                {
                    statusEffect.OnRemove(f, filter.Entity);
                }
                else if (filter.Stats->StatusEffectTimeLeft % statusEffect.TickRate == 0)
                {
                    statusEffect.OnTick(f, filter.Entity);
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

        public static void ModifyHealth(Frame f, PlayerLink* playerLink, Stats* stats, FP amount)
        {
            SetHealth(f, playerLink, stats, stats->CurrentHealth + amount * stats->HealthModifyMultiplier);
        }

        public static void SetHealth(Frame f, PlayerLink* playerLink, Stats* stats, FP amount)
        {
            FP oldHealth = stats->CurrentHealth;
            stats->CurrentHealth = amount;

            if (stats->CurrentHealth > stats->MaxHealth)
                stats->CurrentHealth = stats->MaxHealth;

            f.Events.OnPlayerModifyHealth(*playerLink, oldHealth, stats->CurrentHealth, stats->MaxHealth);

            if (stats->CurrentHealth <= 0)
            {
                ModifyStocks(f, playerLink, stats, -1);
                stats->CurrentHealth = stats->MaxHealth;
            }

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
            SetEnergy(f, playerLink, stats, stats->CurrentEnergy + amount * stats->EnergyModifyMultiplier);
        }

        public static void SetEnergy(Frame f, PlayerLink* playerLink, Stats* stats, FP amount)
        {
            FP oldEnergy = stats->CurrentEnergy;
            stats->CurrentEnergy = amount;

            if (stats->CurrentEnergy > stats->MaxEnergy)
                stats->CurrentEnergy = stats->MaxEnergy;

            f.Events.OnPlayerModifyEnergy(*playerLink, oldEnergy, stats->CurrentEnergy, stats->MaxEnergy);
        }

        public static void ModifyStocks(Frame f, PlayerLink* playerLink, Stats* stats, int amount)
        {
            SetStocks(f, playerLink, stats, stats->CurrentStocks + amount);
        }

        public static void SetStocks(Frame f, PlayerLink* playerLink, Stats* stats, int amount)
        {
            int oldStocks = stats->CurrentStocks;
            stats->CurrentStocks = amount;

            f.Events.OnPlayerModifyStocks(*playerLink, oldStocks, stats->CurrentStocks, stats->MaxStocks);
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
