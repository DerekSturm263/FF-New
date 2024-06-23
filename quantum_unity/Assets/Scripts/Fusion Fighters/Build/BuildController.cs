using Extensions.Components.Miscellaneous;
using Quantum;
using System.Collections.Generic;
using UnityEngine;

public class BuildController : Controller<BuildController>
{
    [SerializeField] private Build _default;

    public static string GetPath() => $"{Application.persistentDataPath}/SaveData/Custom/Builds";

    private readonly Dictionary<int, EntityRef> _localIndicesToPlayers = new();
    private readonly Dictionary<int, EntityRef> _globalIndicesToPlayers = new();

    private SerializableWrapper<Build> _currentlySelected;
    public SerializableWrapper<Build> CurrentlySelected => _currentlySelected;
    public void SetCurrentlySelected(SerializableWrapper<Build> build) => _currentlySelected = build;

    public void New()
    {
        _currentlySelected = new(_default, "Untitled", "", AssetGuid.NewGuid(), System.DateTime.Now.Ticks, System.DateTime.Now.Ticks);
    }

    public void Save(SerializableWrapper<Build> build)
    {
        Serializer.Save(build, build.Guid, GetPath());
    }

    public void SaveCurrent()
    {
        Serializer.Save(_currentlySelected, _currentlySelected.Guid, GetPath());
    }

    public void SetName(string name)
    {
        _currentlySelected.SetName(name);
    }

    public void SetDescription(string description)
    {
        _currentlySelected.SetDescription(description);
    }

    public void Delete()
    {
        string path = GetPath();
        Serializer.Delete($"{path}/{_currentlySelected.Guid}.json", path);

        Destroy(BuildPopulator.ButtonFromItem(_currentlySelected));
        FindFirstObjectByType<BuildPopulator>().GetComponent<SelectAuto>().SetSelectedItem(SelectAuto.SelectType.First);
    }

    public void SetAltWeaponOnPlayer(SerializableWrapper<Weapon> weapon)
    {
        _currentlySelected.Value.Equipment.Weapons.AltWeapon = weapon;

        CommandSetAltWeapon setBuild = new()
        {
            entity = StatsSystem.GetPlayerFromLocalIndex(QuantumRunner.Default.Game.Frames.Verified, 0, 0),
            weapon = weapon
        };

        QuantumRunner.Default.Game.SendCommand(setBuild);
    }

    public void ClearAltWeaponOnPlayer()
    {
        _currentlySelected.Value.Equipment.Weapons.AltWeapon = default;

        CommandSetAltWeapon setBuild = new()
        {
            entity = StatsSystem.GetPlayerFromLocalIndex(QuantumRunner.Default.Game.Frames.Verified, 0, 0),
            weapon = default
        };

        QuantumRunner.Default.Game.SendCommand(setBuild);
    }

    public void SetAvatarOnPlayer(FFAvatarAsset avatar)
    {
        _currentlySelected.Value.Cosmetics.Avatar = new() { Id = avatar.AssetObject.Guid };

        CommandSetAvatar setBuild = new()
        {
            entity = StatsSystem.GetPlayerFromLocalIndex(QuantumRunner.Default.Game.Frames.Verified, 0, 0),
            avatar = new() { Id = avatar.AssetObject.Guid }
        };

        QuantumRunner.Default.Game.SendCommand(setBuild);
    }

    public void SetBadgeOnPlayer(BadgeAsset badge)
    {
        _currentlySelected.Value.Equipment.Badge = new() { Id = badge.AssetObject.Guid };

        CommandSetBadge setBuild = new()
        {
            entity = StatsSystem.GetPlayerFromLocalIndex(QuantumRunner.Default.Game.Frames.Verified, 0, 0),
            badge = new() { Id = badge.AssetObject.Guid }
        };

        QuantumRunner.Default.Game.SendCommand(setBuild);
    }

    public void ClearBadgeOnPlayer()
    {
        _currentlySelected.Value.Equipment.Badge = default;

        CommandSetBadge setBuild = new()
        {
            entity = StatsSystem.GetPlayerFromLocalIndex(QuantumRunner.Default.Game.Frames.Verified, 0, 0),
            badge = default
        };

        QuantumRunner.Default.Game.SendCommand(setBuild);
    }

    public void SetClothingOnPlayer(SerializableWrapper<Apparel> clothing)
    {
        _currentlySelected.Value.Equipment.Outfit.Clothing = clothing;

        CommandSetClothing setBuild = new()
        {
            entity = StatsSystem.GetPlayerFromLocalIndex(QuantumRunner.Default.Game.Frames.Verified, 0, 0),
            clothing = clothing
        };

        QuantumRunner.Default.Game.SendCommand(setBuild);
    }

    public void ClearClothingOnPlayer()
    {
        _currentlySelected.Value.Equipment.Outfit.Clothing = default;

        CommandSetClothing setBuild = new()
        {
            entity = StatsSystem.GetPlayerFromLocalIndex(QuantumRunner.Default.Game.Frames.Verified, 0, 0),
            clothing = default
        };

        QuantumRunner.Default.Game.SendCommand(setBuild);
    }

    public void SetHeadgearOnPlayer(SerializableWrapper<Apparel> headgear)
    {
        _currentlySelected.Value.Equipment.Outfit.Headgear = headgear;

        CommandSetHeadgear setBuild = new()
        {
            entity = StatsSystem.GetPlayerFromLocalIndex(QuantumRunner.Default.Game.Frames.Verified, 0, 0),
            headgear = headgear
        };

        QuantumRunner.Default.Game.SendCommand(setBuild);
    }

    public void ClearHeadgearOnPlayer()
    {
        _currentlySelected.Value.Equipment.Outfit.Headgear = default;

        CommandSetHeadgear setBuild = new()
        {
            entity = StatsSystem.GetPlayerFromLocalIndex(QuantumRunner.Default.Game.Frames.Verified, 0, 0),
            headgear = default
        };

        QuantumRunner.Default.Game.SendCommand(setBuild);
    }

    public void SetLegwearOnPlayer(SerializableWrapper<Apparel> legwear)
    {
        _currentlySelected.Value.Equipment.Outfit.Legwear = legwear;

        CommandSetLegwear setBuild = new()
        {
            entity = StatsSystem.GetPlayerFromLocalIndex(QuantumRunner.Default.Game.Frames.Verified, 0, 0),
            legwear = legwear
        };

        QuantumRunner.Default.Game.SendCommand(setBuild);
    }

    public void ClearLegwearOnPlayer()
    {
        _currentlySelected.Value.Equipment.Outfit.Legwear = default;

        CommandSetLegwear setBuild = new()
        {
            entity = StatsSystem.GetPlayerFromLocalIndex(QuantumRunner.Default.Game.Frames.Verified, 0, 0),
            legwear = default
        };

        QuantumRunner.Default.Game.SendCommand(setBuild);
    }

    public void SetEmoteUpOnPlayer(EmoteAsset emote)
    {
        _currentlySelected.Value.Cosmetics.Emotes.Up = new() { Id = emote.AssetObject.Guid };

        CommandSetEmoteUp setBuild = new()
        {
            entity = StatsSystem.GetPlayerFromLocalIndex(QuantumRunner.Default.Game.Frames.Verified, 0, 0),
            emote = new() { Id = emote.AssetObject.Guid }
        };

        QuantumRunner.Default.Game.SendCommand(setBuild);
    }

    public void ClearEmoteUpOnPlayer()
    {
        _currentlySelected.Value.Cosmetics.Emotes.Up = default;

        CommandSetEmoteUp setBuild = new()
        {
            entity = StatsSystem.GetPlayerFromLocalIndex(QuantumRunner.Default.Game.Frames.Verified, 0, 0),
            emote = default
        };

        QuantumRunner.Default.Game.SendCommand(setBuild);
    }

    public void SetEmoteDownOnPlayer(EmoteAsset emote)
    {
        _currentlySelected.Value.Cosmetics.Emotes.Down = new() { Id = emote.AssetObject.Guid };

        CommandSetEmoteDown setBuild = new()
        {
            entity = StatsSystem.GetPlayerFromLocalIndex(QuantumRunner.Default.Game.Frames.Verified, 0, 0),
            emote = new() { Id = emote.AssetObject.Guid }
        };

        QuantumRunner.Default.Game.SendCommand(setBuild);
    }

    public void ClearEmoteDownOnPlayer()
    {
        _currentlySelected.Value.Cosmetics.Emotes.Down = default;

        CommandSetEmoteDown setBuild = new()
        {
            entity = StatsSystem.GetPlayerFromLocalIndex(QuantumRunner.Default.Game.Frames.Verified, 0, 0),
            emote = default
        };

        QuantumRunner.Default.Game.SendCommand(setBuild);
    }

    public void SetEmoteLeftOnPlayer(EmoteAsset emote)
    {
        _currentlySelected.Value.Cosmetics.Emotes.Left = new() { Id = emote.AssetObject.Guid };

        CommandSetEmoteLeft setBuild = new()
        {
            entity = StatsSystem.GetPlayerFromLocalIndex(QuantumRunner.Default.Game.Frames.Verified, 0, 0),
            emote = new() { Id = emote.AssetObject.Guid }
        };

        QuantumRunner.Default.Game.SendCommand(setBuild);
    }

    public void ClearEmoteLeftOnPlayer()
    {
        _currentlySelected.Value.Cosmetics.Emotes.Left = default;

        CommandSetEmoteLeft setBuild = new()
        {
            entity = StatsSystem.GetPlayerFromLocalIndex(QuantumRunner.Default.Game.Frames.Verified, 0, 0),
            emote = default
        };

        QuantumRunner.Default.Game.SendCommand(setBuild);
    }

    public void SetEmoteRightOnPlayer(EmoteAsset emote)
    {
        _currentlySelected.Value.Cosmetics.Emotes.Right = new() { Id = emote.AssetObject.Guid };

        CommandSetEmoteRight setBuild = new()
        {
            entity = StatsSystem.GetPlayerFromLocalIndex(QuantumRunner.Default.Game.Frames.Verified, 0, 0),
            emote = new() { Id = emote.AssetObject.Guid }
        };

        QuantumRunner.Default.Game.SendCommand(setBuild);
    }

    public void ClearEmoteRightOnPlayer()
    {
        _currentlySelected.Value.Cosmetics.Emotes.Right = default;

        CommandSetEmoteRight setBuild = new()
        {
            entity = StatsSystem.GetPlayerFromLocalIndex(QuantumRunner.Default.Game.Frames.Verified, 0, 0),
            emote = default
        };

        QuantumRunner.Default.Game.SendCommand(setBuild);
    }

    public void SetEyesOnPlayer(EyesAsset eyes)
    {
        _currentlySelected.Value.Cosmetics.Eyes = new() { Id = eyes.AssetObject.Guid };

        CommandSetEyes setBuild = new()
        {
            entity = StatsSystem.GetPlayerFromLocalIndex(QuantumRunner.Default.Game.Frames.Verified, 0, 0),
            eyes = new() { Id = eyes.AssetObject.Guid }
        };

        QuantumRunner.Default.Game.SendCommand(setBuild);
    }

    public void SetHairOnPlayer(HairAsset hair)
    {
        _currentlySelected.Value.Cosmetics.Hair = new() { Id = hair.AssetObject.Guid };

        CommandSetHair setBuild = new()
        {
            entity = StatsSystem.GetPlayerFromLocalIndex(QuantumRunner.Default.Game.Frames.Verified, 0, 0),
            hair = new() { Id = hair.AssetObject.Guid }
        };

        QuantumRunner.Default.Game.SendCommand(setBuild);
    }

    public void SetMainWeaponOnPlayer(SerializableWrapper<Weapon> weapon)
    {
        _currentlySelected.Value.Equipment.Weapons.MainWeapon = weapon;

        CommandSetMainWeapon setBuild = new()
        {
            entity = StatsSystem.GetPlayerFromLocalIndex(QuantumRunner.Default.Game.Frames.Verified, 0, 0),
            weapon = weapon
        };

        QuantumRunner.Default.Game.SendCommand(setBuild);
    }

    public void ClearMainWeaponOnPlayer()
    {
        _currentlySelected.Value.Equipment.Weapons.MainWeapon = default;

        CommandSetMainWeapon setBuild = new()
        {
            entity = StatsSystem.GetPlayerFromLocalIndex(QuantumRunner.Default.Game.Frames.Verified, 0, 0),
            weapon = default
        };

        QuantumRunner.Default.Game.SendCommand(setBuild);
    }

    public void SetSubOnPlayer(SerializableWrapper<Sub> sub)
    {
        _currentlySelected.Value.Equipment.Weapons.SubWeapon = sub;

        CommandSetSub setBuild = new()
        {
            entity = StatsSystem.GetPlayerFromLocalIndex(QuantumRunner.Default.Game.Frames.Verified, 0, 0),
            sub = sub
        };

        QuantumRunner.Default.Game.SendCommand(setBuild);
    }

    public void ClearSubOnPlayer()
    {
        _currentlySelected.Value.Equipment.Weapons.SubWeapon = default;

        CommandSetSub setBuild = new()
        {
            entity = StatsSystem.GetPlayerFromLocalIndex(QuantumRunner.Default.Game.Frames.Verified, 0, 0),
            sub = default
        };

        QuantumRunner.Default.Game.SendCommand(setBuild);
    }

    public void SetUltimateOnPlayer(UltimateAsset ultimate)
    {
        _currentlySelected.Value.Equipment.Ultimate = new() { Id = ultimate.AssetObject.Guid };

        CommandSetUltimate setBuild = new()
        {
            entity = StatsSystem.GetPlayerFromLocalIndex(QuantumRunner.Default.Game.Frames.Verified, 0, 0),
            ultimate = new() { Id = ultimate.AssetObject.Guid }
        };

        QuantumRunner.Default.Game.SendCommand(setBuild);
    }

    public void ClearUltimateOnPlayer()
    {
        _currentlySelected.Value.Equipment.Ultimate = default;

        CommandSetUltimate setBuild = new()
        {
            entity = StatsSystem.GetPlayerFromLocalIndex(QuantumRunner.Default.Game.Frames.Verified, 0, 0),
            ultimate = default
        };

        QuantumRunner.Default.Game.SendCommand(setBuild);
    }

    public void SetVoiceOnPlayer(VoiceAsset voice)
    {
        _currentlySelected.Value.Cosmetics.Voice = new() { Id = voice.AssetObject.Guid };

        CommandSetVoice setBuild = new()
        {
            entity = StatsSystem.GetPlayerFromLocalIndex(QuantumRunner.Default.Game.Frames.Verified, 0, 0),
            voice = new() { Id = voice.AssetObject.Guid }
        };

        QuantumRunner.Default.Game.SendCommand(setBuild);
    }

    public void SetOnPlayer(SerializableWrapper<Build> build, int playerIndex)
    {
        CommandSetBuild setBuild = new()
        {
            entity = StatsSystem.GetPlayerFromLocalIndex(QuantumRunner.Default.Game.Frames.Verified, playerIndex, playerIndex),
            build = build
        };

        QuantumRunner.Default.Game.SendCommand(setBuild);
        PlayerStatController.Instance.HUDS[playerIndex].SetPlayerIcon(build.Icon);
    }

    public void SetOnPlayerDefault(SerializableWrapper<Build> build)
    {
        SetOnPlayer(build, 0);
    }

    public void SetOnPlayerDefault()
    {
        SetOnPlayer(_currentlySelected, 0);
    }
}
