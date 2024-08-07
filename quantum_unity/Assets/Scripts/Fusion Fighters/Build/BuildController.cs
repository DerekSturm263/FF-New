using Extensions.Components.Miscellaneous;
using Quantum;
using UnityEngine;

public class BuildController : Controller<BuildController>
{
    [SerializeField] private BuildAssetAsset _default;

    public static string GetPath() => $"{Application.persistentDataPath}/SaveData/Custom/Builds";

    private SerializableWrapper<Build> _currentlySelected;
    public SerializableWrapper<Build> CurrentlySelected => _currentlySelected;
    public void SetCurrentlySelected(SerializableWrapper<Build> build) => _currentlySelected = build;

    public void New()
    {
        Build randomBuild = _default.Build.value;
        string[] filterTags = new string[] { };
        Extensions.Types.Tuple<string, string>[] groupTags = new Extensions.Types.Tuple<string, string>[] { };

        // TODO: MAKE IT RANDOM
        //randomBuild.Cosmetics.Avatar = new() { Avatar = _avatars[Random.Range(0, _avatars.Length)], Color = _colors[Random.Range(0, _colors.Length)] };
        //randomBuild.Cosmetics.Hair = new() { Hair = _hair[Random.Range(0, _hair.Length)], Color = _colors[Random.Range(0, _colors.Length)] };
        //randomBuild.Cosmetics.Eyes = new() { Eyes = _eyes[Random.Range(0, _eyes.Length)], Color = _colors[Random.Range(0, _colors.Length)] };
        //randomBuild.Cosmetics.Voice = _voices[Random.Range(0, _voices.Length)];

        _currentlySelected = new(randomBuild, "Untitled", "", System.DateTime.Now.Ticks, System.DateTime.Now.Ticks, AssetGuid.NewGuid(), filterTags, groupTags, string.Empty, null);
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

        if (BuildPopulator.Instance && BuildPopulator.Instance.TryGetButtonFromItem(_currentlySelected, out GameObject button))
            Destroy(button);
        
        FindFirstObjectByType<BuildPopulator>().GetComponent<SelectAuto>().SetSelectedItem(SelectAuto.SelectType.First);
    }

    public unsafe void SetAltWeaponOnPlayer(SerializableWrapper<Weapon> weapon)
    {
        _currentlySelected.value.Equipment.Weapons.AltWeapon = weapon;

        CommandSetAltWeapon setBuild = new()
        {
            entity = FighterIndex.GetFirstEntity(QuantumRunner.Default.Game.Frames.Verified, index => index.Type == FighterType.Human),
            weapon = weapon
        };

        QuantumRunner.Default.Game.SendCommand(setBuild);
    }

    public unsafe void ClearAltWeaponOnPlayer()
    {
        _currentlySelected.value.Equipment.Weapons.AltWeapon = default;

        CommandSetAltWeapon setBuild = new()
        {
            entity = FighterIndex.GetFirstEntity(QuantumRunner.Default.Game.Frames.Verified, index => index.Type == FighterType.Human),
            weapon = default
        };

        QuantumRunner.Default.Game.SendCommand(setBuild);
    }

    public unsafe void SetAvatarOnPlayer(FFAvatarAsset avatar)
    {
        _currentlySelected.value.Cosmetics.Avatar.Avatar = new() { Id = avatar.AssetObject.Guid };

        CommandSetAvatar setBuild = new()
        {
            entity = FighterIndex.GetFirstEntity(QuantumRunner.Default.Game.Frames.Verified, index => index.Type == FighterType.Human),
            avatar = new() { Id = avatar.AssetObject.Guid }
        };

        QuantumRunner.Default.Game.SendCommand(setBuild);
    }

    public unsafe void SetAvatarColorOnPlayer(ColorPresetAsset color)
    {
        _currentlySelected.value.Cosmetics.Avatar.Color = new() { Id = color.AssetObject.Guid };

        CommandSetAvatarColor setBuild = new()
        {
            entity = FighterIndex.GetFirstEntity(QuantumRunner.Default.Game.Frames.Verified, index => index.Type == FighterType.Human),
            color = new() { Id = color.AssetObject.Guid }
        };

        QuantumRunner.Default.Game.SendCommand(setBuild);
    }

    public unsafe void SetBadgeOnPlayer(BadgeAsset badge)
    {
        _currentlySelected.value.Equipment.Badge = new() { Id = badge.AssetObject.Guid };

        CommandSetBadge setBuild = new()
        {
            entity = FighterIndex.GetFirstEntity(QuantumRunner.Default.Game.Frames.Verified, index => index.Type == FighterType.Human),
            badge = new() { Id = badge.AssetObject.Guid }
        };

        QuantumRunner.Default.Game.SendCommand(setBuild);
    }

    public unsafe void ClearBadgeOnPlayer()
    {
        _currentlySelected.value.Equipment.Badge = default;

        CommandSetBadge setBuild = new()
        {
            entity = FighterIndex.GetFirstEntity(QuantumRunner.Default.Game.Frames.Verified, index => index.Type == FighterType.Human),
            badge = default
        };

        QuantumRunner.Default.Game.SendCommand(setBuild);
    }

    public unsafe void SetClothingOnPlayer(SerializableWrapper<Apparel> clothing)
    {
        _currentlySelected.value.Equipment.Outfit.Clothing = clothing;

        CommandSetClothing setBuild = new()
        {
            entity = FighterIndex.GetFirstEntity(QuantumRunner.Default.Game.Frames.Verified, index => index.Type == FighterType.Human),
            clothing = clothing
        };

        QuantumRunner.Default.Game.SendCommand(setBuild);
    }

    public unsafe void ClearClothingOnPlayer()
    {
        _currentlySelected.value.Equipment.Outfit.Clothing = default;

        CommandSetClothing setBuild = new()
        {
            entity = FighterIndex.GetFirstEntity(QuantumRunner.Default.Game.Frames.Verified, index => index.Type == FighterType.Human),
            clothing = default
        };

        QuantumRunner.Default.Game.SendCommand(setBuild);
    }

    public unsafe void SetHeadgearOnPlayer(SerializableWrapper<Apparel> headgear)
    {
        _currentlySelected.value.Equipment.Outfit.Headgear = headgear;

        CommandSetHeadgear setBuild = new()
        {
            entity = FighterIndex.GetFirstEntity(QuantumRunner.Default.Game.Frames.Verified, index => index.Type == FighterType.Human),
            headgear = headgear
        };

        QuantumRunner.Default.Game.SendCommand(setBuild);
    }

    public unsafe void ClearHeadgearOnPlayer()
    {
        _currentlySelected.value.Equipment.Outfit.Headgear = default;

        CommandSetHeadgear setBuild = new()
        {
            entity = FighterIndex.GetFirstEntity(QuantumRunner.Default.Game.Frames.Verified, index => index.Type == FighterType.Human),
            headgear = default
        };

        QuantumRunner.Default.Game.SendCommand(setBuild);
    }

    public unsafe void SetLegwearOnPlayer(SerializableWrapper<Apparel> legwear)
    {
        _currentlySelected.value.Equipment.Outfit.Legwear = legwear;

        CommandSetLegwear setBuild = new()
        {
            entity = FighterIndex.GetFirstEntity(QuantumRunner.Default.Game.Frames.Verified, index => index.Type == FighterType.Human),
            legwear = legwear
        };

        QuantumRunner.Default.Game.SendCommand(setBuild);
    }

    public unsafe void ClearLegwearOnPlayer()
    {
        _currentlySelected.value.Equipment.Outfit.Legwear = default;

        CommandSetLegwear setBuild = new()
        {
            entity = FighterIndex.GetFirstEntity(QuantumRunner.Default.Game.Frames.Verified, index => index.Type == FighterType.Human),
            legwear = default
        };

        QuantumRunner.Default.Game.SendCommand(setBuild);
    }

    public unsafe void SetEmoteUpOnPlayer(EmoteAsset emote)
    {
        _currentlySelected.value.Cosmetics.Emotes.Up.Emote = new() { Id = emote.AssetObject.Guid };

        CommandSetEmoteUp setBuild = new()
        {
            entity = FighterIndex.GetFirstEntity(QuantumRunner.Default.Game.Frames.Verified, index => index.Type == FighterType.Human),
            emote = new() { Id = emote.AssetObject.Guid }
        };

        QuantumRunner.Default.Game.SendCommand(setBuild);
    }

    public unsafe void SetEmoteUpMessageOnPlayer(MessagePresetAsset message)
    {
        _currentlySelected.value.Cosmetics.Emotes.Up.Message = new() { Id = message.AssetObject.Guid };

        CommandSetEmoteUpMessage setBuild = new()
        {
            entity = FighterIndex.GetFirstEntity(QuantumRunner.Default.Game.Frames.Verified, index => index.Type == FighterType.Human),
            message = new() { Id = message.AssetObject.Guid }
        };

        QuantumRunner.Default.Game.SendCommand(setBuild);
    }

    public unsafe void SetEmoteDownOnPlayer(EmoteAsset emote)
    {
        _currentlySelected.value.Cosmetics.Emotes.Down.Emote = new() { Id = emote.AssetObject.Guid };

        CommandSetEmoteDown setBuild = new()
        {
            entity = FighterIndex.GetFirstEntity(QuantumRunner.Default.Game.Frames.Verified, index => index.Type == FighterType.Human),
            emote = new() { Id = emote.AssetObject.Guid }
        };

        QuantumRunner.Default.Game.SendCommand(setBuild);
    }

    public unsafe void SetEmoteDownMessageOnPlayer(MessagePresetAsset message)
    {
        _currentlySelected.value.Cosmetics.Emotes.Down.Message = new() { Id = message.AssetObject.Guid };

        CommandSetEmoteDownMessage setBuild = new()
        {
            entity = FighterIndex.GetFirstEntity(QuantumRunner.Default.Game.Frames.Verified, index => index.Type == FighterType.Human),
            message = new() { Id = message.AssetObject.Guid }
        };

        QuantumRunner.Default.Game.SendCommand(setBuild);
    }

    public unsafe void SetEmoteLeftOnPlayer(EmoteAsset emote)
    {
        _currentlySelected.value.Cosmetics.Emotes.Left.Emote = new() { Id = emote.AssetObject.Guid };

        CommandSetEmoteLeft setBuild = new()
        {
            entity = FighterIndex.GetFirstEntity(QuantumRunner.Default.Game.Frames.Verified, index => index.Type == FighterType.Human),
            emote = new() { Id = emote.AssetObject.Guid }
        };

        QuantumRunner.Default.Game.SendCommand(setBuild);
    }

    public unsafe void SetEmoteLeftMessageOnPlayer(MessagePresetAsset message)
    {
        _currentlySelected.value.Cosmetics.Emotes.Left.Message = new() { Id = message.AssetObject.Guid };

        CommandSetEmoteLeftMessage setBuild = new()
        {
            entity = FighterIndex.GetFirstEntity(QuantumRunner.Default.Game.Frames.Verified, index => index.Type == FighterType.Human),
            message = new() { Id = message.AssetObject.Guid }
        };

        QuantumRunner.Default.Game.SendCommand(setBuild);
    }

    public unsafe void SetEmoteRightOnPlayer(EmoteAsset emote)
    {
        _currentlySelected.value.Cosmetics.Emotes.Right.Emote = new() { Id = emote.AssetObject.Guid };

        CommandSetEmoteRight setBuild = new()
        {
            entity = FighterIndex.GetFirstEntity(QuantumRunner.Default.Game.Frames.Verified, index => index.Type == FighterType.Human),
            emote = new() { Id = emote.AssetObject.Guid }
        };

        QuantumRunner.Default.Game.SendCommand(setBuild);
    }

    public unsafe void SetEmoteRightMessageOnPlayer(MessagePresetAsset message)
    {
        _currentlySelected.value.Cosmetics.Emotes.Right.Message = new() { Id = message.AssetObject.Guid };

        CommandSetEmoteRightMessage setBuild = new()
        {
            entity = FighterIndex.GetFirstEntity(QuantumRunner.Default.Game.Frames.Verified, index => index.Type == FighterType.Human),
            message = new() { Id = message.AssetObject.Guid }
        };

        QuantumRunner.Default.Game.SendCommand(setBuild);
    }

    public unsafe void SetEyesOnPlayer(EyesAsset eyes)
    {
        _currentlySelected.value.Cosmetics.Eyes.Eyes = new() { Id = eyes.AssetObject.Guid };

        CommandSetEyes setBuild = new()
        {
            entity = FighterIndex.GetFirstEntity(QuantumRunner.Default.Game.Frames.Verified, index => index.Type == FighterType.Human),
            eyes = new() { Id = eyes.AssetObject.Guid }
        };

        QuantumRunner.Default.Game.SendCommand(setBuild);
    }

    public unsafe void SetEyeColorOnPlayer(ColorPresetAsset color)
    {
        _currentlySelected.value.Cosmetics.Eyes.Color = new() { Id = color.AssetObject.Guid };

        CommandSetEyeColor setBuild = new()
        {
            entity = FighterIndex.GetFirstEntity(QuantumRunner.Default.Game.Frames.Verified, index => index.Type == FighterType.Human),
            color = new() { Id = color.AssetObject.Guid }
        };

        QuantumRunner.Default.Game.SendCommand(setBuild);
    }

    public unsafe void SetHairOnPlayer(HairAsset hair)
    {
        _currentlySelected.value.Cosmetics.Hair.Hair = new() { Id = hair.AssetObject.Guid };

        CommandSetHair setBuild = new()
        {
            entity = FighterIndex.GetFirstEntity(QuantumRunner.Default.Game.Frames.Verified, index => index.Type == FighterType.Human),
            hair = new() { Id = hair.AssetObject.Guid }
        };

        QuantumRunner.Default.Game.SendCommand(setBuild);
    }

    public unsafe void SetHairColorOnPlayer(ColorPresetAsset color)
    {
        _currentlySelected.value.Cosmetics.Hair.Color = new() { Id = color.AssetObject.Guid };

        CommandSetHairColor setBuild = new()
        {
            entity = FighterIndex.GetFirstEntity(QuantumRunner.Default.Game.Frames.Verified, index => index.Type == FighterType.Human),
            color = new() { Id = color.AssetObject.Guid }
        };

        QuantumRunner.Default.Game.SendCommand(setBuild);
    }

    public unsafe void SetMainWeaponOnPlayer(SerializableWrapper<Weapon> weapon)
    {
        _currentlySelected.value.Equipment.Weapons.MainWeapon = weapon;

        CommandSetMainWeapon setBuild = new()
        {
            entity = FighterIndex.GetFirstEntity(QuantumRunner.Default.Game.Frames.Verified, index => index.Type == FighterType.Human),
            weapon = weapon
        };

        QuantumRunner.Default.Game.SendCommand(setBuild);
    }

    public unsafe void ClearMainWeaponOnPlayer()
    {
        _currentlySelected.value.Equipment.Weapons.MainWeapon = default;

        CommandSetMainWeapon setBuild = new()
        {
            entity = FighterIndex.GetFirstEntity(QuantumRunner.Default.Game.Frames.Verified, index => index.Type == FighterType.Human),
            weapon = default
        };

        QuantumRunner.Default.Game.SendCommand(setBuild);
    }

    public unsafe void SetSubOnPlayer(SerializableWrapper<Sub> sub)
    {
        _currentlySelected.value.Equipment.Weapons.SubWeapon = sub;

        CommandSetSub setBuild = new()
        {
            entity = FighterIndex.GetFirstEntity(QuantumRunner.Default.Game.Frames.Verified, index => index.Type == FighterType.Human),
            sub = sub
        };

        QuantumRunner.Default.Game.SendCommand(setBuild);
    }

    public unsafe void ClearSubOnPlayer()
    {
        _currentlySelected.value.Equipment.Weapons.SubWeapon = default;

        CommandSetSub setBuild = new()
        {
            entity = FighterIndex.GetFirstEntity(QuantumRunner.Default.Game.Frames.Verified, index => index.Type == FighterType.Human),
            sub = default
        };

        QuantumRunner.Default.Game.SendCommand(setBuild);
    }

    public unsafe void SetUltimateOnPlayer(UltimateAsset ultimate)
    {
        _currentlySelected.value.Equipment.Ultimate = new() { Id = ultimate.AssetObject.Guid };

        CommandSetUltimate setBuild = new()
        {
            entity = FighterIndex.GetFirstEntity(QuantumRunner.Default.Game.Frames.Verified, index => index.Type == FighterType.Human),
            ultimate = new() { Id = ultimate.AssetObject.Guid }
        };

        QuantumRunner.Default.Game.SendCommand(setBuild);
    }

    public unsafe void ClearUltimateOnPlayer()
    {
        _currentlySelected.value.Equipment.Ultimate = default;

        CommandSetUltimate setBuild = new()
        {
            entity = FighterIndex.GetFirstEntity(QuantumRunner.Default.Game.Frames.Verified, index => index.Type == FighterType.Human),
            ultimate = default
        };

        QuantumRunner.Default.Game.SendCommand(setBuild);
    }

    public unsafe void SetVoiceOnPlayer(VoiceAsset voice)
    {
        _currentlySelected.value.Cosmetics.Voice = new() { Id = voice.AssetObject.Guid };

        CommandSetVoice setBuild = new()
        {
            entity = FighterIndex.GetFirstEntity(QuantumRunner.Default.Game.Frames.Verified, index => index.Type == FighterType.Human),
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
        SetOnPlayer(build, FighterIndex.GetFirstFighterIndex(QuantumRunner.Default.Game.Frames.Verified, index => index.Type == FighterType.Human));
    }

    public void SetOnPlayerDefault()
    {
        SetOnPlayer(_currentlySelected, FighterIndex.GetFirstFighterIndex(QuantumRunner.Default.Game.Frames.Verified, index => index.Type == FighterType.Human));
    }
}
