using Extensions.Components.Miscellaneous;
using Quantum;
using System.Collections.Generic;
using UnityEngine;

public class BuildController : Controller<BuildController>
{
    public static string GetPath() => $"{Application.persistentDataPath}/Builds";

    private readonly Dictionary<int, EntityRef> _indicesToPlayers = new();

    private EntityRef GetPlayer(int playerIndex)
    {
        if (!_indicesToPlayers.ContainsKey(playerIndex))
        {
            int i = 0;
            foreach (var playerLink in QuantumRunner.Default.Game.Frames.Verified.GetComponentIterator<PlayerLink>())
            {
                if (i == playerIndex)
                {
                    _indicesToPlayers[playerIndex] = playerLink.Component.Entity;
                    break;
                }

                ++i;
            }
        }

        return _indicesToPlayers[playerIndex];
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
        if (QuantumRunner.Default.Game.Frames.Verified.TryGet(GetPlayer(playerIndex), out Stats stats))
            Serializer.Save(stats.Build, stats.Build.SerializableData.Guid, GetPath());
    }

    public void SetOnPlayer(SerializableWrapper<Build> build, int playerIndex)
    {
        CommandSetBuild setBuild = new()
        {
            entity = GetPlayer(playerIndex),
            build = build
        };

        QuantumRunner.Default.Game.SendCommand(setBuild);
        PlayerStatController.Instance.HUDS[playerIndex].SetPlayerIcon(build.Icon);
    }
}
