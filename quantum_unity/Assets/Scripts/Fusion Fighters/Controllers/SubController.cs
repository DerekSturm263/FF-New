using Extensions.Components.Miscellaneous;
using Quantum;
using UnityEngine;

public class SubController : Controller<SubController>
{
    private SubTemplateAsset _template;
    public void SetTemplate(SubTemplateAsset template) => _template = template;

    private SubEnhancerAsset _enhancer;
    public void SetEnhancer(SubEnhancerAsset enhancer) => _enhancer = enhancer;

    public static string GetPath() => $"{Application.persistentDataPath}/Subs";

    public void SaveNew()
    {
        Sub sub = new();

        sub.SerializableData.Name = "New Sub";
        sub.SerializableData.Guid = AssetGuid.NewGuid();
        sub.SerializableData.CreationDate = System.DateTime.Now.Ticks;
        sub.SerializableData.LastEdittedDate = System.DateTime.Now.Ticks;

        sub.Template = new AssetRefSubTemplate() { Id = _template.AssetObject.Guid };
        sub.Enhancers.Enhancer1 = new AssetRefSubEnhancer() { Id = _enhancer.AssetObject.Guid };

        SerializableWrapper<Sub> serializable = new(sub);
        Serializer.Save(serializable, serializable.Value.SerializableData.Guid, GetPath());
    }
}
