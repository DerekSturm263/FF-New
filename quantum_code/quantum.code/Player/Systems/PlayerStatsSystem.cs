using Photon.Deterministic;
using System.Collections.Generic;
using System.Linq;

namespace Quantum
{
    public unsafe class PlayerStatsSystem : SystemMainThreadFilter<PlayerStatsSystem.Filter>
    {
        public struct Filter
        {
            public EntityRef Entity;
            public PlayerStats* PlayerStats;
        }

        public override void Update(Frame f, ref Filter filter)
        {
            if (f.TryFindAsset(filter.PlayerStats->Build.Gear.Badge.Id, out Badge badge))
            {
                badge.OnUpdate(f, filter.Entity);
            }

            if (f.Global->IsMatchRunning)
                filter.PlayerStats->Stats.TimeSurvived += f.DeltaTime;
        }

        public static void SetShowReadiness(Frame f, EntityRef entityRef, PlayerStats* stats, bool showReadiness)
        {
            f.Events.OnHideShowReadiness(entityRef, stats->Index, showReadiness);
        }

        public static void SetAllShowReadiness(Frame f, bool showReadiness)
        {
            foreach (var stats in f.Unsafe.GetComponentBlockIterator<PlayerStats>())
            {
                SetShowReadiness(f, stats.Entity, stats.Component, showReadiness);
            }
        }

        struct StatsFilter
        {
            public EntityRef Entity;

            public CharacterController* CharacterController;
            public PlayerStats* PlayerStats;
        }

        public static void SetReadiness(Frame f, EntityRef entityRef, CharacterController* characterController, PlayerStats* playerStats, bool isReady)
        {
            characterController->ReadyTime = 0;
            characterController->IsReady = isReady;

            f.Events.OnPlayerCancel(entityRef, playerStats->Index);
        }

        public static void SetAllReadiness(Frame f, bool isReady)
        {
            var filter = f.Unsafe.FilterStruct<StatsFilter>();
            var item = default(StatsFilter);

            while (filter.Next(&item))
            {
                SetReadiness(f, item.Entity, item.CharacterController, item.PlayerStats, isReady);
            }
        }

        public static void SetBuild(Frame f, EntityRef user, PlayerStats* stats, Build build)
        {
            stats->Build = build;

            Log.Debug("Applying build!");

            SetAvatar(f, user, stats, build.Frame.Avatar.Avatar);
            SetAvatarColor(f, user, stats, build.Frame.Avatar.Color);
            SetAltWeapon(f, user, stats, build.Gear.AltWeapon);
            SetBadge(f, user, stats, build.Gear.Badge);
            SetClothing(f, user, stats, build.Outfit.Clothing);
            SetEmoteDown(f, user, stats, build.Emotes.Down.Emote);
            SetEmoteDownMessage(f, user, stats, build.Emotes.Down.Message);
            SetEmoteLeft(f, user, stats, build.Emotes.Left.Emote);
            SetEmoteLeftMessage(f, user, stats, build.Emotes.Left.Message);
            SetEmoteRight(f, user, stats, build.Emotes.Right.Emote);
            SetEmoteRightMessage(f, user, stats, build.Emotes.Right.Message);
            SetEmoteUp(f, user, stats, build.Emotes.Up.Emote);
            SetEmoteUpMessage(f, user, stats, build.Emotes.Up.Message);
            SetEyes(f, user, stats, build.Frame.Eyes.Eyes);
            SetEyesColor(f, user, stats, build.Frame.Eyes.Color);
            SetHair(f, user, stats, build.Frame.Hair.Hair);
            SetHairColor(f, user, stats, build.Frame.Hair.Color);
            SetHeadgear(f, user, stats, build.Outfit.Headgear);
            SetLegwear(f, user, stats, build.Outfit.Legwear);
            SetMainWeapon(f, user, stats, build.Gear.MainWeapon);
            SetSub(f, user, stats, build.Gear.SubWeapon);
            SetUltimate(f, user, stats, build.Gear.Ultimate);
            SetVoice(f, user, stats, build.Frame.Voice);

            ApplyBuild(f, user, stats, build);

            f.Events.OnPlayerSetIcon(user, stats->Index);
        }

        public static void ApplyBuild(Frame f, EntityRef user, PlayerStats* stats, Build build)
        {
            ApplyBadge(f, user, stats->Build.Gear.Badge);
        }

        public static void UnapplyBuild(Frame f, EntityRef user, PlayerStats* stats)
        {
            UnapplyBadge(f, user, stats->Build.Gear.Badge);
        }

        public static void RemoveBuild(Frame f, EntityRef user, PlayerStats* stats)
        {
            Log.Debug("Removing build!");

            UnapplyBuild(f, user, stats);

            SetAvatar(f, user, stats, default);
            SetAltWeapon(f, user, stats, default);
            SetBadge(f, user, stats, default);
            SetClothing(f, user, stats, default);
            SetEmoteDown(f, user, stats, default);
            SetEmoteLeft(f, user, stats, default);
            SetEmoteRight(f, user, stats, default);
            SetEmoteUp(f, user, stats, default);
            SetEyes(f, user, stats, default);
            SetHair(f, user, stats, default);
            SetHeadgear(f, user, stats, default);
            SetLegwear(f, user, stats, default);
            SetMainWeapon(f, user, stats, default);
            SetSub(f, user, stats, default);
            SetUltimate(f, user, stats, default);
            SetVoice(f, user, stats, default);

            stats->Build = default;
        }

        public static void SetAltWeapon(Frame f, EntityRef user, PlayerStats* stats, Weapon altWeapon)
        {
            Weapon oldAltWeapon = stats->Build.Gear.AltWeapon;
            stats->Build.Gear.AltWeapon = altWeapon;

            f.Events.OnPlayerSetAltWeapon(user, oldAltWeapon, altWeapon);
        }

        public static void SetAvatar(Frame f, EntityRef user, PlayerStats* stats, AssetRefFFAvatar avatar)
        {
            AvatarColorBinding oldAvatar = stats->Build.Frame.Avatar;
            stats->Build.Frame.Avatar.Avatar = avatar;

            f.Events.OnPlayerSetAvatar(user, stats->Index, oldAvatar, stats->Build.Frame.Avatar);
        }

        public static void SetAvatarColor(Frame f, EntityRef user, PlayerStats* stats, AssetRefColorPreset color)
        {
            AvatarColorBinding oldAvatar = stats->Build.Frame.Avatar;
            stats->Build.Frame.Avatar.Color = color;

            f.Events.OnPlayerSetAvatar(user, stats->Index, oldAvatar, stats->Build.Frame.Avatar);
        }

        public static void SetVoice(Frame f, EntityRef user, PlayerStats* stats, AssetRefVoice voice)
        {
            AssetRefVoice oldVoice = stats->Build.Frame.Voice;
            stats->Build.Frame.Voice = voice;

            f.Events.OnPlayerSetVoice(user, oldVoice, voice);
        }

        public static void SetBadge(Frame f, EntityRef user, PlayerStats* stats, AssetRefBadge badge)
        {
            AssetRefBadge oldBadge = stats->Build.Gear.Badge;
            stats->Build.Gear.Badge = badge;

            ApplyBadge(f, user, badge);

            f.Events.OnPlayerSetBadge(user, oldBadge, badge);
        }

        public static void SetClothing(Frame f, EntityRef user, PlayerStats* stats, Apparel clothing)
        {
            Apparel oldClothing = stats->Build.Outfit.Clothing;
            stats->Build.Outfit.Clothing = clothing;

            f.Events.OnPlayerSetClothing(user, oldClothing, clothing);
        }

        public static void SetEmoteDown(Frame f, EntityRef user, PlayerStats* stats, AssetRefEmote emote)
        {
            EmoteMessageBinding oldEmote = stats->Build.Emotes.Down;
            stats->Build.Emotes.Down.Emote = emote;

            f.Events.OnPlayerSetEmoteDown(user, oldEmote, stats->Build.Emotes.Down);
        }

        public static void SetEmoteDownMessage(Frame f, EntityRef user, PlayerStats* stats, AssetRefMessagePreset message)
        {
            EmoteMessageBinding oldEmote = stats->Build.Emotes.Down;
            stats->Build.Emotes.Down.Message = message;

            f.Events.OnPlayerSetEmoteDown(user, oldEmote, stats->Build.Emotes.Down);
        }

        public static void SetEmoteLeft(Frame f, EntityRef user, PlayerStats* stats, AssetRefEmote emote)
        {
            EmoteMessageBinding oldEmote = stats->Build.Emotes.Left;
            stats->Build.Emotes.Left.Emote = emote;

            f.Events.OnPlayerSetEmoteLeft(user, oldEmote, stats->Build.Emotes.Left);
        }

        public static void SetEmoteLeftMessage(Frame f, EntityRef user, PlayerStats* stats, AssetRefMessagePreset message)
        {
            EmoteMessageBinding oldEmote = stats->Build.Emotes.Left;
            stats->Build.Emotes.Left.Message = message;

            f.Events.OnPlayerSetEmoteLeft(user, oldEmote, stats->Build.Emotes.Left);
        }

        public static void SetEmoteRight(Frame f, EntityRef user, PlayerStats* stats, AssetRefEmote emote)
        {
            EmoteMessageBinding oldEmote = stats->Build.Emotes.Right;
            stats->Build.Emotes.Right.Emote = emote;

            f.Events.OnPlayerSetEmoteRight(user, oldEmote, stats->Build.Emotes.Right);
        }

        public static void SetEmoteRightMessage(Frame f, EntityRef user, PlayerStats* stats, AssetRefMessagePreset message)
        {
            EmoteMessageBinding oldEmote = stats->Build.Emotes.Right;
            stats->Build.Emotes.Right.Message = message;

            f.Events.OnPlayerSetEmoteRight(user, oldEmote, stats->Build.Emotes.Right);
        }

        public static void SetEmoteUp(Frame f, EntityRef user, PlayerStats* stats, AssetRefEmote emote)
        {
            EmoteMessageBinding oldEmote = stats->Build.Emotes.Up;
            stats->Build.Emotes.Up.Emote = emote;

            f.Events.OnPlayerSetEmoteUp(user, oldEmote, stats->Build.Emotes.Up);
        }

        public static void SetEmoteUpMessage(Frame f, EntityRef user, PlayerStats* stats, AssetRefMessagePreset message)
        {
            EmoteMessageBinding oldEmote = stats->Build.Emotes.Up;
            stats->Build.Emotes.Up.Message = message;

            f.Events.OnPlayerSetEmoteUp(user, oldEmote, stats->Build.Emotes.Up);
        }

        public static void SetEyes(Frame f, EntityRef user, PlayerStats* stats, AssetRefEyes eyes)
        {
            EyesColorBinding oldEyes = stats->Build.Frame.Eyes;
            stats->Build.Frame.Eyes.Eyes = eyes;

            f.Events.OnPlayerSetEyes(user, oldEyes, stats->Build.Frame.Eyes);
        }

        public static void SetEyesColor(Frame f, EntityRef user, PlayerStats* stats, AssetRefColorPreset color)
        {
            EyesColorBinding oldEyes = stats->Build.Frame.Eyes;
            stats->Build.Frame.Eyes.Color = color;

            f.Events.OnPlayerSetEyes(user, oldEyes, stats->Build.Frame.Eyes);
        }

        public static void SetHair(Frame f, EntityRef user, PlayerStats* stats, AssetRefHair hair)
        {
            HairColorBinding oldHair = stats->Build.Frame.Hair;
            stats->Build.Frame.Hair.Hair = hair;

            f.Events.OnPlayerSetHair(user, oldHair, stats->Build.Frame.Hair);
        }

        public static void SetHairColor(Frame f, EntityRef user, PlayerStats* stats, AssetRefColorPreset color)
        {
            HairColorBinding oldHair = stats->Build.Frame.Hair;
            stats->Build.Frame.Hair.Color = color;

            f.Events.OnPlayerSetHair(user, oldHair, stats->Build.Frame.Hair);
        }

        public static void SetHeadgear(Frame f, EntityRef user, PlayerStats* stats, Apparel headgear)
        {
            Apparel oldHeadgear = stats->Build.Outfit.Headgear;
            stats->Build.Outfit.Headgear = headgear;

            f.Events.OnPlayerSetHeadgear(user, oldHeadgear, headgear);
        }

        public static void SetLegwear(Frame f, EntityRef user, PlayerStats* stats, Apparel legwear)
        {
            Apparel oldOutfit = stats->Build.Outfit.Legwear;
            stats->Build.Outfit.Legwear = legwear;

            f.Events.OnPlayerSetLegwear(user, oldOutfit, legwear);
        }

        public static void SetMainWeapon(Frame f, EntityRef user, PlayerStats* stats, Weapon mainWeapon)
        {
            Weapon oldMainWeapon = stats->Build.Gear.MainWeapon;
            stats->Build.Gear.MainWeapon = mainWeapon;

            f.Events.OnPlayerSetMainWeapon(user, oldMainWeapon, mainWeapon);
        }

        public static void SetSub(Frame f, EntityRef user, PlayerStats* stats, Sub sub)
        {
            Sub oldSub = stats->Build.Gear.SubWeapon;
            stats->Build.Gear.SubWeapon = sub;

            f.Events.OnPlayerSetSub(user, oldSub, sub);
        }

        public static void SetUltimate(Frame f, EntityRef user, PlayerStats* stats, AssetRefUltimate ultimate)
        {
            AssetRefUltimate oldUltimate = stats->Build.Gear.Ultimate;
            stats->Build.Gear.Ultimate = ultimate;

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

        public static void RemoveBadge(Frame f, EntityRef user, PlayerStats* stats)
        {
            UnapplyBadge(f, user, stats->Build.Gear.Badge);

            AssetRefBadge oldBadge = stats->Build.Gear.Badge;
            stats->Build.Gear.Badge = default;

            f.Events.OnPlayerSetBadge(user, oldBadge, default);
        }

        public static void ResetTemporaryValues(Frame f, PlayerStats* playerStats)
        {
            playerStats->Stats = default;
            playerStats->HeldItem = EntityRef.None;
            playerStats->ApparelStatsMultiplier = ApparelHelper.Default;
            playerStats->WeaponStatsMultiplier = WeaponHelper.Default;
        }

        public static void ResetAllTemporaryValues(Frame f)
        {
            foreach (var stats in f.Unsafe.GetComponentBlockIterator<PlayerStats>())
            {
                ResetTemporaryValues(f, stats.Component);
            }
        }

        public static EntityRef FindNearestOtherPlayer(Frame f, EntityRef user)
        {
            List<EntityRef> players = [];

            foreach (var stats in f.GetComponentIterator<PlayerStats>())
            {
                players.Add(stats.Entity);
            }

            players.Remove(user);

            Transform2D userTransform = f.Get<Transform2D>(user);
            players.OrderBy(item => FPVector2.DistanceSquared(userTransform.Position, f.Get<Transform2D>(item).Position));

            return players[0];
        }
    }
}