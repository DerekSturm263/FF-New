using Photon.Deterministic;
using System;

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

            if (f.TryGet(user, out PlayerLink playerLink))
            {
                f.Events.OnPlayerSetAltWeapon(playerLink, oldBuild.Equipment.Weapons.AltWeapon, build.Equipment.Weapons.AltWeapon);
                f.Events.OnPlayerSetAvatar(playerLink, oldBuild.Cosmetics.Avatar, build.Cosmetics.Avatar);
                f.Events.OnPlayerSetBadge(playerLink, oldBuild.Equipment.Badge, build.Equipment.Badge);
                f.Events.OnPlayerSetClothing(playerLink, oldBuild.Equipment.Outfit.Clothing, build.Equipment.Outfit.Clothing);
                f.Events.OnPlayerSetEmoteDown(playerLink, oldBuild.Cosmetics.Emotes.Down, build.Cosmetics.Emotes.Down);
                f.Events.OnPlayerSetEmoteLeft(playerLink, oldBuild.Cosmetics.Emotes.Left, build.Cosmetics.Emotes.Left);
                f.Events.OnPlayerSetEmoteRight(playerLink, oldBuild.Cosmetics.Emotes.Right, build.Cosmetics.Emotes.Right);
                f.Events.OnPlayerSetEmoteUp(playerLink, oldBuild.Cosmetics.Emotes.Right, build.Cosmetics.Emotes.Up);
                f.Events.OnPlayerSetEyes(playerLink, oldBuild.Cosmetics.Eyes, build.Cosmetics.Eyes);
                f.Events.OnPlayerSetHair(playerLink, oldBuild.Cosmetics.Hair, build.Cosmetics.Hair);
                f.Events.OnPlayerSetHeadgear(playerLink, oldBuild.Equipment.Outfit.Headgear, build.Equipment.Outfit.Headgear);
                f.Events.OnPlayerSetLegwear(playerLink, oldBuild.Equipment.Outfit.Legwear, build.Equipment.Outfit.Legwear);
                f.Events.OnPlayerSetMainWeapon(playerLink, oldBuild.Equipment.Weapons.MainWeapon, build.Equipment.Weapons.MainWeapon);
                f.Events.OnPlayerSetSub(playerLink, oldBuild.Equipment.Weapons.SubWeapon, build.Equipment.Weapons.SubWeapon);
                f.Events.OnPlayerSetUltimate(playerLink, oldBuild.Equipment.Ultimate, build.Equipment.Ultimate);
            }
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

            if (f.TryGet(user, out PlayerLink playerLink))
            {
                f.Events.OnPlayerSetAltWeapon(playerLink, stats->Build.Equipment.Weapons.AltWeapon, default);
                f.Events.OnPlayerSetAvatar(playerLink, stats->Build.Cosmetics.Avatar, default);
                f.Events.OnPlayerSetBadge(playerLink, stats->Build.Equipment.Badge, default);
                f.Events.OnPlayerSetClothing(playerLink, stats->Build.Equipment.Outfit.Clothing, default);
                f.Events.OnPlayerSetEmoteDown(playerLink, stats->Build.Cosmetics.Emotes.Down, default);
                f.Events.OnPlayerSetEmoteLeft(playerLink, stats->Build.Cosmetics.Emotes.Left, default);
                f.Events.OnPlayerSetEmoteRight(playerLink, stats->Build.Cosmetics.Emotes.Right, default);
                f.Events.OnPlayerSetEmoteUp(playerLink, stats->Build.Cosmetics.Emotes.Up, default);
                f.Events.OnPlayerSetEyes(playerLink, stats->Build.Cosmetics.Eyes, default);
                f.Events.OnPlayerSetHair(playerLink, stats->Build.Cosmetics.Hair, default);
                f.Events.OnPlayerSetHeadgear(playerLink, stats->Build.Equipment.Outfit.Headgear, default);
                f.Events.OnPlayerSetLegwear(playerLink, stats->Build.Equipment.Outfit.Legwear, default);
                f.Events.OnPlayerSetMainWeapon(playerLink, stats->Build.Equipment.Weapons.MainWeapon, default);
                f.Events.OnPlayerSetSub(playerLink, stats->Build.Equipment.Weapons.SubWeapon, default);
                f.Events.OnPlayerSetUltimate(playerLink, stats->Build.Equipment.Ultimate, default);
            }

            stats->Build = default;
        }

        public static void SetAltWeapon(Frame f, EntityRef user, Stats* stats, Weapon altWeapon)
        {
            Weapon oldAltWeapon = stats->Build.Equipment.Weapons.AltWeapon;
            stats->Build.Equipment.Weapons.AltWeapon = altWeapon;

            f.Events.OnPlayerSetAltWeapon(f.Get<PlayerLink>(user), oldAltWeapon, altWeapon);
        }

        public static void SetAvatar(Frame f, EntityRef user, Stats* stats, AssetRefFFAvatar avatar)
        {
            AssetRefFFAvatar oldAvatar = stats->Build.Cosmetics.Avatar;
            stats->Build.Cosmetics.Avatar = avatar;

            f.Events.OnPlayerSetAvatar(f.Get<PlayerLink>(user), oldAvatar, avatar);
        }

        public static void SetBadge(Frame f, EntityRef user, Stats* stats, AssetRefBadge badge)
        {
            AssetRefBadge oldBadge = stats->Build.Equipment.Badge;
            stats->Build.Equipment.Badge = badge;

            ApplyBadge(f, user, badge);

            f.Events.OnPlayerSetBadge(f.Get<PlayerLink>(user), oldBadge, badge);
        }

        public static void SetClothing(Frame f, EntityRef user, Stats* stats, Apparel clothing)
        {
            Apparel oldClothing = stats->Build.Equipment.Outfit.Clothing;
            stats->Build.Equipment.Outfit.Clothing = clothing;

            f.Events.OnPlayerSetClothing(f.Get<PlayerLink>(user), oldClothing, clothing);
        }

        public static void SetEmoteDown(Frame f, EntityRef user, Stats* stats, AssetRefEmote emote)
        {
            AssetRefEmote oldEmote = stats->Build.Cosmetics.Emotes.Down;
            stats->Build.Cosmetics.Emotes.Down = emote;

            f.Events.OnPlayerSetEmoteDown(f.Get<PlayerLink>(user), oldEmote, emote);
        }

        public static void SetEmoteLeft(Frame f, EntityRef user, Stats* stats, AssetRefEmote emote)
        {
            AssetRefEmote oldEmote = stats->Build.Cosmetics.Emotes.Left;
            stats->Build.Cosmetics.Emotes.Left = emote;

            f.Events.OnPlayerSetEmoteLeft(f.Get<PlayerLink>(user), oldEmote, emote);
        }

        public static void SetEmoteRight(Frame f, EntityRef user, Stats* stats, AssetRefEmote emote)
        {
            AssetRefEmote oldEmote = stats->Build.Cosmetics.Emotes.Right;
            stats->Build.Cosmetics.Emotes.Right = emote;

            f.Events.OnPlayerSetEmoteRight(f.Get<PlayerLink>(user), oldEmote, emote);
        }

        public static void SetEmoteUp(Frame f, EntityRef user, Stats* stats, AssetRefEmote emote)
        {
            AssetRefEmote oldEmote = stats->Build.Cosmetics.Emotes.Up;
            stats->Build.Cosmetics.Emotes.Up = emote;

            f.Events.OnPlayerSetEmoteUp(f.Get<PlayerLink>(user), oldEmote, emote);
        }

        public static void SetEyes(Frame f, EntityRef user, Stats* stats, AssetRefEyes eyes)
        {
            AssetRefEyes oldEyes = stats->Build.Cosmetics.Eyes;
            stats->Build.Cosmetics.Eyes = eyes;

            f.Events.OnPlayerSetEyes(f.Get<PlayerLink>(user), oldEyes, eyes);
        }

        public static void SetHair(Frame f, EntityRef user, Stats* stats, AssetRefHair hair)
        {
            AssetRefHair oldHair = stats->Build.Cosmetics.Hair;
            stats->Build.Cosmetics.Hair = oldHair;

            f.Events.OnPlayerSetHair(f.Get<PlayerLink>(user), oldHair, hair);
        }

        public static void SetHeadgear(Frame f, EntityRef user, Stats* stats, Apparel headgear)
        {
            Apparel oldHeadgear = stats->Build.Equipment.Outfit.Headgear;
            stats->Build.Equipment.Outfit.Headgear = headgear;

            f.Events.OnPlayerSetHeadgear(f.Get<PlayerLink>(user), oldHeadgear, headgear);
        }

        public static void SetLegwear(Frame f, EntityRef user, Stats* stats, Apparel legwear)
        {
            Apparel oldOutfit = stats->Build.Equipment.Outfit.Legwear;
            stats->Build.Equipment.Outfit.Legwear = legwear;

            f.Events.OnPlayerSetLegwear(f.Get<PlayerLink>(user), oldOutfit, legwear);
        }

        public static void SetMainWeapon(Frame f, EntityRef user, Stats* stats, Weapon mainWeapon)
        {
            Weapon oldMainWeapon = stats->Build.Equipment.Weapons.MainWeapon;
            stats->Build.Equipment.Weapons.MainWeapon = mainWeapon;

            f.Events.OnPlayerSetMainWeapon(f.Get<PlayerLink>(user), oldMainWeapon, mainWeapon);
        }

        public static void SetSub(Frame f, EntityRef user, Stats* stats, Sub sub)
        {
            Sub oldSub = stats->Build.Equipment.Weapons.SubWeapon;
            stats->Build.Equipment.Weapons.SubWeapon = sub;

            f.Events.OnPlayerSetSub(f.Get<PlayerLink>(user), oldSub, sub);
        }

        public static void SetUltimate(Frame f, EntityRef user, Stats* stats, AssetRefUltimate ultimate)
        {
            AssetRefUltimate oldUltimate = stats->Build.Equipment.Ultimate;
            stats->Build.Equipment.Ultimate = ultimate;

            f.Events.OnPlayerSetUltimate(f.Get<PlayerLink>(user), oldUltimate, ultimate);
        }

        public static void ApplyBadge(Frame f, EntityRef user, AssetRefBadge badge)
        {
            f.FindAsset<Badge>(badge.Id).OnApply(f, user);
        }

        public static void UnapplyBadge(Frame f, EntityRef user, AssetRefBadge badge)
        {
            f.FindAsset<Badge>(badge.Id).OnRemove(f, user);
        }

        public static void RemoveBadge(Frame f, EntityRef user, Stats* stats)
        {
            UnapplyBadge(f, user, stats->Build.Equipment.Badge);

            AssetRefBadge oldBadge = stats->Build.Equipment.Badge;
            stats->Build.Equipment.Badge = default;

            f.Events.OnPlayerSetBadge(f.Get<PlayerLink>(user), oldBadge, default);
        }
    }
}
