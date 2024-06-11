using Extensions.Components.Miscellaneous;
using Quantum;
using System.Collections.Generic;
using UnityEngine;

public class BuildController : Controller<BuildController>
{
    public static string GetPath() => $"{Application.persistentDataPath}/Builds";

    private readonly Dictionary<int, EntityRef> _localIndicesToPlayers = new();
    private readonly Dictionary<int, EntityRef> _globalIndicesToPlayers = new();

    public EntityRef GetPlayerLocalIndex(int playerIndex)
    {
        if (!_localIndicesToPlayers.ContainsKey(playerIndex))
        {
            foreach (var stats in QuantumRunner.Default.Game.Frames.Verified.GetComponentIterator<Stats>())
            {
                if (stats.Component.LocalIndex == playerIndex)
                {
                    _localIndicesToPlayers[playerIndex] = stats.Entity;
                    break;
                }
            }
        }

        return _localIndicesToPlayers[playerIndex];
    }

    public EntityRef GetPlayerGlobalIndex(int playerIndex)
    {
        if (!_globalIndicesToPlayers.ContainsKey(playerIndex))
        {
            foreach (var stats in QuantumRunner.Default.Game.Frames.Verified.GetComponentIterator<Stats>())
            {
                if (stats.Component.GlobalIndex == playerIndex)
                {
                    _globalIndicesToPlayers[playerIndex] = stats.Entity;
                    break;
                }
            }
        }

        return _globalIndicesToPlayers[playerIndex];
    }

    public SerializableWrapper<Build> New()
    {
        Build build = new();
        return new(build, "Untitled", "", AssetGuid.NewGuid(), System.DateTime.Now.Ticks, System.DateTime.Now.Ticks);
    }

    public void Save(SerializableWrapper<Build> build)
    {
        Serializer.Save(build, build.Value.SerializableData.Guid, GetPath());
    }

    public void SaveOnPlayer(int playerIndex)
    {
        if (QuantumRunner.Default.Game.Frames.Verified.TryGet(GetPlayerLocalIndex(playerIndex), out Stats stats))
            Serializer.Save(stats.Build, stats.Build.SerializableData.Guid, GetPath());
    }

    public void SetAltWeaponOnPlayer(SerializableWrapper<Weapon> weapon)
    {
        CommandSetAltWeapon setBuild = new()
        {
            entity = GetPlayerLocalIndex(0),
            weapon = weapon
        };

        QuantumRunner.Default.Game.SendCommand(setBuild);
    }

    public void SetAvatarOnPlayer(FFAvatarAsset avatar)
    {
        CommandSetAvatar setBuild = new()
        {
            entity = GetPlayerLocalIndex(0),
            avatar = new() { Id = avatar.AssetObject.Guid }
        };

        QuantumRunner.Default.Game.SendCommand(setBuild);
    }

    public void SetBadgeOnPlayer(BadgeAsset badge)
    {
        CommandSetBadge setBuild = new()
        {
            entity = GetPlayerLocalIndex(0),
            badge = new() { Id = badge.AssetObject.Guid }
        };

        QuantumRunner.Default.Game.SendCommand(setBuild);
    }

    public void SetClothingOnPlayer(SerializableWrapper<Apparel> clothing)
    {
        CommandSetClothing setBuild = new()
        {
            entity = GetPlayerLocalIndex(0),
            clothing = clothing
        };

        QuantumRunner.Default.Game.SendCommand(setBuild);
    }

    public void SetHeadgearOnPlayer(SerializableWrapper<Apparel> headgear)
    {
        CommandSetHeadgear setBuild = new()
        {
            entity = GetPlayerLocalIndex(0),
            headgear = headgear
        };

        QuantumRunner.Default.Game.SendCommand(setBuild);
    }

    public void SetLegwearOnPlayer(SerializableWrapper<Apparel> legwear)
    {
        CommandSetLegwear setBuild = new()
        {
            entity = GetPlayerLocalIndex(0),
            legwear = legwear
        };

        QuantumRunner.Default.Game.SendCommand(setBuild);
    }

    public void SetEmoteUpOnPlayer(EmoteAsset emote)
    {
        CommandSetEmoteUp setBuild = new()
        {
            entity = GetPlayerLocalIndex(0),
            emote = new() { Id = emote.AssetObject.Guid }
        };

        QuantumRunner.Default.Game.SendCommand(setBuild);
    }

    public void SetEmoteDownOnPlayer(EmoteAsset emote)
    {
        CommandSetEmoteDown setBuild = new()
        {
            entity = GetPlayerLocalIndex(0),
            emote = new() { Id = emote.AssetObject.Guid }
        };

        QuantumRunner.Default.Game.SendCommand(setBuild);
    }

    public void SetEmoteLeftOnPlayer(EmoteAsset emote)
    {
        CommandSetEmoteLeft setBuild = new()
        {
            entity = GetPlayerLocalIndex(0),
            emote = new() { Id = emote.AssetObject.Guid }
        };

        QuantumRunner.Default.Game.SendCommand(setBuild);
    }

    public void SetEmoteRightOnPlayer(EmoteAsset emote)
    {
        CommandSetEmoteRight setBuild = new()
        {
            entity = GetPlayerLocalIndex(0),
            emote = new() { Id = emote.AssetObject.Guid }
        };

        QuantumRunner.Default.Game.SendCommand(setBuild);
    }

    public void SetEyesOnPlayer(EyesAsset eyes)
    {
        CommandSetEyes setBuild = new()
        {
            entity = GetPlayerLocalIndex(0),
            eyes = new() { Id = eyes.AssetObject.Guid }
        };

        QuantumRunner.Default.Game.SendCommand(setBuild);
    }

    public void SetHairOnPlayer(HairAsset hair)
    {
        CommandSetHair setBuild = new()
        {
            entity = GetPlayerLocalIndex(0),
            hair = new() { Id = hair.AssetObject.Guid }
        };

        QuantumRunner.Default.Game.SendCommand(setBuild);
    }

    public void SetMainWeaponOnPlayer(SerializableWrapper<Weapon> weapon)
    {
        CommandSetMainWeapon setBuild = new()
        {
            entity = GetPlayerLocalIndex(0),
            weapon = weapon
        };

        QuantumRunner.Default.Game.SendCommand(setBuild);
    }

    public void SetSubOnPlayer(SerializableWrapper<Sub> sub)
    {
        CommandSetSub setBuild = new()
        {
            entity = GetPlayerLocalIndex(0),
            sub = sub
        };

        QuantumRunner.Default.Game.SendCommand(setBuild);
    }

    public void SetUltimateOnPlayer(UltimateAsset ultimate)
    {
        CommandSetUltimate setBuild = new()
        {
            entity = GetPlayerLocalIndex(0),
            ultimate = new() { Id = ultimate.AssetObject.Guid }
        };

        QuantumRunner.Default.Game.SendCommand(setBuild);
    }

    public void SetVoiceOnPlayer(VoiceAsset voice)
    {
        CommandSetVoice setBuild = new()
        {
            entity = GetPlayerLocalIndex(0),
            voice = new() { Id = voice.AssetObject.Guid }
        };

        QuantumRunner.Default.Game.SendCommand(setBuild);
    }

    public void SetOnPlayer(SerializableWrapper<Build> build, int playerIndex)
    {
        CommandSetBuild setBuild = new()
        {
            entity = GetPlayerLocalIndex(playerIndex),
            build = build
        };

        QuantumRunner.Default.Game.SendCommand(setBuild);
        PlayerStatController.Instance.HUDS[playerIndex].SetPlayerIcon(build.Icon);
    }

    public void SetOnPlayerDefault(SerializableWrapper<Build> build)
    {
        SetOnPlayer(build, 0);
    }
}
