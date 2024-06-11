using Extensions.Components.Miscellaneous;
using Extensions.Components.UI;
using GameResources.UI.Popup;
using Quantum;
using UnityEngine;

public class ApparelController : Controller<ApparelController>
{
    private ApparelTemplateAsset _template;
    public void SetTemplate(ApparelTemplateAsset template) => _template = template;
    public void ClearTemplate() => _template = null;

    private ApparelPatternAsset _pattern;
    public void SetPattern(ApparelPatternAsset pattern) => _pattern = pattern;
    public void ClearPattern() => _pattern = null;

    private ApparelModifierAsset _modifier1;
    public void SetModifier1(ApparelModifierAsset modifier1) => _modifier1 = modifier1;
    public void ClearModifier1() => _modifier1 = null;

    private ApparelModifierAsset _modifier2;
    public void SetModifier2(ApparelModifierAsset modifier2) => _modifier2 = modifier2;
    public void ClearModifier2() => _modifier2 = null;

    private ApparelModifierAsset _modifier3;
    public void SetModifier3(ApparelModifierAsset modifier3) => _modifier3 = modifier3;
    public void ClearModifier3() => _modifier3 = null;

    public void Clear()
    {
        ClearTemplate();
        ClearPattern();
        ClearModifier1();
        ClearModifier2();
        ClearModifier3();
    }

    [SerializeField] private Popup _onSuccess;
    [SerializeField] private Popup _onFail;
    [SerializeField] private Popup _onNotEnoughCurrency;

    [SerializeField] private PopulateBase _populator;

    public static string GetPath() => $"{Application.persistentDataPath}/Apparel";

    private void Awake()
    {
        _instance = this;
    }

    public void SaveNew()
    {
        if (!_template)
        {
            PopupController.Instance.DisplayPopup(_onFail);
            return;
        }

        if (!Inventory.Instance.HasEnoughCurrency(_template.Price, _pattern?.Price, _modifier1?.Price, _modifier2?.Price, _modifier3?.Price))
        {
            PopupController.Instance.DisplayPopup(_onNotEnoughCurrency);
            return;
        }

        Apparel apparel = new();
        apparel.Template = new AssetRefApparelTemplate() { Id = _template.AssetObject.Guid };
        apparel.Pattern = new AssetRefApparelPattern() { Id = _pattern ? _pattern.AssetObject.Guid : AssetGuid.Invalid };
        apparel.Modifiers.Modifier1 = new AssetRefApparelModifier() { Id = _modifier1 ? _modifier1.AssetObject.Guid : AssetGuid.Invalid };
        apparel.Modifiers.Modifier2 = new AssetRefApparelModifier() { Id = _modifier2 ? _modifier2.AssetObject.Guid : AssetGuid.Invalid };
        apparel.Modifiers.Modifier3 = new AssetRefApparelModifier() { Id = _modifier3 ? _modifier3.AssetObject.Guid : AssetGuid.Invalid };

        InventoryController.Instance.UseCountableItem(_template);
        InventoryController.Instance.LoseCurrency(_template.Price);

        if (_pattern && _pattern.AssetObject.Guid != AssetGuid.Invalid)
        {
            InventoryController.Instance.UseCountableItem(_pattern);
            InventoryController.Instance.LoseCurrency(_pattern.Price);
        }

        if (_modifier1 && _modifier1.AssetObject.Guid != AssetGuid.Invalid)
        {
            InventoryController.Instance.UseCountableItem(_modifier1);
            InventoryController.Instance.LoseCurrency(_modifier1.Price);
        }

        if (_modifier2 && _modifier2.AssetObject.Guid != AssetGuid.Invalid)
        {
            InventoryController.Instance.UseCountableItem(_modifier2);
            InventoryController.Instance.LoseCurrency(_modifier2.Price);
        }

        if (_modifier3 && _modifier3.AssetObject.Guid != AssetGuid.Invalid)
        {
            InventoryController.Instance.UseCountableItem(_modifier3);
            InventoryController.Instance.LoseCurrency(_modifier3.Price);
        }

        SerializableWrapper<Apparel> serializable = new(apparel, "Untitled", "", AssetGuid.NewGuid(), System.DateTime.Now.Ticks, System.DateTime.Now.Ticks);
        serializable.SetIcon(_template.Icon.texture);

        Serializer.Save(serializable, serializable.Value.SerializableData.Guid, GetPath());

        PopupController.Instance.DisplayPopup(_onSuccess);
        Clear();
    }

    private SerializableWrapper<Apparel> _currentlySelected;
    public void SetCurrentlySelected(SerializableWrapper<Apparel> apparel) => _currentlySelected = apparel;

    public void InstanceDelete() => Instance.Delete();

    private void Delete()
    {
        string path = GetPath();
        Serializer.Delete($"{path}/{_currentlySelected.Value.SerializableData.Guid}.json", path);

        Destroy(ApparelPopulator.ButtonFromItem(_currentlySelected));
        _populator.GetComponent<SelectAuto>().SetSelectedItem(SelectAuto.SelectType.First);
    }
}
