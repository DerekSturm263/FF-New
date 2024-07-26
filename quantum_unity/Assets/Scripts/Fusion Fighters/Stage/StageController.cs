using Extensions.Components.Miscellaneous;
using Quantum;
using UnityEngine;

public class StageController : Controller<StageController>
{
    private SerializableWrapper<Stage>? _stage;
    public SerializableWrapper<Stage>? Stage => _stage;

    public static string GetPath() => $"{Application.persistentDataPath}/SaveData/Custom/Stages";

    public SerializableWrapper<Stage> New()
    {
        Stage stage = new();
        return new(stage, "Untitled", "", System.DateTime.Now.Ticks, System.DateTime.Now.Ticks, AssetGuid.NewGuid());
    }

    public void Save(SerializableWrapper<Stage> stage)
    {
        stage.Save(GetPath());
    }

    public void Select(SerializableWrapper<Stage> stage, int playerIndex)
    {
        _stage = stage;
    }

    public void LoadFromAsset(StageAssetAsset stage)
    {
        Load(stage.Stage);
    }

    public void Load(Stage stage)
    {
        _stage = new(stage, "", "", 0, 0, AssetGuid.NewGuid());
        FindFirstObjectByType<QuantumRunnerLocalDebug>().OnStart.AddListener(_ => SendToSimulation());
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
            stage = _stage.GetValueOrDefault()
        };

        QuantumRunner.Default.Game.SendCommand(setStage);
    }

    public void UpdateDisplay(SerializableWrapper<Stage> stage)
    {
        FindFirstObjectByType<DisplayStageInfo>().UpdateDisplay(stage);
    }
}
