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

        build.SerializableData.Name = "Untitled";
        build.SerializableData.Guid = AssetGuid.NewGuid();
        build.SerializableData.CreationDate = System.DateTime.Now.Ticks;
        build.SerializableData.LastEdittedDate = System.DateTime.Now.Ticks;

        return new(build);
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
}
