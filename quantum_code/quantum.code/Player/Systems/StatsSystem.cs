using Photon.Deterministic;
using Quantum.Custom.Animator;
using System;

namespace Quantum
{
    public unsafe class StatsSystem : SystemMainThreadFilter<StatsSystem.Filter>, ISignalOnComponentAdded<Stats>, ISignalOnComponentRemoved<Stats>
    {
        public struct Filter
        {
            public EntityRef Entity;

            public Transform2D* Transform;
            public CharacterController* CharacterController;
            public CustomAnimator* CustomAnimator;
            public Stats* Stats;
        }

        public override void Update(Frame f, ref Filter filter)
        {
            UpdateHurtboxes(f, ref filter);

            UpdateStatusEffect(f, filter.Entity, filter.Stats);
        }

        public void OnAdded(Frame f, EntityRef entity, Stats* component)
        {
            if (f.TryGet(entity, out PlayerStats playerStats))
            {
                component->Hitboxes = f.AllocateList<EntityRef>();
                component->Hurtboxes = f.AllocateDictionary<HurtboxType, EntityRef>();

                for (int i = 0; i < 15; ++i)
                {
                    EntityRef hurtbox = f.CreateChilded(f.RuntimeConfig.Hurtbox, entity);

                    if (f.Unsafe.TryGetPointer(hurtbox, out HurtboxInstance* hurtboxInstance))
                    {
                        SetHurtbox(f, entity, hurtboxInstance, hurtbox, i);
                    }

                    if (f.Unsafe.TryGetPointer(hurtbox, out PhysicsCollider2D* physicsCollider2D))
                    {
                        HurtboxSetup hurtboxSetup = f.FindAsset<HurtboxSetup>(playerStats.HurtboxSetup.Id);
                        physicsCollider2D->Shape.Circle.Radius = hurtboxSetup.HurtboxSizes[i];
                    }
                }
            }
        }

        public void OnRemoved(Frame f, EntityRef entity, Stats* component)
        {
            if (f.TryGet(entity, out PlayerStats _))
            {
                f.FreeList(component->Hitboxes);
                component->Hitboxes = default;

                f.FreeDictionary(component->Hurtboxes);
                component->Hurtboxes = default;
            }
        }

        private void UpdateHurtboxes(Frame f, ref Filter filter)
        {
            var dictionary = f.ResolveDictionary(filter.Stats->Hurtboxes);

            foreach (var hurtbox in dictionary)
            {
                UpdateHurtbox(f, ref filter, hurtbox.Value);
            }
        }

        private void UpdateHurtbox(Frame f, ref Filter filter, EntityRef hurtbox)
        {
            if (f.Unsafe.TryGetPointer(hurtbox, out ChildParentLink* childParentLink) && f.Unsafe.TryGetPointer(hurtbox, out HurtboxInstance* hurtboxInstance))
            {
                HurtboxTransformInfo transform = CustomAnimator.GetFrame(f, filter.CustomAnimator).hurtboxPositions[hurtboxInstance->Index];

                childParentLink->LocalPosition = transform.position;
                childParentLink->LocalRotation = transform.rotation;

                if (filter.CharacterController->MovementDirection < 0)
                    childParentLink->LocalPosition.X *= -1;
            }
        }

        public static void SetHurtbox(Frame f, EntityRef owner, HurtboxInstance* hurtbox, EntityRef hurtboxEntity, int index)
        {
            if (f.Unsafe.TryGetPointer(owner, out Stats* stats))
            {
                hurtbox->Owner = owner;
                hurtbox->Index = index;

                HurtboxType type = (HurtboxType)(1 << index);

                var hurtboxes = f.ResolveDictionary(stats->Hurtboxes);
                hurtboxes[type] = hurtboxEntity;
            }
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

        public static bool ModifyHealth(Frame f, EntityRef entityRef, Stats* stats, FP amount, bool triggerDeath)
        {
            if (f.Unsafe.TryGetPointer(entityRef, out PlayerStats* playerStats))
            {
                if (amount < 0)
                    playerStats->Stats.TotalDamageTaken += amount;
                else
                    playerStats->Stats.TotalDamageHealed += amount;
            }

            return SetHealth(f, entityRef, stats, stats->CurrentStats.Health + amount * stats->StatsMultiplier.Health, triggerDeath);
        }

        public static bool SetHealth(Frame f, EntityRef entityRef, Stats* stats, FP amount, bool triggerDeath)
        {
            bool didDie = false;

            FP oldHealth = stats->CurrentStats.Health;
            stats->CurrentStats.Health = amount;

            if (stats->CurrentStats.Health > f.Global->CurrentMatch.Ruleset.Players.MaxHealth)
                stats->CurrentStats.Health = f.Global->CurrentMatch.Ruleset.Players.MaxHealth;

            if (f.Unsafe.TryGetPointer(entityRef, out PlayerStats* playerStats))
                f.Events.OnPlayerModifyHealth(entityRef, playerStats->Index, oldHealth, stats->CurrentStats.Health, f.Global->CurrentMatch.Ruleset.Players.MaxHealth);

            if (triggerDeath && stats->CurrentStats.Health <= 0)
            {
                ModifyStocks(f, entityRef, stats, -1);
                stats->CurrentStats.Health = f.Global->CurrentMatch.Ruleset.Players.MaxHealth;

                didDie = true;
            }

            return didDie;

            /*if (stats->CurrentHealth <= stats->MaxHealth / 5)
                f.Events.OnPlayerLowHealth(*playerLink);*/
        }

        public static void SetAllHealth(Frame f, FP amount)
        {
            foreach (var stats in f.Unsafe.GetComponentBlockIterator<Stats>())
            {
                SetHealth(f, stats.Entity, stats.Component, amount, false);
            }
        }

        public static bool TryModifyEnergy(Frame f, EntityRef entityRef, Stats* stats, FP amount)
        {
            if (stats->CurrentStats.Energy < amount)
                return false;

            ModifyEnergy(f, entityRef, stats, amount);
            return true;
        }

        public static void ModifyEnergy(Frame f, EntityRef entityRef, Stats* stats, FP amount)
        {
            if (f.Unsafe.TryGetPointer(entityRef, out PlayerStats* playerStats))
            {
                if (amount > 0)
                    playerStats->Stats.TotalEnergyGenerated += amount;
                else
                    playerStats->Stats.TotalEnergyConsumed += amount;
            }

            SetEnergy(f, entityRef, stats, stats->CurrentStats.Energy + (amount * stats->StatsMultiplier.Energy * (amount > 0 ? f.Global->CurrentMatch.Ruleset.Players.EnergyChargeRate : f.Global->CurrentMatch.Ruleset.Players.EnergyConsumptionRate)));
        }

        public static void SetEnergy(Frame f, EntityRef entityRef, Stats* stats, FP amount)
        {
            FP oldEnergy = stats->CurrentStats.Energy;
            stats->CurrentStats.Energy = amount;

            if (stats->CurrentStats.Energy > f.Global->CurrentMatch.Ruleset.Players.MaxEnergy)
                stats->CurrentStats.Energy = f.Global->CurrentMatch.Ruleset.Players.MaxEnergy;
            else if (stats->CurrentStats.Energy < 0)
                stats->CurrentStats.Energy = 0;

            if (f.Unsafe.TryGetPointer(entityRef, out PlayerStats* playerStats))
                f.Events.OnPlayerModifyEnergy(entityRef, playerStats->Index, oldEnergy, stats->CurrentStats.Energy, f.Global->CurrentMatch.Ruleset.Players.MaxEnergy);
        }

        public static void SetAllEnergy(Frame f, FP amount)
        {
            foreach (var stats in f.Unsafe.GetComponentBlockIterator<Stats>())
            {
                SetEnergy(f, stats.Entity, stats.Component, amount);
            }
        }

        public static void ModifyStocks(Frame f, EntityRef entityRef, Stats* stats, int amount)
        {
            SetStocks(f, entityRef, stats, stats->CurrentStats.Stocks + amount);
        }

        public static void SetStocks(Frame f, EntityRef entityRef, Stats* stats, int amount)
        {
            int oldStocks = stats->CurrentStats.Stocks;
            stats->CurrentStats.Stocks = amount;

            if (stats->CurrentStats.Stocks > f.Global->CurrentMatch.Ruleset.Players.StockCount)
                stats->CurrentStats.Stocks = f.Global->CurrentMatch.Ruleset.Players.StockCount;
            else if (stats->CurrentStats.Stocks < 0)
                stats->CurrentStats.Stocks = 0;

            if (f.Unsafe.TryGetPointer(entityRef, out PlayerStats* playerStats))
                f.Events.OnPlayerModifyStocks(entityRef, playerStats->Index, oldStocks, stats->CurrentStats.Stocks, f.Global->CurrentMatch.Ruleset.Players.StockCount);
        }

        public static void SetAllStocks(Frame f, int amount)
        {
            foreach (var stats in f.Unsafe.GetComponentBlockIterator<Stats>())
            {
                SetStocks(f, stats.Entity, stats.Component, amount);
            }
        }

        public static void GiveStatusEffect(Frame f, AssetRefStatusEffect statusEffectAsset, EntityRef entity, Stats* stats)
        {
            if (f.TryFindAsset(statusEffectAsset.Id, out StatusEffect statusEffect))
            {
                stats->StatusEffect = statusEffect;
                stats->StatusEffectTimeLeft = statusEffect.ActiveTime;

                statusEffect.OnApply(f, entity);
            }
        }

        public static void ResetTemporaryValues(Frame f, Stats* stats)
        {
            stats->StatsMultiplier = new() { Energy = 1, Health = 1 };
            stats->StatusEffect.Id = AssetGuid.Invalid;
            stats->StatusEffectMultiplier = 1;
            stats->StatusEffectTimeLeft = 0;
        }

        public static void ResetAllTemporaryValues(Frame f)
        {
            foreach (var stats in f.Unsafe.GetComponentBlockIterator<Stats>())
            {
                ResetTemporaryValues(f, stats.Component);
            }
        }

        public static void ModifyHurtboxes(Frame f, EntityRef entity, HurtboxType hurtboxesType, HurtboxSettings settings)
        {
            if (f.Unsafe.TryGetPointer(entity, out Stats* stats))
            {
                var hurtboxes = f.ResolveDictionary(stats->Hurtboxes);

                for (int i = 0; i < 15; ++i)
                {
                    HurtboxType hurtboxType = (HurtboxType)Math.Pow(2, i);
                    if (!hurtboxesType.HasFlag(hurtboxType))
                        continue;

                    if (f.Unsafe.TryGetPointer(hurtboxes[hurtboxType], out HurtboxInstance* hurtbox))
                    {
                        hurtbox->Settings = settings;
                    }
                }

                f.Events.OnHurtboxStateChange(entity, hurtboxesType, settings);
            }
        }
    }
}
