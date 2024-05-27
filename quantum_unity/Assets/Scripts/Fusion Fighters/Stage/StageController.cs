using Extensions.Components.Miscellaneous;
using Quantum;
using UnityEngine;

public class StageController : Controller<StageController>
{
    private SerializableWrapper<Stage> _stage;
    public SerializableWrapper<Stage> Stage => _stage;

    public static string GetPath() => $"{Application.persistentDataPath}/Stages";

    public SerializableWrapper<Stage> New()
    {
        Stage stage = new();

        stage.SerializableData.Name = "Untitled";
        stage.SerializableData.Guid = AssetGuid.NewGuid();
        stage.SerializableData.CreationDate = System.DateTime.Now.Ticks;
        stage.SerializableData.LastEdittedDate = System.DateTime.Now.Ticks;

        return new(stage);
    }

    public void Save(SerializableWrapper<Stage> stage)
    {
        Serializer.Save(stage, stage.Value.SerializableData.Guid, GetPath());
    }

    public void Select(SerializableWrapper<Stage> stage)
    {
        _stage = stage;
    }

    public void ResetValue()
    {
        _stage = null;
    }

    public void SendToSimulation(QuantumGame game)
    {
        if (_stage is null)
            return;

        CommandSetStage setStage = new()
        {
            stage = _stage
        };

        game.SendCommand(setStage);
    }
}
