using Extensions.Components.Miscellaneous;
using Quantum;
using UnityEngine;

public class StageManager : Controller<StageManager>
{
    public static string GetPath() => $"{Application.persistentDataPath}/Stages";

    public override void Initialize()
    {
        base.Initialize();
    }

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
}
