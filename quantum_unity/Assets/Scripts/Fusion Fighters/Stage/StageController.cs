using Extensions.Components.Miscellaneous;
using Quantum;
using UnityEngine;

public class StageController : Controller<StageController>
{
    private Stage _stage;
    public Stage Stage => _stage;

    public static string GetPath() => $"{Application.persistentDataPath}/Stages";

    public Stage New()
    {
        Stage stage = new();

        stage.SerializableData.Name = "Untitled";
        stage.SerializableData.Guid = AssetGuid.NewGuid();
        stage.SerializableData.CreationDate = System.DateTime.Now.Ticks;
        stage.SerializableData.LastEdittedDate = System.DateTime.Now.Ticks;

        return stage;
    }

    public void Save(Stage stage)
    {
        Serializer.Save(stage, stage.SerializableData.Guid, GetPath());
    }

    public void Select(Stage stage)
    {
        _stage = stage;
    }

    public void SendToSimulation(Stage stage)
    {
        CommandSetStage setStage = new()
        {
            stage = _stage
        };

        QuantumRunner.Default.Game.SendCommand(setStage);
    }
}
