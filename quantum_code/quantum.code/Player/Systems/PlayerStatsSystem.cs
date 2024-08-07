using Photon.Deterministic;
using Quantum.Custom.Animator;
using System.Collections.Generic;
using System.Linq;

namespace Quantum
{
    public unsafe class PlayerStatsSystem : SystemMainThreadFilter<PlayerStatsSystem.Filter>, ISignalOnComponentRemoved<PlayerStats>
    {
        public struct Filter
        {
            public EntityRef Entity;
            public PlayerStats* PlayerStats;
        }

        public override void Update(Frame f, ref Filter filter)
        {
            if (f.Unsafe.TryGetPointer(filter.Entity, out CustomAnimator* customAnimator) && f.Unsafe.TryGetPointer(filter.Entity, out CharacterController* characterController))
            {
                UpdateMainWeapon(f, ref filter, customAnimator, characterController);
                UpdateAltWeapon(f, ref filter, customAnimator, characterController);
            }

            if (f.TryFindAsset(filter.PlayerStats->Build.Equipment.Badge.Id, out Badge badge))
            {
                badge.OnUpdate(f, filter.Entity);
            }
        }

        public void OnRemoved(Frame f, EntityRef entity, PlayerStats* component)
        {
            RemoveBuild(f, entity, component);
        }

        public void UpdateMainWeapon(Frame f, ref Filter filter, CustomAnimator* customAnimator, CharacterController* characterController)
        {
            if (f.Unsafe.TryGetPointer(filter.PlayerStats->MainWeapon, out ChildParentLink* childParentLink))
            {
                HurtboxTransformInfo transform = CustomAnimator.GetFrame(f, customAnimator).hurtboxPositions[15];

                childParentLink->LocalPosition = transform.position;
                childParentLink->LocalRotation = transform.rotation;

                if (characterController->MovementDirection < 0)
                    childParentLink->LocalPosition.X *= -1;
            }
        }

        public void UpdateAltWeapon(Frame f, ref Filter filter, CustomAnimator* customAnimator, CharacterController* characterController)
        {
            if (f.Unsafe.TryGetPointer(filter.PlayerStats->AltWeapon, out ChildParentLink* childParentLink))
            {
                HurtboxTransformInfo transform = CustomAnimator.GetFrame(f, customAnimator).hurtboxPositions[16];

                childParentLink->LocalPosition = transform.position;
                childParentLink->LocalRotation = transform.rotation;

                if (characterController->MovementDirection < 0)
                    childParentLink->LocalPosition.X *= -1;
            }
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

            SetAvatar(f, user, stats, build.Cosmetics.Avatar.Avatar);
            SetAvatarColor(f, user, stats, build.Cosmetics.Avatar.Color);
            SetAltWeapon(f, user, stats, build.Equipment.Weapons.AltWeapon);
            SetBadge(f, user, stats, build.Equipment.Badge);
            SetClothing(f, user, stats, build.Equipment.Outfit.Clothing);
            SetEmoteDown(f, user, stats, build.Cosmetics.Emotes.Down.Emote);
            SetEmoteDownMessage(f, user, stats, build.Cosmetics.Emotes.Down.Message);
            SetEmoteLeft(f, user, stats, build.Cosmetics.Emotes.Left.Emote);
            SetEmoteLeftMessage(f, user, stats, build.Cosmetics.Emotes.Left.Message);
            SetEmoteRight(f, user, stats, build.Cosmetics.Emotes.Right.Emote);
            SetEmoteRightMessage(f, user, stats, build.Cosmetics.Emotes.Right.Message);
            SetEmoteUp(f, user, stats, build.Cosmetics.Emotes.Up.Emote);
            SetEmoteUpMessage(f, user, stats, build.Cosmetics.Emotes.Up.Message);
            SetEyes(f, user, stats, build.Cosmetics.Eyes.Eyes);
            SetEyesColor(f, user, stats, build.Cosmetics.Eyes.Color);
            SetHair(f, user, stats, build.Cosmetics.Hair.Hair);
            SetHairColor(f, user, stats, build.Cosmetics.Hair.Color);
            SetHeadgear(f, user, stats, build.Equipment.Outfit.Headgear);
            SetLegwear(f, user, stats, build.Equipment.Outfit.Legwear);
            SetMainWeapon(f, user, stats, build.Equipment.Weapons.MainWeapon);
            SetSub(f, user, stats, build.Equipment.Weapons.SubWeapon);
            SetUltimate(f, user, stats, build.Equipment.Ultimate);
            SetVoice(f, user, stats, build.Cosmetics.Voice);

            ApplyBuild(f, user, stats, build);
        }

        public static void ApplyBuild(Frame f, EntityRef user, PlayerStats* stats, Build build)
        {
            ApplyBadge(f, user, stats->Build.Equipment.Badge);
        }

        public static void UnapplyBuild(Frame f, EntityRef user, PlayerStats* stats)
        {
            UnapplyBadge(f, user, stats->Build.Equipment.Badge);
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
            if (stats->AltWeapon.IsValid)
                f.Destroy(stats->AltWeapon);

            Weapon oldAltWeapon = stats->Build.Equipment.Weapons.AltWeapon;
            stats->Build.Equipment.Weapons.AltWeapon = altWeapon;

            if (altWeapon.Template.Id.IsValid)
            {
                if (f.Unsafe.TryGetPointer(user, out Stats* genericStats))
                {
                    WeaponTemplate entity = f.FindAsset<WeaponTemplate>(altWeapon.Template.Id);
                    stats->AltWeapon = f.CreateChilded(entity.Weapon, user);
                }
            }

            f.Events.OnPlayerSetAltWeapon(user, oldAltWeapon, altWeapon);
        }

        public static void SetAvatar(Frame f, EntityRef user, PlayerStats* stats, AssetRefFFAvatar avatar)
        {
            AvatarColorBinding oldAvatar = stats->Build.Cosmetics.Avatar;
            stats->Build.Cosmetics.Avatar.Avatar = avatar;

            f.Events.OnPlayerSetAvatar(user, oldAvatar, stats->Build.Cosmetics.Avatar);
        }

        public static void SetAvatarColor(Frame f, EntityRef user, PlayerStats* stats, AssetRefColorPreset color)
        {
            AvatarColorBinding oldAvatar = stats->Build.Cosmetics.Avatar;
            stats->Build.Cosmetics.Avatar.Color = color;

            f.Events.OnPlayerSetAvatar(user, oldAvatar, stats->Build.Cosmetics.Avatar);
        }

        public static void SetVoice(Frame f, EntityRef user, PlayerStats* stats, AssetRefVoice voice)
        {
            AssetRefVoice oldVoice = stats->Build.Cosmetics.Voice;
            stats->Build.Cosmetics.Voice = voice;

            f.Events.OnPlayerSetVoice(user, oldVoice, voice);
        }

        public static void SetBadge(Frame f, EntityRef user, PlayerStats* stats, AssetRefBadge badge)
        {
            AssetRefBadge oldBadge = stats->Build.Equipment.Badge;
            stats->Build.Equipment.Badge = badge;

            ApplyBadge(f, user, badge);

            f.Events.OnPlayerSetBadge(user, oldBadge, badge);
        }

        public static void SetClothing(Frame f, EntityRef user, PlayerStats* stats, Apparel clothing)
        {
            Apparel oldClothing = stats->Build.Equipment.Outfit.Clothing;
            stats->Build.Equipment.Outfit.Clothing = clothing;

            f.Events.OnPlayerSetClothing(user, oldClothing, clothing);
        }

        public static void SetEmoteDown(Frame f, EntityRef user, PlayerStats* stats, AssetRefEmote emote)
        {
            EmoteMessageBinding oldEmote = stats->Build.Cosmetics.Emotes.Down;
            stats->Build.Cosmetics.Emotes.Down.Emote = emote;

            f.Events.OnPlayerSetEmoteDown(user, oldEmote, stats->Build.Cosmetics.Emotes.Down);
        }

        public static void SetEmoteDownMessage(Frame f, EntityRef user, PlayerStats* stats, AssetRefMessagePreset message)
        {
            EmoteMessageBinding oldEmote = stats->Build.Cosmetics.Emotes.Down;
            stats->Build.Cosmetics.Emotes.Down.Message = message;

            f.Events.OnPlayerSetEmoteDown(user, oldEmote, stats->Build.Cosmetics.Emotes.Down);
        }

        public static void SetEmoteLeft(Frame f, EntityRef user, PlayerStats* stats, AssetRefEmote emote)
        {
            EmoteMessageBinding oldEmote = stats->Build.Cosmetics.Emotes.Left;
            stats->Build.Cosmetics.Emotes.Left.Emote = emote;

            f.Events.OnPlayerSetEmoteLeft(user, oldEmote, stats->Build.Cosmetics.Emotes.Left);
        }

        public static void SetEmoteLeftMessage(Frame f, EntityRef user, PlayerStats* stats, AssetRefMessagePreset message)
        {
            EmoteMessageBinding oldEmote = stats->Build.Cosmetics.Emotes.Left;
            stats->Build.Cosmetics.Emotes.Left.Message = message;

            f.Events.OnPlayerSetEmoteLeft(user, oldEmote, stats->Build.Cosmetics.Emotes.Left);
        }

        public static void SetEmoteRight(Frame f, EntityRef user, PlayerStats* stats, AssetRefEmote emote)
        {
            EmoteMessageBinding oldEmote = stats->Build.Cosmetics.Emotes.Right;
            stats->Build.Cosmetics.Emotes.Right.Emote = emote;

            f.Events.OnPlayerSetEmoteRight(user, oldEmote, stats->Build.Cosmetics.Emotes.Right);
        }

        public static void SetEmoteRightMessage(Frame f, EntityRef user, PlayerStats* stats, AssetRefMessagePreset message)
        {
            EmoteMessageBinding oldEmote = stats->Build.Cosmetics.Emotes.Right;
            stats->Build.Cosmetics.Emotes.Right.Message = message;

            f.Events.OnPlayerSetEmoteRight(user, oldEmote, stats->Build.Cosmetics.Emotes.Right);
        }

        public static void SetEmoteUp(Frame f, EntityRef user, PlayerStats* stats, AssetRefEmote emote)
        {
            EmoteMessageBinding oldEmote = stats->Build.Cosmetics.Emotes.Up;
            stats->Build.Cosmetics.Emotes.Up.Emote = emote;

            f.Events.OnPlayerSetEmoteUp(user, oldEmote, stats->Build.Cosmetics.Emotes.Up);
        }

        public static void SetEmoteUpMessage(Frame f, EntityRef user, PlayerStats* stats, AssetRefMessagePreset message)
        {
            EmoteMessageBinding oldEmote = stats->Build.Cosmetics.Emotes.Up;
            stats->Build.Cosmetics.Emotes.Up.Message = message;

            f.Events.OnPlayerSetEmoteUp(user, oldEmote, stats->Build.Cosmetics.Emotes.Up);
        }

        public static void SetEyes(Frame f, EntityRef user, PlayerStats* stats, AssetRefEyes eyes)
        {
            EyesColorBinding oldEyes = stats->Build.Cosmetics.Eyes;
            stats->Build.Cosmetics.Eyes.Eyes = eyes;

            f.Events.OnPlayerSetEyes(user, oldEyes, stats->Build.Cosmetics.Eyes);
        }

        public static void SetEyesColor(Frame f, EntityRef user, PlayerStats* stats, AssetRefColorPreset color)
        {
            EyesColorBinding oldEyes = stats->Build.Cosmetics.Eyes;
            stats->Build.Cosmetics.Eyes.Color = color;

            f.Events.OnPlayerSetEyes(user, oldEyes, stats->Build.Cosmetics.Eyes);
        }

        public static void SetHair(Frame f, EntityRef user, PlayerStats* stats, AssetRefHair hair)
        {
            HairColorBinding oldHair = stats->Build.Cosmetics.Hair;
            stats->Build.Cosmetics.Hair.Hair = hair;

            f.Events.OnPlayerSetHair(user, oldHair, stats->Build.Cosmetics.Hair);
        }

        public static void SetHairColor(Frame f, EntityRef user, PlayerStats* stats, AssetRefColorPreset color)
        {
            HairColorBinding oldHair = stats->Build.Cosmetics.Hair;
            stats->Build.Cosmetics.Hair.Color = color;

            f.Events.OnPlayerSetHair(user, oldHair, stats->Build.Cosmetics.Hair);
        }

        public static void SetHeadgear(Frame f, EntityRef user, PlayerStats* stats, Apparel headgear)
        {
            Apparel oldHeadgear = stats->Build.Equipment.Outfit.Headgear;
            stats->Build.Equipment.Outfit.Headgear = headgear;

            f.Events.OnPlayerSetHeadgear(user, oldHeadgear, headgear);
        }

        public static void SetLegwear(Frame f, EntityRef user, PlayerStats* stats, Apparel legwear)
        {
            Apparel oldOutfit = stats->Build.Equipment.Outfit.Legwear;
            stats->Build.Equipment.Outfit.Legwear = legwear;

            f.Events.OnPlayerSetLegwear(user, oldOutfit, legwear);
        }

        public static void SetMainWeapon(Frame f, EntityRef user, PlayerStats* stats, Weapon mainWeapon)
        {
            if (stats->MainWeapon.IsValid)
                f.Destroy(stats->MainWeapon);

            Weapon oldMainWeapon = stats->Build.Equipment.Weapons.MainWeapon;
            stats->Build.Equipment.Weapons.MainWeapon = mainWeapon;

            if (mainWeapon.Template.Id.IsValid)
            {
                if (f.Unsafe.TryGetPointer(user, out Stats* genericStats))
                {
                    WeaponTemplate entity = f.FindAsset<WeaponTemplate>(mainWeapon.Template.Id);
                    stats->MainWeapon = f.CreateChilded(entity.Weapon, user);
                }
            }

            f.Events.OnPlayerSetMainWeapon(user, oldMainWeapon, mainWeapon);
        }

        public static void SetSub(Frame f, EntityRef user, PlayerStats* stats, Sub sub)
        {
            Sub oldSub = stats->Build.Equipment.Weapons.SubWeapon;
            stats->Build.Equipment.Weapons.SubWeapon = sub;

            f.Events.OnPlayerSetSub(user, oldSub, sub);
        }

        public static void SetUltimate(Frame f, EntityRef user, PlayerStats* stats, AssetRefUltimate ultimate)
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

        public static void RemoveBadge(Frame f, EntityRef user, PlayerStats* stats)
        {
            UnapplyBadge(f, user, stats->Build.Equipment.Badge);

            AssetRefBadge oldBadge = stats->Build.Equipment.Badge;
            stats->Build.Equipment.Badge = default;

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