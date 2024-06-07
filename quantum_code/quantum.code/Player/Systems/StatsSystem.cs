using Photon.Deterministic;
using System.Collections.Generic;
using System.Linq;

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

        public static bool ModifyHealth(Frame f, EntityRef entityRef, Stats* stats, FP amount)
        {
            return SetHealth(f, entityRef, stats, stats->CurrentHealth + amount * stats->HealthModifyMultiplier);
        }

        public static bool SetHealth(Frame f, EntityRef entityRef, Stats* stats, FP amount)
        {
            bool didDie = false;

            FP oldHealth = stats->CurrentHealth;
            stats->CurrentHealth = amount;

            if (f.Unsafe.TryGetPointerSingleton(out MatchInstance* matchInstance))
            {
                if (stats->CurrentHealth > matchInstance->Match.Ruleset.Players.MaxHealth)
                    stats->CurrentHealth = matchInstance->Match.Ruleset.Players.MaxHealth;

                f.Events.OnPlayerModifyHealth(entityRef, stats->PlayerIndex, oldHealth, stats->CurrentHealth, matchInstance->Match.Ruleset.Players.MaxHealth);

                if (stats->CurrentHealth <= 0)
                {
                    ModifyStocks(f, entityRef, stats, -1);
                    stats->CurrentHealth = matchInstance->Match.Ruleset.Players.MaxHealth;

                    didDie = true;
                    ++stats->Deaths;
                }
            }

            return didDie;

            /*if (stats->CurrentHealth <= stats->MaxHealth / 5)
                f.Events.OnPlayerLowHealth(*playerLink);*/
        }

        public static void SetAllHealth(Frame f, FP amount)
        {
            foreach (var stats in f.Unsafe.GetComponentBlockIterator<Stats>())
            {
                SetHealth(f, stats.Entity, stats.Component, amount);
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
            if (f.Unsafe.TryGetPointerSingleton(out MatchInstance* matchInstance))
            {
                SetEnergy(f, entityRef, stats, stats->CurrentEnergy + amount * stats->EnergyModifyMultiplier * matchInstance->Match.Ruleset.Players.EnergyChargeRate);
            }
        }

        public static void SetEnergy(Frame f, EntityRef entityRef, Stats* stats, FP amount)
        {
            FP oldEnergy = stats->CurrentEnergy;
            stats->CurrentEnergy = amount;

            if (f.Unsafe.TryGetPointerSingleton(out MatchInstance* matchInstance))
            {
                if (stats->CurrentEnergy > matchInstance->Match.Ruleset.Players.MaxEnergy)
                    stats->CurrentEnergy = matchInstance->Match.Ruleset.Players.MaxEnergy;
                else if (stats->CurrentEnergy < 0)
                    stats->CurrentEnergy = 0;

                f.Events.OnPlayerModifyEnergy(entityRef, stats->PlayerIndex, oldEnergy, stats->CurrentEnergy, matchInstance->Match.Ruleset.Players.MaxEnergy);
            }
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

            if (f.Unsafe.TryGetPointerSingleton(out MatchInstance* matchInstance))
            {
                if (stats->CurrentStocks > matchInstance->Match.Ruleset.Players.StockCount)
                    stats->CurrentStocks = matchInstance->Match.Ruleset.Players.StockCount;
                else if (stats->CurrentStocks < 0)
                    stats->CurrentStocks = 0;

                f.Events.OnPlayerModifyStocks(entityRef, stats->PlayerIndex, oldStocks, stats->CurrentStocks, matchInstance->Match.Ruleset.Players.StockCount);
            }
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

        public static void SetBuild(Frame f, EntityRef user, Stats* stats, Build build)
        {
            Build oldBuild = stats->Build;
            stats->Build = build;

            ApplyBuild(f, user, stats, build);

            f.Events.OnPlayerSetAltWeapon(user, stats->PlayerIndex, oldBuild.Equipment.Weapons.AltWeapon, build.Equipment.Weapons.AltWeapon);
            f.Events.OnPlayerSetAvatar(user, stats->PlayerIndex, oldBuild.Cosmetics.Avatar, build.Cosmetics.Avatar);
            f.Events.OnPlayerSetVoice(user, stats->PlayerIndex, oldBuild.Cosmetics.Voice, build.Cosmetics.Voice);
            f.Events.OnPlayerSetBadge(user, stats->PlayerIndex, oldBuild.Equipment.Badge, build.Equipment.Badge);
            f.Events.OnPlayerSetClothing(user, stats->PlayerIndex, oldBuild.Equipment.Outfit.Clothing, build.Equipment.Outfit.Clothing);
            f.Events.OnPlayerSetEmoteDown(user, stats->PlayerIndex, oldBuild.Cosmetics.Emotes.Down, build.Cosmetics.Emotes.Down);
            f.Events.OnPlayerSetEmoteLeft(user, stats->PlayerIndex, oldBuild.Cosmetics.Emotes.Left, build.Cosmetics.Emotes.Left);
            f.Events.OnPlayerSetEmoteRight(user, stats->PlayerIndex, oldBuild.Cosmetics.Emotes.Right, build.Cosmetics.Emotes.Right);
            f.Events.OnPlayerSetEmoteUp(user, stats->PlayerIndex, oldBuild.Cosmetics.Emotes.Right, build.Cosmetics.Emotes.Up);
            f.Events.OnPlayerSetEyes(user, stats->PlayerIndex, oldBuild.Cosmetics.Eyes, build.Cosmetics.Eyes);
            f.Events.OnPlayerSetHair(user, stats->PlayerIndex, oldBuild.Cosmetics.Hair, build.Cosmetics.Hair);
            f.Events.OnPlayerSetHeadgear(user, stats->PlayerIndex, oldBuild.Equipment.Outfit.Headgear, build.Equipment.Outfit.Headgear);
            f.Events.OnPlayerSetLegwear(user, stats->PlayerIndex, oldBuild.Equipment.Outfit.Legwear, build.Equipment.Outfit.Legwear);
            f.Events.OnPlayerSetMainWeapon(user, stats->PlayerIndex, oldBuild.Equipment.Weapons.MainWeapon, build.Equipment.Weapons.MainWeapon);
            f.Events.OnPlayerSetSub(user, stats->PlayerIndex, oldBuild.Equipment.Weapons.SubWeapon, build.Equipment.Weapons.SubWeapon);
            f.Events.OnPlayerSetUltimate(user, stats->PlayerIndex, oldBuild.Equipment.Ultimate, build.Equipment.Ultimate);
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

            f.Events.OnPlayerSetAltWeapon(user, stats->PlayerIndex, stats->Build.Equipment.Weapons.AltWeapon, default);
            f.Events.OnPlayerSetAvatar(user, stats->PlayerIndex, stats->Build.Cosmetics.Avatar, default);
            f.Events.OnPlayerSetVoice(user, stats->PlayerIndex, stats->Build.Cosmetics.Voice, default);
            f.Events.OnPlayerSetBadge(user, stats->PlayerIndex, stats->Build.Equipment.Badge, default);
            f.Events.OnPlayerSetClothing(user, stats->PlayerIndex, stats->Build.Equipment.Outfit.Clothing, default);
            f.Events.OnPlayerSetEmoteDown(user, stats->PlayerIndex, stats->Build.Cosmetics.Emotes.Down, default);
            f.Events.OnPlayerSetEmoteLeft(user, stats->PlayerIndex, stats->Build.Cosmetics.Emotes.Left, default);
            f.Events.OnPlayerSetEmoteRight(user, stats->PlayerIndex, stats->Build.Cosmetics.Emotes.Right, default);
            f.Events.OnPlayerSetEmoteUp(user, stats->PlayerIndex, stats->Build.Cosmetics.Emotes.Up, default);
            f.Events.OnPlayerSetEyes(user, stats->PlayerIndex, stats->Build.Cosmetics.Eyes, default);
            f.Events.OnPlayerSetHair(user, stats->PlayerIndex, stats->Build.Cosmetics.Hair, default);
            f.Events.OnPlayerSetHeadgear(user, stats->PlayerIndex, stats->Build.Equipment.Outfit.Headgear, default);
            f.Events.OnPlayerSetLegwear(user, stats->PlayerIndex, stats->Build.Equipment.Outfit.Legwear, default);
            f.Events.OnPlayerSetMainWeapon(user, stats->PlayerIndex, stats->Build.Equipment.Weapons.MainWeapon, default);
            f.Events.OnPlayerSetSub(user, stats->PlayerIndex, stats->Build.Equipment.Weapons.SubWeapon, default);
            f.Events.OnPlayerSetUltimate(user, stats->PlayerIndex, stats->Build.Equipment.Ultimate, default);

            stats->Build = default;
        }

        public static void SetAltWeapon(Frame f, EntityRef user, Stats* stats, Weapon altWeapon)
        {
            Weapon oldAltWeapon = stats->Build.Equipment.Weapons.AltWeapon;
            stats->Build.Equipment.Weapons.AltWeapon = altWeapon;

            f.Events.OnPlayerSetAltWeapon(user, stats->PlayerIndex, oldAltWeapon, altWeapon);
        }

        public static void SetAvatar(Frame f, EntityRef user, Stats* stats, AssetRefFFAvatar avatar)
        {
            AssetRefFFAvatar oldAvatar = stats->Build.Cosmetics.Avatar;
            stats->Build.Cosmetics.Avatar = avatar;

            f.Events.OnPlayerSetAvatar(user, stats->PlayerIndex, oldAvatar, avatar);
        }

        public static void SetVoice(Frame f, EntityRef user, Stats* stats, AssetRefVoice voice)
        {
            AssetRefVoice oldVoice = stats->Build.Cosmetics.Voice;
            stats->Build.Cosmetics.Voice = voice;

            f.Events.OnPlayerSetVoice(user, stats->PlayerIndex, oldVoice, voice);
        }

        public static void SetBadge(Frame f, EntityRef user, Stats* stats, AssetRefBadge badge)
        {
            AssetRefBadge oldBadge = stats->Build.Equipment.Badge;
            stats->Build.Equipment.Badge = badge;

            ApplyBadge(f, user, badge);

            f.Events.OnPlayerSetBadge(user, stats->PlayerIndex, oldBadge, badge);
        }

        public static void SetClothing(Frame f, EntityRef user, Stats* stats, Apparel clothing)
        {
            Apparel oldClothing = stats->Build.Equipment.Outfit.Clothing;
            stats->Build.Equipment.Outfit.Clothing = clothing;

            f.Events.OnPlayerSetClothing(user, stats->PlayerIndex, oldClothing, clothing);
        }

        public static void SetEmoteDown(Frame f, EntityRef user, Stats* stats, AssetRefEmote emote)
        {
            AssetRefEmote oldEmote = stats->Build.Cosmetics.Emotes.Down;
            stats->Build.Cosmetics.Emotes.Down = emote;

            f.Events.OnPlayerSetEmoteDown(user, stats->PlayerIndex, oldEmote, emote);
        }

        public static void SetEmoteLeft(Frame f, EntityRef user, Stats* stats, AssetRefEmote emote)
        {
            AssetRefEmote oldEmote = stats->Build.Cosmetics.Emotes.Left;
            stats->Build.Cosmetics.Emotes.Left = emote;

            f.Events.OnPlayerSetEmoteLeft(user, stats->PlayerIndex, oldEmote, emote);
        }

        public static void SetEmoteRight(Frame f, EntityRef user, Stats* stats, AssetRefEmote emote)
        {
            AssetRefEmote oldEmote = stats->Build.Cosmetics.Emotes.Right;
            stats->Build.Cosmetics.Emotes.Right = emote;

            f.Events.OnPlayerSetEmoteRight(user, stats->PlayerIndex, oldEmote, emote);
        }

        public static void SetEmoteUp(Frame f, EntityRef user, Stats* stats, AssetRefEmote emote)
        {
            AssetRefEmote oldEmote = stats->Build.Cosmetics.Emotes.Up;
            stats->Build.Cosmetics.Emotes.Up = emote;

            f.Events.OnPlayerSetEmoteUp(user, stats->PlayerIndex, oldEmote, emote);
        }

        public static void SetEyes(Frame f, EntityRef user, Stats* stats, AssetRefEyes eyes)
        {
            AssetRefEyes oldEyes = stats->Build.Cosmetics.Eyes;
            stats->Build.Cosmetics.Eyes = eyes;

            f.Events.OnPlayerSetEyes(user, stats->PlayerIndex, oldEyes, eyes);
        }

        public static void SetHair(Frame f, EntityRef user, Stats* stats, AssetRefHair hair)
        {
            AssetRefHair oldHair = stats->Build.Cosmetics.Hair;
            stats->Build.Cosmetics.Hair = oldHair;

            f.Events.OnPlayerSetHair(user, stats->PlayerIndex, oldHair, hair);
        }

        public static void SetHeadgear(Frame f, EntityRef user, Stats* stats, Apparel headgear)
        {
            Apparel oldHeadgear = stats->Build.Equipment.Outfit.Headgear;
            stats->Build.Equipment.Outfit.Headgear = headgear;

            f.Events.OnPlayerSetHeadgear(user, stats->PlayerIndex, oldHeadgear, headgear);
        }

        public static void SetLegwear(Frame f, EntityRef user, Stats* stats, Apparel legwear)
        {
            Apparel oldOutfit = stats->Build.Equipment.Outfit.Legwear;
            stats->Build.Equipment.Outfit.Legwear = legwear;

            f.Events.OnPlayerSetLegwear(user, stats->PlayerIndex, oldOutfit, legwear);
        }

        public static void SetMainWeapon(Frame f, EntityRef user, Stats* stats, Weapon mainWeapon)
        {
            Weapon oldMainWeapon = stats->Build.Equipment.Weapons.MainWeapon;
            stats->Build.Equipment.Weapons.MainWeapon = mainWeapon;

            f.Events.OnPlayerSetMainWeapon(user, stats->PlayerIndex, oldMainWeapon, mainWeapon);
        }

        public static void SetSub(Frame f, EntityRef user, Stats* stats, Sub sub)
        {
            Sub oldSub = stats->Build.Equipment.Weapons.SubWeapon;
            stats->Build.Equipment.Weapons.SubWeapon = sub;

            f.Events.OnPlayerSetSub(user, stats->PlayerIndex, oldSub, sub);
        }

        public static void SetUltimate(Frame f, EntityRef user, Stats* stats, AssetRefUltimate ultimate)
        {
            AssetRefUltimate oldUltimate = stats->Build.Equipment.Ultimate;
            stats->Build.Equipment.Ultimate = ultimate;

            f.Events.OnPlayerSetUltimate(user, stats->PlayerIndex, oldUltimate, ultimate);
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

            f.Events.OnPlayerSetBadge(user, stats->PlayerIndex, oldBadge, default);
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
