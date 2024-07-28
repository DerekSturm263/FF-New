using Photon.Deterministic;
using System.Collections.Generic;
using System.Linq;
using static Photon.Deterministic.DeterministicTickInput;

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

                HurtboxType type = (HurtboxType)(1 << index);

                var hurtboxes = f.ResolveDictionary(stats->Hurtboxes);
                hurtboxes[type] = hurtboxEntity;
            }
        }

        public static bool ModifyHealth(Frame f, EntityRef entityRef, Stats* stats, FP amount, bool triggerDeath)
        {
            return SetHealth(f, entityRef, stats, stats->CurrentHealth + amount * stats->HealthModifyMultiplier, triggerDeath);
        }

        public static bool SetHealth(Frame f, EntityRef entityRef, Stats* stats, FP amount, bool triggerDeath)
        {
            bool didDie = false;

            FP oldHealth = stats->CurrentHealth;
            stats->CurrentHealth = amount;

            if (stats->CurrentHealth > f.Global->CurrentMatch.Ruleset.Players.MaxHealth)
                stats->CurrentHealth = f.Global->CurrentMatch.Ruleset.Players.MaxHealth;

            f.Events.OnPlayerModifyHealth(entityRef, stats->Index, oldHealth, stats->CurrentHealth, f.Global->CurrentMatch.Ruleset.Players.MaxHealth);

            if (triggerDeath && stats->CurrentHealth <= 0)
            {
                ModifyStocks(f, entityRef, stats, -1);
                stats->CurrentHealth = f.Global->CurrentMatch.Ruleset.Players.MaxHealth;

                didDie = true;
                ++stats->Deaths;
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
            if (stats->CurrentEnergy < amount)
                return false;

            ModifyEnergy(f, entityRef, stats, amount);
            return true;
        }

        public static void ModifyEnergy(Frame f, EntityRef entityRef, Stats* stats, FP amount)
        {
            SetEnergy(f, entityRef, stats, stats->CurrentEnergy + amount * stats->EnergyModifyMultiplier * f.Global->CurrentMatch.Ruleset.Players.EnergyChargeRate);
        }

        public static void SetEnergy(Frame f, EntityRef entityRef, Stats* stats, FP amount)
        {
            FP oldEnergy = stats->CurrentEnergy;
            stats->CurrentEnergy = amount;

            if (stats->CurrentEnergy > f.Global->CurrentMatch.Ruleset.Players.MaxEnergy)
                stats->CurrentEnergy = f.Global->CurrentMatch.Ruleset.Players.MaxEnergy;
            else if (stats->CurrentEnergy < 0)
                stats->CurrentEnergy = 0;

            f.Events.OnPlayerModifyEnergy(entityRef, stats->Index, oldEnergy, stats->CurrentEnergy, f.Global->CurrentMatch.Ruleset.Players.MaxEnergy);
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
            SetStocks(f, entityRef, stats, stats->CurrentStocks + amount);
        }

        public static void SetStocks(Frame f, EntityRef entityRef, Stats* stats, int amount)
        {
            int oldStocks = stats->CurrentStocks;
            stats->CurrentStocks = amount;

            if (stats->CurrentStocks > f.Global->CurrentMatch.Ruleset.Players.StockCount)
                stats->CurrentStocks = f.Global->CurrentMatch.Ruleset.Players.StockCount;
            else if (stats->CurrentStocks < 0)
                stats->CurrentStocks = 0;

            f.Events.OnPlayerModifyStocks(entityRef, stats->Index, oldStocks, stats->CurrentStocks, f.Global->CurrentMatch.Ruleset.Players.StockCount);
        }

        public static void SetAllStocks(Frame f, int amount)
        {
            foreach (var stats in f.Unsafe.GetComponentBlockIterator<Stats>())
            {
                SetStocks(f, stats.Entity, stats.Component, amount);
            }
        }

        public static void SetShowReadiness(Frame f, EntityRef entityRef, Stats* stats, bool showReadiness)
        {
            f.Events.OnHideShowReadiness(entityRef, stats->Index, showReadiness);
        }

        public static void SetAllShowReadiness(Frame f, bool showReadiness)
        {
            foreach (var stats in f.Unsafe.GetComponentBlockIterator<Stats>())
            {
                SetShowReadiness(f, stats.Entity, stats.Component, showReadiness);
            }
        }

        public static void SetReadiness(Frame f, EntityRef entityRef, Stats* stats, bool isReady)
        {
            stats->ReadyTime = 0;
            stats->IsReady = isReady;

            //f.Events.OnPlayerUpdateReady(entityRef, stats->Index, stats->ReadyTime / FP._0_50);
            f.Events.OnPlayerCancel(entityRef, stats->Index);
        }

        public static void SetAllReadiness(Frame f, bool isReady)
        {
            foreach (var stats in f.Unsafe.GetComponentBlockIterator<Stats>())
            {
                SetReadiness(f, stats.Entity, stats.Component, isReady);
            }
        }

        public static void ResetTemporaryValues(Frame f, Stats* stats)
        {
            stats->ApparelStatsMultiplier = ApparelHelper.Default;
            stats->Deaths = 0;
            stats->EnergyModifyMultiplier = 1;
            stats->HealthModifyMultiplier = 1;

            stats->HeldItem = EntityRef.None;
            stats->Kills = 0;
            stats->StatusEffect.Id = AssetGuid.Invalid;
            stats->StatusEffectMultiplier = 1;
            stats->StatusEffectTimeLeft = 0;
            stats->WeaponStatsMultiplier = WeaponHelper.Default;
        }

        public static void ResetAllTemporaryValues(Frame f)
        {
            foreach (var stats in f.Unsafe.GetComponentBlockIterator<Stats>())
            {
                ResetTemporaryValues(f, stats.Component);
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

        public static void SetBuild(Frame f, EntityRef user, Stats* stats, Build build)
        {
            Build oldBuild = stats->Build;
            stats->Build = build;

            ApplyBuild(f, user, stats, build);

            f.Events.OnPlayerSetAvatar(user, oldBuild.Cosmetics.Avatar, build.Cosmetics.Avatar);
            f.Events.OnPlayerSetAltWeapon(user, oldBuild.Equipment.Weapons.AltWeapon, build.Equipment.Weapons.AltWeapon);
            f.Events.OnPlayerSetBadge(user, oldBuild.Equipment.Badge, build.Equipment.Badge);
            f.Events.OnPlayerSetClothing(user, oldBuild.Equipment.Outfit.Clothing, build.Equipment.Outfit.Clothing);
            f.Events.OnPlayerSetEmoteDown(user, oldBuild.Cosmetics.Emotes.Down, build.Cosmetics.Emotes.Down);
            f.Events.OnPlayerSetEmoteLeft(user, oldBuild.Cosmetics.Emotes.Left, build.Cosmetics.Emotes.Left);
            f.Events.OnPlayerSetEmoteRight(user, oldBuild.Cosmetics.Emotes.Right, build.Cosmetics.Emotes.Right);
            f.Events.OnPlayerSetEmoteUp(user, oldBuild.Cosmetics.Emotes.Right, build.Cosmetics.Emotes.Up);
            f.Events.OnPlayerSetEyes(user, oldBuild.Cosmetics.Eyes, build.Cosmetics.Eyes);
            f.Events.OnPlayerSetHair(user, oldBuild.Cosmetics.Hair, build.Cosmetics.Hair);
            f.Events.OnPlayerSetHeadgear(user, oldBuild.Equipment.Outfit.Headgear, build.Equipment.Outfit.Headgear);
            f.Events.OnPlayerSetLegwear(user, oldBuild.Equipment.Outfit.Legwear, build.Equipment.Outfit.Legwear);
            f.Events.OnPlayerSetMainWeapon(user, oldBuild.Equipment.Weapons.MainWeapon, build.Equipment.Weapons.MainWeapon);
            f.Events.OnPlayerSetSub(user, oldBuild.Equipment.Weapons.SubWeapon, build.Equipment.Weapons.SubWeapon);
            f.Events.OnPlayerSetUltimate(user, oldBuild.Equipment.Ultimate, build.Equipment.Ultimate);
            f.Events.OnPlayerSetVoice(user, oldBuild.Cosmetics.Voice, build.Cosmetics.Voice);
        }

        public static void ApplyBuild(Frame f, EntityRef user, Stats* stats, Build build)
        {
            ApplyBadge(f, user, stats->Build.Equipment.Badge);
        }

        public static void UnapplyBuild(Frame f, EntityRef user, Stats* stats)
        {
            UnapplyBadge(f, user, stats->Build.Equipment.Badge);
        }

        public static void RemoveBuild(Frame f, EntityRef user, Stats* stats)
        {
            UnapplyBuild(f, user, stats);

            f.Events.OnPlayerSetAvatar(user, stats->Build.Cosmetics.Avatar, default);
            f.Events.OnPlayerSetAltWeapon(user, stats->Build.Equipment.Weapons.AltWeapon, default);
            f.Events.OnPlayerSetBadge(user, stats->Build.Equipment.Badge, default);
            f.Events.OnPlayerSetClothing(user, stats->Build.Equipment.Outfit.Clothing, default);
            f.Events.OnPlayerSetEmoteDown(user, stats->Build.Cosmetics.Emotes.Down, default);
            f.Events.OnPlayerSetEmoteLeft(user, stats->Build.Cosmetics.Emotes.Left, default);
            f.Events.OnPlayerSetEmoteRight(user, stats->Build.Cosmetics.Emotes.Right, default);
            f.Events.OnPlayerSetEmoteUp(user, stats->Build.Cosmetics.Emotes.Up, default);
            f.Events.OnPlayerSetEyes(user, stats->Build.Cosmetics.Eyes, default);
            f.Events.OnPlayerSetHair(user, stats->Build.Cosmetics.Hair, default);
            f.Events.OnPlayerSetHeadgear(user, stats->Build.Equipment.Outfit.Headgear, default);
            f.Events.OnPlayerSetLegwear(user, stats->Build.Equipment.Outfit.Legwear, default);
            f.Events.OnPlayerSetMainWeapon(user, stats->Build.Equipment.Weapons.MainWeapon, default);
            f.Events.OnPlayerSetSub(user, stats->Build.Equipment.Weapons.SubWeapon, default);
            f.Events.OnPlayerSetUltimate(user, stats->Build.Equipment.Ultimate, default);
            f.Events.OnPlayerSetVoice(user, stats->Build.Cosmetics.Voice, default);

            stats->Build = default;
        }

        public static void SetAltWeapon(Frame f, EntityRef user, Stats* stats, Weapon altWeapon)
        {
            Weapon oldAltWeapon = stats->Build.Equipment.Weapons.AltWeapon;
            stats->Build.Equipment.Weapons.AltWeapon = altWeapon;

            f.Events.OnPlayerSetAltWeapon(user, oldAltWeapon, altWeapon);
        }

        public static void SetAvatar(Frame f, EntityRef user, Stats* stats, AssetRefFFAvatar avatar)
        {
            AssetRefFFAvatar oldAvatar = stats->Build.Cosmetics.Avatar;
            stats->Build.Cosmetics.Avatar = avatar;

            f.Events.OnPlayerSetAvatar(user, oldAvatar, avatar);
        }

        public static void SetVoice(Frame f, EntityRef user, Stats* stats, AssetRefVoice voice)
        {
            AssetRefVoice oldVoice = stats->Build.Cosmetics.Voice;
            stats->Build.Cosmetics.Voice = voice;

            f.Events.OnPlayerSetVoice(user, oldVoice, voice);
        }

        public static void SetBadge(Frame f, EntityRef user, Stats* stats, AssetRefBadge badge)
        {
            AssetRefBadge oldBadge = stats->Build.Equipment.Badge;
            stats->Build.Equipment.Badge = badge;

            ApplyBadge(f, user, badge);

            f.Events.OnPlayerSetBadge(user, oldBadge, badge);
        }

        public static void SetClothing(Frame f, EntityRef user, Stats* stats, Apparel clothing)
        {
            Apparel oldClothing = stats->Build.Equipment.Outfit.Clothing;
            stats->Build.Equipment.Outfit.Clothing = clothing;

            f.Events.OnPlayerSetClothing(user, oldClothing, clothing);
        }

        public static void SetEmoteDown(Frame f, EntityRef user, Stats* stats, AssetRefEmote emote)
        {
            AssetRefEmote oldEmote = stats->Build.Cosmetics.Emotes.Down;
            stats->Build.Cosmetics.Emotes.Down = emote;

            f.Events.OnPlayerSetEmoteDown(user, oldEmote, emote);
        }

        public static void SetEmoteLeft(Frame f, EntityRef user, Stats* stats, AssetRefEmote emote)
        {
            AssetRefEmote oldEmote = stats->Build.Cosmetics.Emotes.Left;
            stats->Build.Cosmetics.Emotes.Left = emote;

            f.Events.OnPlayerSetEmoteLeft(user, oldEmote, emote);
        }

        public static void SetEmoteRight(Frame f, EntityRef user, Stats* stats, AssetRefEmote emote)
        {
            AssetRefEmote oldEmote = stats->Build.Cosmetics.Emotes.Right;
            stats->Build.Cosmetics.Emotes.Right = emote;

            f.Events.OnPlayerSetEmoteRight(user, oldEmote, emote);
        }

        public static void SetEmoteUp(Frame f, EntityRef user, Stats* stats, AssetRefEmote emote)
        {
            AssetRefEmote oldEmote = stats->Build.Cosmetics.Emotes.Up;
            stats->Build.Cosmetics.Emotes.Up = emote;

            f.Events.OnPlayerSetEmoteUp(user, oldEmote, emote);
        }

        public static void SetEyes(Frame f, EntityRef user, Stats* stats, AssetRefEyes eyes)
        {
            AssetRefEyes oldEyes = stats->Build.Cosmetics.Eyes;
            stats->Build.Cosmetics.Eyes = eyes;

            f.Events.OnPlayerSetEyes(user, oldEyes, eyes);
        }

        public static void SetHair(Frame f, EntityRef user, Stats* stats, AssetRefHair hair)
        {
            AssetRefHair oldHair = stats->Build.Cosmetics.Hair;
            stats->Build.Cosmetics.Hair = oldHair;

            f.Events.OnPlayerSetHair(user, oldHair, hair);
        }

        public static void SetHeadgear(Frame f, EntityRef user, Stats* stats, Apparel headgear)
        {
            Apparel oldHeadgear = stats->Build.Equipment.Outfit.Headgear;
            stats->Build.Equipment.Outfit.Headgear = headgear;

            f.Events.OnPlayerSetHeadgear(user, oldHeadgear, headgear);
        }

        public static void SetLegwear(Frame f, EntityRef user, Stats* stats, Apparel legwear)
        {
            Apparel oldOutfit = stats->Build.Equipment.Outfit.Legwear;
            stats->Build.Equipment.Outfit.Legwear = legwear;

            f.Events.OnPlayerSetLegwear(user, oldOutfit, legwear);
        }

        public static void SetMainWeapon(Frame f, EntityRef user, Stats* stats, Weapon mainWeapon)
        {
            Weapon oldMainWeapon = stats->Build.Equipment.Weapons.MainWeapon;
            stats->Build.Equipment.Weapons.MainWeapon = mainWeapon;

            f.Events.OnPlayerSetMainWeapon(user, oldMainWeapon, mainWeapon);
        }

        public static void SetSub(Frame f, EntityRef user, Stats* stats, Sub sub)
        {
            Sub oldSub = stats->Build.Equipment.Weapons.SubWeapon;
            stats->Build.Equipment.Weapons.SubWeapon = sub;

            f.Events.OnPlayerSetSub(user, oldSub, sub);
        }

        public static void SetUltimate(Frame f, EntityRef user, Stats* stats, AssetRefUltimate ultimate)
        {
            AssetRefUltimate oldUltimate = stats->Build.Equipment.Ultimate;
            stats->Build.Equipment.Ultimate = ultimate;

            f.Events.OnPlayerSetUltimate(user, oldUltimate, ultimate);
        }

        public static void ApplyBadge(Frame f, EntityRef user, AssetRefBadge badgeAsset)
        {
            if (f.TryFindAsset(badgeAsset.Id, out Badge badge))
                badge.OnApply(f, user);
        }

        public static void UnapplyBadge(Frame f, EntityRef user, AssetRefBadge badgeAsset)
        {
            if (f.TryFindAsset(badgeAsset.Id, out Badge badge))
                badge.OnRemove(f, user);
        }

        public static void RemoveBadge(Frame f, EntityRef user, Stats* stats)
        {
            UnapplyBadge(f, user, stats->Build.Equipment.Badge);

            AssetRefBadge oldBadge = stats->Build.Equipment.Badge;
            stats->Build.Equipment.Badge = default;

            f.Events.OnPlayerSetBadge(user, oldBadge, default);
        }

        public static EntityRef FindNearestOtherPlayer(Frame f, EntityRef user)
        {
            List<EntityRef> players = [];

            foreach (var stats in f.GetComponentIterator<Stats>())
            {
                players.Add(stats.Entity);
            }

            players.Remove(user);

            Transform2D userTransform = f.Get<Transform2D>(user);
            players.OrderBy(item => FPVector2.DistanceSquared(userTransform.Position, f.Get<Transform2D>(item).Position));

            return players[0];
        }

        public static void ModifyHurtboxes(Frame f, EntityRef entity, HurtboxType hurtboxesType, HurtboxSettings settings)
        {
            if (f.Unsafe.TryGetPointer(entity, out Stats* stats))
            {
                var hurtboxes = f.ResolveDictionary(stats->Hurtboxes);

                for (int i = 0; i < 15; ++i)
                {
                    HurtboxType hurtboxType = (HurtboxType)(1 << i);
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
