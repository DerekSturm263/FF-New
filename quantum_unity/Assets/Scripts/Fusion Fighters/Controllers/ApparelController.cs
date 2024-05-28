using Extensions.Components.Miscellaneous;
using Quantum;
using UnityEngine;

public class ApparelController : Controller<ApparelController>
{
    private ApparelTemplateAsset _template;
    public void SetTemplate(ApparelTemplateAsset template) => _template = template;

    private ApparelPatternAsset _pattern;
    public void SetEnhancer(ApparelPatternAsset pattern) => _pattern = pattern;

    public static string GetPath() => $"{Application.persistentDataPath}/Apparel";

    public void SaveNew()
    {
        Apparel apparel = new();

        apparel.SerializableData.Name = "New Apparel";
        apparel.SerializableData.Guid = AssetGuid.NewGuid();
        apparel.SerializableData.CreationDate = System.DateTime.Now.Ticks;
        apparel.SerializableData.LastEdittedDate = System.DateTime.Now.Ticks;

        apparel.Template = new AssetRefApparelTemplate() { Id = _template.AssetObject.Guid };
        apparel.Pattern = new AssetRefApparelPattern() { Id = _pattern.AssetObject.Guid };

        SerializableWrapper<Apparel> serializable = new(apparel);
        Serializer.Save(serializable, serializable.Value.SerializableData.Guid, GetPath());
    }
}
