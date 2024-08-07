using Extensions.Components.Miscellaneous;
using GameResources.UI.Popup;
using Quantum;
using UnityEngine;

public class BuildController : Controller<BuildController>
{
    [SerializeField] private BuildAssetAsset _default;
    [SerializeField] private BuildAssetAsset _none;

    [SerializeField] private Popup _savePopup;

    private bool _isDirty;

    public static string GetPath() => $"{Application.persistentDataPath}/SaveData/Custom/Builds";

    private SerializableWrapper<Build> _currentBuild;
    public SerializableWrapper<Build> CurrentBuild => _currentBuild;
    public void SetCurrentlySelected(SerializableWrapper<Build> build)
    {
        _currentBuild = build;
        _isDirty = false;
    }

    public void New()
    {
        Build randomBuild = _default.Build.value;
        string[] filterTags = new string[] { };
        Extensions.Types.Tuple<string, string>[] groupTags = new Extensions.Types.Tuple<string, string>[] { };

        var colors = InventoryController.Instance.GetAllUnlocked<ColorPresetAsset>();
        var avatarStyles = InventoryController.Instance.GetAllUnlocked<FFAvatarAsset>();
        var eyeStyles = InventoryController.Instance.GetAllUnlocked<EyesAsset>();
        var hairStyles = InventoryController.Instance.GetAllUnlocked<HairAsset>();
        var voiceStyles = InventoryController.Instance.GetAllUnlocked<VoiceAsset>();

        randomBuild.Cosmetics.Avatar = new()
        {
            Avatar = new() { Id = avatarStyles[Random.Range(0, avatarStyles.Count)].AssetObject.Guid },
            Color = new() { Id = colors[Random.Range(0, colors.Count)].AssetObject.Guid }
        };
        randomBuild.Cosmetics.Eyes = new()
        {
            Eyes = new() { Id = eyeStyles[Random.Range(0, eyeStyles.Count)].AssetObject.Guid },
            Color = new() { Id = colors[Random.Range(0, colors.Count)].AssetObject.Guid }
        };
        randomBuild.Cosmetics.Hair = new()
        {
            Hair = new() { Id = hairStyles[Random.Range(0, hairStyles.Count)].AssetObject.Guid },
            Color = new() { Id = colors[Random.Range(0, colors.Count)].AssetObject.Guid }
        };
        randomBuild.Cosmetics.Voice = new() { Id = voiceStyles[Random.Range(0, voiceStyles.Count)].AssetObject.Guid };

        _currentBuild = new(randomBuild, "Untitled", "", System.DateTime.Now.Ticks, System.DateTime.Now.Ticks, AssetGuid.NewGuid(), filterTags, groupTags, string.Empty, null);
        _isDirty = true;
    }

    public void Save(SerializableWrapper<Build> build)
    {
        build.Save(GetPath());
    }

    public void SaveCurrent()
    {
        _currentBuild.Save(GetPath());
        _isDirty = true;
    }

    public void CloseOrSaveConfirm(InvokableGameObject invokable)
    {
        if (_isDirty)
        {
            (PopupController.Instance as PopupController).InsertEvent(invokable);
            PopupController.Instance.Spawn(_savePopup);
        }
        else
        {
            invokable.Invoke();
        }
    }

    public void SetName(string name)
    {
        _currentBuild.SetName(name);
        _isDirty = true;
    }

    public void SetDescription(string description)
    {
        _currentBuild.SetDescription(description);
        _isDirty = true;
    }

    public void Delete()
    {
        _currentBuild.Delete(GetPath());

        if (BuildPopulator.Instance && BuildPopulator.Instance.TryGetButtonFromItem(_currentBuild, out GameObject button))
            Destroy(button);
        
        FindFirstObjectByType<BuildPopulator>()?.GetComponent<SelectAuto>().SetSelectedItem(SelectAuto.SelectType.First);
    }

    public unsafe void SetAltWeaponOnPlayer(SerializableWrapper<Weapon> weapon)
    {
        _currentBuild.value.Equipment.Weapons.AltWeapon = weapon;
        _isDirty = true;

        CommandSetAltWeapon setBuild = new()
        {
            entity = FighterIndex.GetFirstEntity(QuantumRunner.Default.Game.Frames.Verified, index => index.Type == FighterType.Human),
            weapon = weapon
        };

        QuantumRunner.Default.Game.SendCommand(setBuild);
    }

    public unsafe void ClearAltWeaponOnPlayer()
    {
        _currentBuild.value.Equipment.Weapons.AltWeapon = default;
        _isDirty = true;

        CommandSetAltWeapon setBuild = new()
        {
            entity = FighterIndex.GetFirstEntity(QuantumRunner.Default.Game.Frames.Verified, index => index.Type == FighterType.Human),
            weapon = default
        };

        QuantumRunner.Default.Game.SendCommand(setBuild);
    }

    public unsafe void SetAvatarOnPlayer(FFAvatarAsset avatar)
    {
        _currentBuild.value.Cosmetics.Avatar.Avatar = new() { Id = avatar.AssetObject.Guid };
        _isDirty = true;

        CommandSetAvatar setBuild = new()
        {
            entity = FighterIndex.GetFirstEntity(QuantumRunner.Default.Game.Frames.Verified, index => index.Type == FighterType.Human),
            avatar = new() { Id = avatar.AssetObject.Guid }
        };

        QuantumRunner.Default.Game.SendCommand(setBuild);
    }

    public unsafe void SetAvatarColorOnPlayer(ColorPresetAsset color)
    {
        _currentBuild.value.Cosmetics.Avatar.Color = new() { Id = color.AssetObject.Guid };
        _isDirty = true;

        CommandSetAvatarColor setBuild = new()
        {
            entity = FighterIndex.GetFirstEntity(QuantumRunner.Default.Game.Frames.Verified, index => index.Type == FighterType.Human),
            color = new() { Id = color.AssetObject.Guid }
        };

        QuantumRunner.Default.Game.SendCommand(setBuild);
    }

    public unsafe void SetBadgeOnPlayer(BadgeAsset badge)
    {
        _currentBuild.value.Equipment.Badge = new() { Id = badge.AssetObject.Guid };
        _isDirty = true;

        CommandSetBadge setBuild = new()
        {
            entity = FighterIndex.GetFirstEntity(QuantumRunner.Default.Game.Frames.Verified, index => index.Type == FighterType.Human),
            badge = new() { Id = badge.AssetObject.Guid }
        };

        QuantumRunner.Default.Game.SendCommand(setBuild);
    }

    public unsafe void ClearBadgeOnPlayer()
    {
        _currentBuild.value.Equipment.Badge = default;
        _isDirty = true;

        CommandSetBadge setBuild = new()
        {
            entity = FighterIndex.GetFirstEntity(QuantumRunner.Default.Game.Frames.Verified, index => index.Type == FighterType.Human),
            badge = default
        };

        QuantumRunner.Default.Game.SendCommand(setBuild);
    }

    public unsafe void SetClothingOnPlayer(SerializableWrapper<Apparel> clothing)
    {
        _currentBuild.value.Equipment.Outfit.Clothing = clothing;
        _isDirty = true;

        CommandSetClothing setBuild = new()
        {
            entity = FighterIndex.GetFirstEntity(QuantumRunner.Default.Game.Frames.Verified, index => index.Type == FighterType.Human),
            clothing = clothing
        };

        QuantumRunner.Default.Game.SendCommand(setBuild);
    }

    public unsafe void ClearClothingOnPlayer()
    {
        _currentBuild.value.Equipment.Outfit.Clothing = default;
        _isDirty = true;

        CommandSetClothing setBuild = new()
        {
            entity = FighterIndex.GetFirstEntity(QuantumRunner.Default.Game.Frames.Verified, index => index.Type == FighterType.Human),
            clothing = default
        };

        QuantumRunner.Default.Game.SendCommand(setBuild);
    }

    public unsafe void SetHeadgearOnPlayer(SerializableWrapper<Apparel> headgear)
    {
        _currentBuild.value.Equipment.Outfit.Headgear = headgear;
        _isDirty = true;

        CommandSetHeadgear setBuild = new()
        {
            entity = FighterIndex.GetFirstEntity(QuantumRunner.Default.Game.Frames.Verified, index => index.Type == FighterType.Human),
            headgear = headgear
        };

        QuantumRunner.Default.Game.SendCommand(setBuild);
    }

    public unsafe void ClearHeadgearOnPlayer()
    {
        _currentBuild.value.Equipment.Outfit.Headgear = default;
        _isDirty = true;

        CommandSetHeadgear setBuild = new()
        {
            entity = FighterIndex.GetFirstEntity(QuantumRunner.Default.Game.Frames.Verified, index => index.Type == FighterType.Human),
            headgear = default
        };

        QuantumRunner.Default.Game.SendCommand(setBuild);
    }

    public unsafe void SetLegwearOnPlayer(SerializableWrapper<Apparel> legwear)
    {
        _currentBuild.value.Equipment.Outfit.Legwear = legwear;
        _isDirty = true;

        CommandSetLegwear setBuild = new()
        {
            entity = FighterIndex.GetFirstEntity(QuantumRunner.Default.Game.Frames.Verified, index => index.Type == FighterType.Human),
            legwear = legwear
        };

        QuantumRunner.Default.Game.SendCommand(setBuild);
    }

    public unsafe void ClearLegwearOnPlayer()
    {
        _currentBuild.value.Equipment.Outfit.Legwear = default;
        _isDirty = true;

        CommandSetLegwear setBuild = new()
        {
            entity = FighterIndex.GetFirstEntity(QuantumRunner.Default.Game.Frames.Verified, index => index.Type == FighterType.Human),
            legwear = default
        };

        QuantumRunner.Default.Game.SendCommand(setBuild);
    }

    public unsafe void SetEmoteUpOnPlayer(EmoteAsset emote)
    {
        _currentBuild.value.Cosmetics.Emotes.Up.Emote = new() { Id = emote.AssetObject.Guid };
        _isDirty = true;

        CommandSetEmoteUp setBuild = new()
        {
            entity = FighterIndex.GetFirstEntity(QuantumRunner.Default.Game.Frames.Verified, index => index.Type == FighterType.Human),
            emote = new() { Id = emote.AssetObject.Guid }
        };

        QuantumRunner.Default.Game.SendCommand(setBuild);
    }

    public unsafe void SetEmoteUpMessageOnPlayer(MessagePresetAsset message)
    {
        _currentBuild.value.Cosmetics.Emotes.Up.Message = new() { Id = message.AssetObject.Guid };
        _isDirty = true;

        CommandSetEmoteUpMessage setBuild = new()
        {
            entity = FighterIndex.GetFirstEntity(QuantumRunner.Default.Game.Frames.Verified, index => index.Type == FighterType.Human),
            message = new() { Id = message.AssetObject.Guid }
        };

        QuantumRunner.Default.Game.SendCommand(setBuild);
    }

    public unsafe void SetEmoteDownOnPlayer(EmoteAsset emote)
    {
        _currentBuild.value.Cosmetics.Emotes.Down.Emote = new() { Id = emote.AssetObject.Guid };
        _isDirty = true;

        CommandSetEmoteDown setBuild = new()
        {
            entity = FighterIndex.GetFirstEntity(QuantumRunner.Default.Game.Frames.Verified, index => index.Type == FighterType.Human),
            emote = new() { Id = emote.AssetObject.Guid }
        };

        QuantumRunner.Default.Game.SendCommand(setBuild);
    }

    public unsafe void SetEmoteDownMessageOnPlayer(MessagePresetAsset message)
    {
        _currentBuild.value.Cosmetics.Emotes.Down.Message = new() { Id = message.AssetObject.Guid };
        _isDirty = true;

        CommandSetEmoteDownMessage setBuild = new()
        {
            entity = FighterIndex.GetFirstEntity(QuantumRunner.Default.Game.Frames.Verified, index => index.Type == FighterType.Human),
            message = new() { Id = message.AssetObject.Guid }
        };

        QuantumRunner.Default.Game.SendCommand(setBuild);
    }

    public unsafe void SetEmoteLeftOnPlayer(EmoteAsset emote)
    {
        _currentBuild.value.Cosmetics.Emotes.Left.Emote = new() { Id = emote.AssetObject.Guid };
        _isDirty = true;

        CommandSetEmoteLeft setBuild = new()
        {
            entity = FighterIndex.GetFirstEntity(QuantumRunner.Default.Game.Frames.Verified, index => index.Type == FighterType.Human),
            emote = new() { Id = emote.AssetObject.Guid }
        };

        QuantumRunner.Default.Game.SendCommand(setBuild);
    }

    public unsafe void SetEmoteLeftMessageOnPlayer(MessagePresetAsset message)
    {
        _currentBuild.value.Cosmetics.Emotes.Left.Message = new() { Id = message.AssetObject.Guid };
        _isDirty = true;

        CommandSetEmoteLeftMessage setBuild = new()
        {
            entity = FighterIndex.GetFirstEntity(QuantumRunner.Default.Game.Frames.Verified, index => index.Type == FighterType.Human),
            message = new() { Id = message.AssetObject.Guid }
        };

        QuantumRunner.Default.Game.SendCommand(setBuild);
    }

    public unsafe void SetEmoteRightOnPlayer(EmoteAsset emote)
    {
        _currentBuild.value.Cosmetics.Emotes.Right.Emote = new() { Id = emote.AssetObject.Guid };
        _isDirty = true;

        CommandSetEmoteRight setBuild = new()
        {
            entity = FighterIndex.GetFirstEntity(QuantumRunner.Default.Game.Frames.Verified, index => index.Type == FighterType.Human),
            emote = new() { Id = emote.AssetObject.Guid }
        };

        QuantumRunner.Default.Game.SendCommand(setBuild);
    }

    public unsafe void SetEmoteRightMessageOnPlayer(MessagePresetAsset message)
    {
        _currentBuild.value.Cosmetics.Emotes.Right.Message = new() { Id = message.AssetObject.Guid };
        _isDirty = true;

        CommandSetEmoteRightMessage setBuild = new()
        {
            entity = FighterIndex.GetFirstEntity(QuantumRunner.Default.Game.Frames.Verified, index => index.Type == FighterType.Human),
            message = new() { Id = message.AssetObject.Guid }
        };

        QuantumRunner.Default.Game.SendCommand(setBuild);
    }

    public unsafe void SetEyesOnPlayer(EyesAsset eyes)
    {
        _currentBuild.value.Cosmetics.Eyes.Eyes = new() { Id = eyes.AssetObject.Guid };
        _isDirty = true;

        CommandSetEyes setBuild = new()
        {
            entity = FighterIndex.GetFirstEntity(QuantumRunner.Default.Game.Frames.Verified, index => index.Type == FighterType.Human),
            eyes = new() { Id = eyes.AssetObject.Guid }
        };

        QuantumRunner.Default.Game.SendCommand(setBuild);
    }

    public unsafe void SetEyeColorOnPlayer(ColorPresetAsset color)
    {
        _currentBuild.value.Cosmetics.Eyes.Color = new() { Id = color.AssetObject.Guid };
        _isDirty = true;

        CommandSetEyeColor setBuild = new()
        {
            entity = FighterIndex.GetFirstEntity(QuantumRunner.Default.Game.Frames.Verified, index => index.Type == FighterType.Human),
            color = new() { Id = color.AssetObject.Guid }
        };

        QuantumRunner.Default.Game.SendCommand(setBuild);
    }

    public unsafe void SetHairOnPlayer(HairAsset hair)
    {
        _currentBuild.value.Cosmetics.Hair.Hair = new() { Id = hair.AssetObject.Guid };
        _isDirty = true;

        CommandSetHair setBuild = new()
        {
            entity = FighterIndex.GetFirstEntity(QuantumRunner.Default.Game.Frames.Verified, index => index.Type == FighterType.Human),
            hair = new() { Id = hair.AssetObject.Guid }
        };

        QuantumRunner.Default.Game.SendCommand(setBuild);
    }

    public unsafe void SetHairColorOnPlayer(ColorPresetAsset color)
    {
        _currentBuild.value.Cosmetics.Hair.Color = new() { Id = color.AssetObject.Guid };
        _isDirty = true;

        CommandSetHairColor setBuild = new()
        {
            entity = FighterIndex.GetFirstEntity(QuantumRunner.Default.Game.Frames.Verified, index => index.Type == FighterType.Human),
            color = new() { Id = color.AssetObject.Guid }
        };

        QuantumRunner.Default.Game.SendCommand(setBuild);
    }

    public unsafe void SetMainWeaponOnPlayer(SerializableWrapper<Weapon> weapon)
    {
        _currentBuild.value.Equipment.Weapons.MainWeapon = weapon;
        _isDirty = true;

        CommandSetMainWeapon setBuild = new()
        {
            entity = FighterIndex.GetFirstEntity(QuantumRunner.Default.Game.Frames.Verified, index => index.Type == FighterType.Human),
            weapon = weapon
        };

        QuantumRunner.Default.Game.SendCommand(setBuild);
    }

    public unsafe void ClearMainWeaponOnPlayer()
    {
        _currentBuild.value.Equipment.Weapons.MainWeapon = default;
        _isDirty = true;

        CommandSetMainWeapon setBuild = new()
        {
            entity = FighterIndex.GetFirstEntity(QuantumRunner.Default.Game.Frames.Verified, index => index.Type == FighterType.Human),
            weapon = default
        };

        QuantumRunner.Default.Game.SendCommand(setBuild);
    }

    public unsafe void SetSubOnPlayer(SerializableWrapper<Sub> sub)
    {
        _currentBuild.value.Equipment.Weapons.SubWeapon = sub;
        _isDirty = true;

        CommandSetSub setBuild = new()
        {
            entity = FighterIndex.GetFirstEntity(QuantumRunner.Default.Game.Frames.Verified, index => index.Type == FighterType.Human),
            sub = sub
        };

        QuantumRunner.Default.Game.SendCommand(setBuild);
    }

    public unsafe void ClearSubOnPlayer()
    {
        _currentBuild.value.Equipment.Weapons.SubWeapon = default;
        _isDirty = true;

        CommandSetSub setBuild = new()
        {
            entity = FighterIndex.GetFirstEntity(QuantumRunner.Default.Game.Frames.Verified, index => index.Type == FighterType.Human),
            sub = default
        };

        QuantumRunner.Default.Game.SendCommand(setBuild);
    }

    public unsafe void SetUltimateOnPlayer(UltimateAsset ultimate)
    {
        _currentBuild.value.Equipment.Ultimate = new() { Id = ultimate.AssetObject.Guid };
        _isDirty = true;

        CommandSetUltimate setBuild = new()
        {
            entity = FighterIndex.GetFirstEntity(QuantumRunner.Default.Game.Frames.Verified, index => index.Type == FighterType.Human),
            ultimate = new() { Id = ultimate.AssetObject.Guid }
        };

        QuantumRunner.Default.Game.SendCommand(setBuild);
    }

    public unsafe void ClearUltimateOnPlayer()
    {
        _currentBuild.value.Equipment.Ultimate = default;
        _isDirty = true;

        CommandSetUltimate setBuild = new()
        {
            entity = FighterIndex.GetFirstEntity(QuantumRunner.Default.Game.Frames.Verified, index => index.Type == FighterType.Human),
            ultimate = default
        };

        QuantumRunner.Default.Game.SendCommand(setBuild);
    }

    public unsafe void SetVoiceOnPlayer(VoiceAsset voice)
    {
        _currentBuild.value.Cosmetics.Voice = new() { Id = voice.AssetObject.Guid };
        _isDirty = true;

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
        SetOnPlayer(_currentBuild, FighterIndex.GetFirstFighterIndex(QuantumRunner.Default.Game.Frames.Verified, index => index.Type == FighterType.Human));
    }
}
