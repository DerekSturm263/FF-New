using Extensions.Components.Miscellaneous;
using GameResources.UI.Popup;
using Quantum;
using UnityEngine;

public class ApparelController : Controller<ApparelController>
{
    private ApparelTemplateAsset _template;
    public void SetTemplate(ApparelTemplateAsset template) => _template = template;

    private ApparelPatternAsset _pattern;
    public void SetEnhancer(ApparelPatternAsset pattern) => _pattern = pattern;

    [SerializeField] private Popup _onSuccess;
    [SerializeField] private Popup _onFail;

    public static string GetPath() => $"{Application.persistentDataPath}/Apparel";

    public void SaveNew()
    {
        if (!_template)
        {
            PopupController.Instance.DisplayPopup(_onFail);
            return;
        }

        Apparel apparel = new();

        apparel.SerializableData.Name = "New Apparel";
        apparel.SerializableData.Guid = AssetGuid.NewGuid();
        apparel.SerializableData.CreationDate = System.DateTime.Now.Ticks;
        apparel.SerializableData.LastEdittedDate = System.DateTime.Now.Ticks;

        apparel.Template = new AssetRefApparelTemplate() { Id = _template.AssetObject.Guid };
        apparel.Pattern = new AssetRefApparelPattern() { Id = _pattern ? _pattern.AssetObject.Guid : AssetGuid.Invalid };

        SerializableWrapper<Apparel> serializable = new(apparel);
        serializable.SetIcon(_template.Icon.texture);

        Serializer.Save(serializable, serializable.Value.SerializableData.Guid, GetPath());

        PopupController.Instance.DisplayPopup(_onSuccess);
    }
}
