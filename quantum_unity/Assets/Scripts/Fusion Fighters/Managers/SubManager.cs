using Extensions.Components.Miscellaneous;
using Quantum;
using UnityEngine;

public class SubManager : Controller<SubManager>
{
    public static string GetPath() => $"{Application.persistentDataPath}/Subs";

    public override void Initialize()
    {
        base.Initialize();
    }

    public Sub New()
    {
        Sub sub = new();

        sub.SerializableData.Name = "Untitled";
        sub.SerializableData.Guid = AssetGuid.NewGuid();
        sub.SerializableData.CreationDate = System.DateTime.Now.Ticks;
        sub.SerializableData.LastEdittedDate = System.DateTime.Now.Ticks;

        return sub;
    }

    public void Save(Sub sub)
    {
        Serializer.Save(sub, sub.SerializableData.Guid, GetPath());
    }
}
