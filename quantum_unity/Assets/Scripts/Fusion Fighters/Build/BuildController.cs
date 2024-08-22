using Extensions.Components.Miscellaneous;
using GameResources.UI.Popup;
using Quantum;
using UnityEngine;
using UnityEngine.InputSystem;

public class BuildController : Controller<BuildController>
{
    [SerializeField] private BuildAssetAsset _defaultProfileBuild;
    [SerializeField] private BuildAssetAsset _default;
    [SerializeField] private BuildAssetAsset _none;

    [SerializeField] private Popup _savePopup;

    [SerializeField] private Shader _renderShader;

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

        randomBuild.Frame.Avatar = new()
        {
            Avatar = new() { Id = avatarStyles[Random.Range(0, avatarStyles.Count)].AssetObject.Guid },
            Color = new() { Id = colors[Random.Range(0, colors.Count)].AssetObject.Guid }
        };
        randomBuild.Frame.Eyes = new()
        {
            Eyes = new() { Id = eyeStyles[Random.Range(0, eyeStyles.Count)].AssetObject.Guid },
            Color = new() { Id = colors[Random.Range(0, colors.Count)].AssetObject.Guid }
        };
        randomBuild.Frame.Hair = new()
        {
            Hair = new() { Id = hairStyles[Random.Range(0, hairStyles.Count)].AssetObject.Guid },
            Color = new() { Id = colors[Random.Range(0, colors.Count)].AssetObject.Guid }
        };
        randomBuild.Frame.Voice = new() { Id = voiceStyles[Random.Range(0, voiceStyles.Count)].AssetObject.Guid };

        AssetGuid newGuid = AssetGuid.NewGuid();
        _currentBuild = new(randomBuild, GetPath(), "Untitled", "", newGuid, filterTags, groupTags);

        _isDirty = true;
    }

    public void Save(SerializableWrapper<Build> build)
    {
        build.Save();
    }

    public void SaveCurrent()
    {
        _currentBuild.Save();
        _isDirty = false;

        EntityRef player = FighterIndex.GetFirstEntity(QuantumRunner.Default.Game.Frames.Verified, item => item.Type == FighterType.Human);
        FighterIndex index = FighterIndex.GetFirstFighterIndex(QuantumRunner.Default.Game.Frames.Verified, item => item.Type == FighterType.Human);
        GameObject playerObj = FindFirstObjectByType<EntityViewUpdater>().GetView(player).gameObject;

        Camera renderCamera = playerObj.GetComponentInChildren<Camera>();
        _currentBuild.CreateIcon(renderCamera, _renderShader, FindFirstObjectByType<PlayerSpawnEventListener>().PlayerIcons[index.Global], playerObj.GetComponent<CustomQuantumAnimator>().Direction == -1);

        foreach (var userProfile in FusionFighters.Serializer.LoadAllFromDirectory<SerializableWrapper<UserProfile>>(UserProfileController.GetPath()))
        {
            var newUserProfile = userProfile;

            if (userProfile.value.LastBuild.Equals(_currentBuild))
                newUserProfile.value.SetLastBuild(_currentBuild);

            newUserProfile.Save();
        }

        ToastController.Instance.Spawn("Build saved");
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
        foreach (var userProfile in FusionFighters.Serializer.LoadAllFromDirectory<SerializableWrapper<UserProfile>>(UserProfileController.GetPath()))
        {
            var newUserProfile = userProfile;

            if (userProfile.value.LastBuild.Equals(_currentBuild))
                newUserProfile.value.SetLastBuild(_defaultProfileBuild.Build);

            newUserProfile.Save();
        }

        _currentBuild.Delete();

        if (BuildPopulator.Instance && BuildPopulator.Instance.TryGetButtonFromItem(_currentBuild, out GameObject button))
        {
            DestroyImmediate(button);
            BuildPopulator.Instance.GetComponentInParent<SelectAuto>().SetSelectedItem(SelectAuto.SelectType.First);
        }

        ToastController.Instance.Spawn("Build deleted");
    }

    public unsafe void SetAltWeaponOnPlayer(SerializableWrapper<Weapon> weapon)
    {
        _currentBuild.value.Gear.AltWeapon = weapon;
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
        _currentBuild.value.Gear.AltWeapon = default;
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
        _currentBuild.value.Frame.Avatar.Avatar = new() { Id = avatar.AssetObject.Guid };
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
        _currentBuild.value.Frame.Avatar.Color = new() { Id = color.AssetObject.Guid };
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
        _currentBuild.value.Gear.Badge = new() { Id = badge.AssetObject.Guid };
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
        _currentBuild.value.Gear.Badge = default;
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
        _currentBuild.value.Outfit.Clothing = clothing;
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
        _currentBuild.value.Outfit.Clothing = default;
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
        _currentBuild.value.Outfit.Headgear = headgear;
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
        _currentBuild.value.Outfit.Headgear = default;
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
        _currentBuild.value.Outfit.Legwear = legwear;
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
        _currentBuild.value.Outfit.Legwear = default;
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
        _currentBuild.value.Emotes.Up.Emote = new() { Id = emote.AssetObject.Guid };
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
        _currentBuild.value.Emotes.Up.Message = new() { Id = message.AssetObject.Guid };
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
        _currentBuild.value.Emotes.Down.Emote = new() { Id = emote.AssetObject.Guid };
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
        _currentBuild.value.Emotes.Down.Message = new() { Id = message.AssetObject.Guid };
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
        _currentBuild.value.Emotes.Left.Emote = new() { Id = emote.AssetObject.Guid };
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
        _currentBuild.value.Emotes.Left.Message = new() { Id = message.AssetObject.Guid };
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
        _currentBuild.value.Emotes.Right.Emote = new() { Id = emote.AssetObject.Guid };
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
        _currentBuild.value.Emotes.Right.Message = new() { Id = message.AssetObject.Guid };
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
        _currentBuild.value.Frame.Eyes.Eyes = new() { Id = eyes.AssetObject.Guid };
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
        _currentBuild.value.Frame.Eyes.Color = new() { Id = color.AssetObject.Guid };
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
        _currentBuild.value.Frame.Hair.Hair = new() { Id = hair.AssetObject.Guid };
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
        _currentBuild.value.Frame.Hair.Color = new() { Id = color.AssetObject.Guid };
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
        _currentBuild.value.Gear.MainWeapon = weapon;
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
        _currentBuild.value.Gear.MainWeapon = default;
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
        _currentBuild.value.Gear.SubWeapon = sub;
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
        _currentBuild.value.Gear.SubWeapon = default;
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
        _currentBuild.value.Gear.Ultimate = new() { Id = ultimate.AssetObject.Guid };
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
        _currentBuild.value.Gear.Ultimate = default;
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
        _currentBuild.value.Frame.Voice = new() { Id = voice.AssetObject.Guid };
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

        if (PlayerJoinController.Instance.TryGetPlayer(index, out LocalPlayerInfo player) && player.Profile.MadeByPlayer)
        {
            player.Profile.value.SetLastBuild(build);
            player.Profile.Save();
        }
    }

    public unsafe void ChangeTeam(InputAction.CallbackContext ctx)
    {
        if (!PlayerJoinController.Instance.TryGetPlayer(ctx.control.device, out LocalPlayerInfo player))
            return;

        EntityRef entity = FighterIndex.GetPlayerFromIndex(QuantumRunner.Default.Game.Frames.Verified, player.Index);

        if (QuantumRunner.Default.Game.Frames.Verified.TryGet(entity, out PlayerStats stats))
        {
            CommandChangeTeam command = new()
            {
                player = entity,
                teamIndex = (stats.Index.Team + 1) % QuantumRunner.Default.Game.Frames.Verified.Global->PlayersReady
            };

            QuantumRunner.Default.Game.SendCommand(command);
        }
    }

    public void SetOnPlayerDefault(SerializableWrapper<Build> build)
    {
        SetOnPlayer(build, FighterIndex.GetFirstFighterIndex(QuantumRunner.Default.Game.Frames.Verified, index => index.Type == FighterType.Human));
    }

    public void SetOnPlayerDefault()
    {
        SetOnPlayer(_currentBuild, FighterIndex.GetFirstFighterIndex(QuantumRunner.Default.Game.Frames.Verified, index => index.Type == FighterType.Human));
    }

    public void SetBehaviorOnBotDefault(BehaviorAsset behavior)
    {
        CommandSetBehavior command = new()
        {
            entity = FighterIndex.GetFirstEntity(QuantumRunner.Default.Game.Frames.Verified, item => item.Type == FighterType.Bot),
            behavior = new() { Id = behavior.AssetObject.Guid }
        };

        QuantumRunner.Default.Game.SendCommand(command);
    }
}
