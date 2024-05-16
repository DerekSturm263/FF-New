using Extensions.Components.Miscellaneous;
using Quantum;
using UnityEngine;

public class BuildManager : Controller<BuildManager>
{
    private EntityView _player;

    public static string GetPath() => $"{Application.persistentDataPath}/Builds";

    public override void Initialize()
    {
        base.Initialize();
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

    public void SaveOnPlayer()
    {
        if (!_player)
            _player = GameObject.FindGameObjectWithTag("Player").GetComponent<EntityView>();

        if (QuantumRunner.Default.Game.Frames.Verified.TryGet(_player.EntityRef, out Stats stats))
            Serializer.Save(stats.Build, stats.Build.SerializableData.Guid, GetPath());
    }

    public void SetOnPlayer(Build build)
    {
        if (!_player)
            _player = GameObject.FindGameObjectWithTag("Player").GetComponent<EntityView>();

        CommandSetBuild setBuild = new()
        {
            entity = _player.EntityRef,
            build = build
        };

        QuantumRunner.Default.Game.SendCommand(setBuild);
    }
}
