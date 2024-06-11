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
        return new(stage, "Untitled", "", AssetGuid.NewGuid(), System.DateTime.Now.Ticks, System.DateTime.Now.Ticks);
    }

    public void Save(SerializableWrapper<Stage> stage)
    {
        Serializer.Save(stage, stage.Value.SerializableData.Guid, GetPath());
    }

    public void Select(SerializableWrapper<Stage> stage, int playerIndex)
    {
        _stage = stage;
    }

    public void LoadFromAsset(StageAssetAsset stage)
    {
        _stage = stage.Stage;
        FindFirstObjectByType<QuantumRunnerLocalDebug>().OnStartDeferred.AddListener(_ => SendToSimulation());
    }

    public void Load(Stage stage)
    {
        _stage = new(stage);
        FindFirstObjectByType<QuantumRunnerLocalDebug>().OnStartDeferred.AddListener(_ => SendToSimulation());
    }

    public void ResetValue()
    {
        _stage = null;
    }

    public void SendToSimulation()
    {
        if (_stage is null)
            return;

        CommandSetStage setStage = new()
        {
            stage = _stage
        };

        QuantumRunner.Default.Game.SendCommand(setStage);
    }
}
