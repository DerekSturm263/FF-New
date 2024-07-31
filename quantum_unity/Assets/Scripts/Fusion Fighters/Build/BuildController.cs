using Extensions.Components.Miscellaneous;
using Quantum;
using UnityEngine;

public class BuildController : Controller<BuildController>
{
    [SerializeField] private Build _default;

    public static string GetPath() => $"{Application.persistentDataPath}/SaveData/Custom/Builds";

    private SerializableWrapper<Build> _currentlySelected;
    public SerializableWrapper<Build> CurrentlySelected => _currentlySelected;
    public void SetCurrentlySelected(SerializableWrapper<Build> build) => _currentlySelected = build;

    public void New()
    {
        _currentlySelected = new(_default, "Untitled", "", System.DateTime.Now.Ticks, System.DateTime.Now.Ticks, AssetGuid.NewGuid());
    }

    public void Save(SerializableWrapper<Build> build)
    {
        build.Save(GetPath());
    }

    public void SaveCurrent()
    {
        _currentlySelected.Save(GetPath());
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
        _currentlySelected.Delete(GetPath());

        Destroy(BuildPopulator.ButtonFromItem(_currentlySelected));
        FindFirstObjectByType<BuildPopulator>().GetComponent<SelectAuto>().SetSelectedItem(SelectAuto.SelectType.First);
    }

    public unsafe void SetAltWeaponOnPlayer(SerializableWrapper<Weapon> weapon)
    {
        _currentlySelected.value.Equipment.Weapons.AltWeapon = weapon;

        CommandSetAltWeapon setBuild = new()
        {
            entity = FighterIndex.GetFirstPlayer(QuantumRunner.Default.Game.Frames.Verified, index => index.GlobalNoBots == 0),
            weapon = weapon
        };

        QuantumRunner.Default.Game.SendCommand(setBuild);
    }

    public unsafe void ClearAltWeaponOnPlayer()
    {
        _currentlySelected.value.Equipment.Weapons.AltWeapon = default;

        CommandSetAltWeapon setBuild = new()
        {
            entity = FighterIndex.GetFirstPlayer(QuantumRunner.Default.Game.Frames.Verified, index => index.GlobalNoBots == 0),
            weapon = default
        };

        QuantumRunner.Default.Game.SendCommand(setBuild);
    }

    public unsafe void SetAvatarOnPlayer(FFAvatarAsset avatar)
    {
        _currentlySelected.value.Cosmetics.Avatar = new() { Id = avatar.AssetObject.Guid };

        CommandSetAvatar setBuild = new()
        {
            entity = FighterIndex.GetFirstPlayer(QuantumRunner.Default.Game.Frames.Verified, index => index.GlobalNoBots == 0),
            avatar = new() { Id = avatar.AssetObject.Guid }
        };

        QuantumRunner.Default.Game.SendCommand(setBuild);
    }

    public unsafe void SetBadgeOnPlayer(BadgeAsset badge)
    {
        _currentlySelected.value.Equipment.Badge = new() { Id = badge.AssetObject.Guid };

        CommandSetBadge setBuild = new()
        {
            entity = FighterIndex.GetFirstPlayer(QuantumRunner.Default.Game.Frames.Verified, index => index.GlobalNoBots == 0),
            badge = new() { Id = badge.AssetObject.Guid }
        };

        QuantumRunner.Default.Game.SendCommand(setBuild);
    }

    public unsafe void ClearBadgeOnPlayer()
    {
        _currentlySelected.value.Equipment.Badge = default;

        CommandSetBadge setBuild = new()
        {
            entity = FighterIndex.GetFirstPlayer(QuantumRunner.Default.Game.Frames.Verified, index => index.GlobalNoBots == 0),
            badge = default
        };

        QuantumRunner.Default.Game.SendCommand(setBuild);
    }

    public unsafe void SetClothingOnPlayer(SerializableWrapper<Apparel> clothing)
    {
        _currentlySelected.value.Equipment.Outfit.Clothing = clothing;

        CommandSetClothing setBuild = new()
        {
            entity = FighterIndex.GetFirstPlayer(QuantumRunner.Default.Game.Frames.Verified, index => index.GlobalNoBots == 0),
            clothing = clothing
        };

        QuantumRunner.Default.Game.SendCommand(setBuild);
    }

    public unsafe void ClearClothingOnPlayer()
    {
        _currentlySelected.value.Equipment.Outfit.Clothing = default;

        CommandSetClothing setBuild = new()
        {
            entity = FighterIndex.GetFirstPlayer(QuantumRunner.Default.Game.Frames.Verified, index => index.GlobalNoBots == 0),
            clothing = default
        };

        QuantumRunner.Default.Game.SendCommand(setBuild);
    }

    public unsafe void SetHeadgearOnPlayer(SerializableWrapper<Apparel> headgear)
    {
        _currentlySelected.value.Equipment.Outfit.Headgear = headgear;

        CommandSetHeadgear setBuild = new()
        {
            entity = FighterIndex.GetFirstPlayer(QuantumRunner.Default.Game.Frames.Verified, index => index.GlobalNoBots == 0),
            headgear = headgear
        };

        QuantumRunner.Default.Game.SendCommand(setBuild);
    }

    public unsafe void ClearHeadgearOnPlayer()
    {
        _currentlySelected.value.Equipment.Outfit.Headgear = default;

        CommandSetHeadgear setBuild = new()
        {
            entity = FighterIndex.GetFirstPlayer(QuantumRunner.Default.Game.Frames.Verified, index => index.GlobalNoBots == 0),
            headgear = default
        };

        QuantumRunner.Default.Game.SendCommand(setBuild);
    }

    public unsafe void SetLegwearOnPlayer(SerializableWrapper<Apparel> legwear)
    {
        _currentlySelected.value.Equipment.Outfit.Legwear = legwear;

        CommandSetLegwear setBuild = new()
        {
            entity = FighterIndex.GetFirstPlayer(QuantumRunner.Default.Game.Frames.Verified, index => index.GlobalNoBots == 0),
            legwear = legwear
        };

        QuantumRunner.Default.Game.SendCommand(setBuild);
    }

    public unsafe void ClearLegwearOnPlayer()
    {
        _currentlySelected.value.Equipment.Outfit.Legwear = default;

        CommandSetLegwear setBuild = new()
        {
            entity = FighterIndex.GetFirstPlayer(QuantumRunner.Default.Game.Frames.Verified, index => index.GlobalNoBots == 0),
            legwear = default
        };

        QuantumRunner.Default.Game.SendCommand(setBuild);
    }

    public unsafe void SetEmoteUpOnPlayer(EmoteAsset emote)
    {
        _currentlySelected.value.Cosmetics.Emotes.Up = new() { Id = emote.AssetObject.Guid };

        CommandSetEmoteUp setBuild = new()
        {
            entity = FighterIndex.GetFirstPlayer(QuantumRunner.Default.Game.Frames.Verified, index => index.GlobalNoBots == 0),
            emote = new() { Id = emote.AssetObject.Guid }
        };

        QuantumRunner.Default.Game.SendCommand(setBuild);
    }

    public unsafe void ClearEmoteUpOnPlayer()
    {
        _currentlySelected.value.Cosmetics.Emotes.Up = default;

        CommandSetEmoteUp setBuild = new()
        {
            entity = FighterIndex.GetFirstPlayer(QuantumRunner.Default.Game.Frames.Verified, index => index.GlobalNoBots == 0),
            emote = default
        };

        QuantumRunner.Default.Game.SendCommand(setBuild);
    }

    public unsafe void SetEmoteDownOnPlayer(EmoteAsset emote)
    {
        _currentlySelected.value.Cosmetics.Emotes.Down = new() { Id = emote.AssetObject.Guid };

        CommandSetEmoteDown setBuild = new()
        {
            entity = FighterIndex.GetFirstPlayer(QuantumRunner.Default.Game.Frames.Verified, index => index.GlobalNoBots == 0),
            emote = new() { Id = emote.AssetObject.Guid }
        };

        QuantumRunner.Default.Game.SendCommand(setBuild);
    }

    public unsafe void ClearEmoteDownOnPlayer()
    {
        _currentlySelected.value.Cosmetics.Emotes.Down = default;

        CommandSetEmoteDown setBuild = new()
        {
            entity = FighterIndex.GetFirstPlayer(QuantumRunner.Default.Game.Frames.Verified, index => index.GlobalNoBots == 0),
            emote = default
        };

        QuantumRunner.Default.Game.SendCommand(setBuild);
    }

    public unsafe void SetEmoteLeftOnPlayer(EmoteAsset emote)
    {
        _currentlySelected.value.Cosmetics.Emotes.Left = new() { Id = emote.AssetObject.Guid };

        CommandSetEmoteLeft setBuild = new()
        {
            entity = FighterIndex.GetFirstPlayer(QuantumRunner.Default.Game.Frames.Verified, index => index.GlobalNoBots == 0),
            emote = new() { Id = emote.AssetObject.Guid }
        };

        QuantumRunner.Default.Game.SendCommand(setBuild);
    }

    public unsafe void ClearEmoteLeftOnPlayer()
    {
        _currentlySelected.value.Cosmetics.Emotes.Left = default;

        CommandSetEmoteLeft setBuild = new()
        {
            entity = FighterIndex.GetFirstPlayer(QuantumRunner.Default.Game.Frames.Verified, index => index.GlobalNoBots == 0),
            emote = default
        };

        QuantumRunner.Default.Game.SendCommand(setBuild);
    }

    public unsafe void SetEmoteRightOnPlayer(EmoteAsset emote)
    {
        _currentlySelected.value.Cosmetics.Emotes.Right = new() { Id = emote.AssetObject.Guid };

        CommandSetEmoteRight setBuild = new()
        {
            entity = FighterIndex.GetFirstPlayer(QuantumRunner.Default.Game.Frames.Verified, index => index.GlobalNoBots == 0),
            emote = new() { Id = emote.AssetObject.Guid }
        };

        QuantumRunner.Default.Game.SendCommand(setBuild);
    }

    public unsafe void ClearEmoteRightOnPlayer()
    {
        _currentlySelected.value.Cosmetics.Emotes.Right = default;

        CommandSetEmoteRight setBuild = new()
        {
            entity = FighterIndex.GetFirstPlayer(QuantumRunner.Default.Game.Frames.Verified, index => index.GlobalNoBots == 0),
            emote = default
        };

        QuantumRunner.Default.Game.SendCommand(setBuild);
    }

    public unsafe void SetEyesOnPlayer(EyesAsset eyes)
    {
        _currentlySelected.value.Cosmetics.Eyes = new() { Id = eyes.AssetObject.Guid };

        CommandSetEyes setBuild = new()
        {
            entity = FighterIndex.GetFirstPlayer(QuantumRunner.Default.Game.Frames.Verified, index => index.GlobalNoBots == 0),
            eyes = new() { Id = eyes.AssetObject.Guid }
        };

        QuantumRunner.Default.Game.SendCommand(setBuild);
    }

    public unsafe void SetHairOnPlayer(HairAsset hair)
    {
        _currentlySelected.value.Cosmetics.Hair = new() { Id = hair.AssetObject.Guid };

        CommandSetHair setBuild = new()
        {
            entity = FighterIndex.GetFirstPlayer(QuantumRunner.Default.Game.Frames.Verified, index => index.GlobalNoBots == 0),
            hair = new() { Id = hair.AssetObject.Guid }
        };

        QuantumRunner.Default.Game.SendCommand(setBuild);
    }

    public unsafe void SetMainWeaponOnPlayer(SerializableWrapper<Weapon> weapon)
    {
        _currentlySelected.value.Equipment.Weapons.MainWeapon = weapon;

        CommandSetMainWeapon setBuild = new()
        {
            entity = FighterIndex.GetFirstPlayer(QuantumRunner.Default.Game.Frames.Verified, index => index.GlobalNoBots == 0),
            weapon = weapon
        };

        QuantumRunner.Default.Game.SendCommand(setBuild);
    }

    public unsafe void ClearMainWeaponOnPlayer()
    {
        _currentlySelected.value.Equipment.Weapons.MainWeapon = default;

        CommandSetMainWeapon setBuild = new()
        {
            entity = FighterIndex.GetFirstPlayer(QuantumRunner.Default.Game.Frames.Verified, index => index.GlobalNoBots == 0),
            weapon = default
        };

        QuantumRunner.Default.Game.SendCommand(setBuild);
    }

    public unsafe void SetSubOnPlayer(SerializableWrapper<Sub> sub)
    {
        _currentlySelected.value.Equipment.Weapons.SubWeapon = sub;

        CommandSetSub setBuild = new()
        {
            entity = FighterIndex.GetFirstPlayer(QuantumRunner.Default.Game.Frames.Verified, index => index.GlobalNoBots == 0),
            sub = sub
        };

        QuantumRunner.Default.Game.SendCommand(setBuild);
    }

    public unsafe void ClearSubOnPlayer()
    {
        _currentlySelected.value.Equipment.Weapons.SubWeapon = default;

        CommandSetSub setBuild = new()
        {
            entity = FighterIndex.GetFirstPlayer(QuantumRunner.Default.Game.Frames.Verified, index => index.GlobalNoBots == 0),
            sub = default
        };

        QuantumRunner.Default.Game.SendCommand(setBuild);
    }

    public unsafe void SetUltimateOnPlayer(UltimateAsset ultimate)
    {
        _currentlySelected.value.Equipment.Ultimate = new() { Id = ultimate.AssetObject.Guid };

        CommandSetUltimate setBuild = new()
        {
            entity = FighterIndex.GetFirstPlayer(QuantumRunner.Default.Game.Frames.Verified, index => index.GlobalNoBots == 0),
            ultimate = new() { Id = ultimate.AssetObject.Guid }
        };

        QuantumRunner.Default.Game.SendCommand(setBuild);
    }

    public unsafe void ClearUltimateOnPlayer()
    {
        _currentlySelected.value.Equipment.Ultimate = default;

        CommandSetUltimate setBuild = new()
        {
            entity = FighterIndex.GetFirstPlayer(QuantumRunner.Default.Game.Frames.Verified, index => index.GlobalNoBots == 0),
            ultimate = default
        };

        QuantumRunner.Default.Game.SendCommand(setBuild);
    }

    public unsafe void SetVoiceOnPlayer(VoiceAsset voice)
    {
        _currentlySelected.value.Cosmetics.Voice = new() { Id = voice.AssetObject.Guid };

        CommandSetVoice setBuild = new()
        {
            entity = FighterIndex.GetFirstPlayer(QuantumRunner.Default.Game.Frames.Verified, index => index.GlobalNoBots == 0),
            voice = new() { Id = voice.AssetObject.Guid }
        };

        QuantumRunner.Default.Game.SendCommand(setBuild);
    }

    public void SetOnPlayer(SerializableWrapper<Build> build, FighterIndex index)
    {
        CommandSetBuild setBuild = new()
        {
            entity = FighterIndex.GetPlayerFromIndex(QuantumRunner.Default.Game.Frames.Verified, index),
            build = build
        };

        QuantumRunner.Default.Game.SendCommand(setBuild);
        PlayerStatController.Instance.HUDS[index.Global].SetPlayerIcon(build.Icon);
    }

    public void SetOnPlayerDefault(SerializableWrapper<Build> build)
    {
        SetOnPlayer(build, FighterIndex.GetFirstFighterIndex(QuantumRunner.Default.Game.Frames.Verified, index => index.GlobalNoBots == 0));
    }

    public void SetOnPlayerDefault()
    {
        SetOnPlayer(_currentlySelected, FighterIndex.GetFirstFighterIndex(QuantumRunner.Default.Game.Frames.Verified, index => index.GlobalNoBots == 0));
    }
}
