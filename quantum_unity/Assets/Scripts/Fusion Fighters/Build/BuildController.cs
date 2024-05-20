using Extensions.Components.Miscellaneous;
using Quantum;
using UnityEngine;

public class BuildController : Controller<BuildController>
{
    public static string GetPath() => $"{Application.persistentDataPath}/Builds";

    private EntityRef GetPlayer(int playerIndex)
    {
        PlayerRef? playerRef = QuantumRunner.Default.Game.Frames.Verified.ActorIdToFirstPlayer(playerIndex);
        
        if (playerRef.HasValue)
            return /*QuantumRunner.Default.Game.Frames.Verified.Value*/EntityRef.None;
        else
            return EntityRef.None;
    }

    public Build New()
    {
        Build build = new();

        build.SerializableData.Name = "Untitled";
        build.SerializableData.Guid = AssetGuid.NewGuid();
        build.SerializableData.CreationDate = System.DateTime.Now.Ticks;
        build.SerializableData.LastEdittedDate = System.DateTime.Now.Ticks;

        return build;
    }

    public void Save(Build build)
    {
        Serializer.Save(build, build.SerializableData.Guid, GetPath());
    }

    public void SaveOnPlayer(int playerIndex)
    {
        if (QuantumRunner.Default.Game.Frames.Verified.TryGet(GetPlayer(playerIndex), out Stats stats))
            Serializer.Save(stats.Build, stats.Build.SerializableData.Guid, GetPath());
    }

    public void SetOnPlayer(Build build, int playerIndex)
    {
        CommandSetBuild setBuild = new()
        {
            entity = GetPlayer(playerIndex),
            build = build
        };

        QuantumRunner.Default.Game.SendCommand(setBuild);
    }
}
