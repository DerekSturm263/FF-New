using Extensions.Components.Miscellaneous;
using Quantum;
using UnityEngine;

public class ApparelManager : Controller<ApparelManager>
{
    public static string GetPath() => $"{Application.persistentDataPath}/Apparel";

    public override void Initialize()
    {
        base.Initialize();
    }

    public Apparel New()
    {
        Apparel apparel = new();

        apparel.SerializableData.Name = "Untitled";
        apparel.SerializableData.Guid = AssetGuid.NewGuid();
        apparel.SerializableData.CreationDate = System.DateTime.Now.Ticks;
        apparel.SerializableData.LastEdittedDate = System.DateTime.Now.Ticks;

        return apparel;
    }

    public void Save(Apparel apparel)
    {
        Serializer.Save(apparel, apparel.SerializableData.Guid, GetPath());
    }
}
