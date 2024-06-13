using Extensions.Components.Miscellaneous;
using Extensions.Components.UI;
using GameResources.UI.Popup;
using Quantum;
using UnityEngine;
using UnityEngine.Events;

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

    private string _name = "Untitled";
    public void SetName(string name) => _name = name;
    public void ClearName() => _name = string.Empty;

    private string _description = string.Empty;
    public void SetDescription(string description) => _description = description;
    public void ClearDescription() => _description = string.Empty;

    public void Clear()
    {
        ClearTemplate();
        ClearPattern();
        ClearModifier1();
        ClearModifier2();
        ClearModifier3();
        ClearName();
        ClearDescription();
    }

    [SerializeField] private Popup _onSuccess;
    [SerializeField] private Popup _onFail;
    [SerializeField] private Popup _onNotEnoughCurrency;

    [SerializeField] private PopulateBase _populator;
    [SerializeField] private TMPro.TMP_Text _price;

    [SerializeField] private Transform _objParent;
    private GameObject _templateObj;

    [SerializeField] private UnityEvent _onSuccessEvent;

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

        if (!InventoryController.Instance.HasEnoughCurrency(_template.Price, _pattern?.Price, _modifier1?.Price, _modifier2?.Price, _modifier3?.Price))
        {
            PopupController.Instance.DisplayPopup(_onNotEnoughCurrency);
            return;
        }

        Apparel apparel = new()
        {
            Template = new AssetRefApparelTemplate() { Id = _template.AssetObject.Guid },
            Pattern = new AssetRefApparelPattern() { Id = _pattern ? _pattern.AssetObject.Guid : AssetGuid.Invalid }
        };
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

        SerializableWrapper<Apparel> serializable = new(apparel, _name, _description, AssetGuid.NewGuid(), System.DateTime.Now.Ticks, System.DateTime.Now.Ticks);
        serializable.SetIcon(_template.Icon.texture);

        Serializer.Save(serializable, serializable.Guid, GetPath());

        PopupController.Instance.DisplayPopup(_onSuccess);
        Clear();

        _onSuccessEvent.Invoke();
    }

    private SerializableWrapper<Apparel> _currentlySelected;
    public void SetCurrentlySelected(SerializableWrapper<Apparel> apparel) => _currentlySelected = apparel;

    public void InstanceDelete() => Instance.Delete();

    private void Delete()
    {
        string path = GetPath();
        Serializer.Delete($"{path}/{_currentlySelected.Guid}.json", path);

        Destroy(ApparelPopulator.ButtonFromItem(_currentlySelected));
        _populator.GetComponent<SelectAuto>().SetSelectedItem(SelectAuto.SelectType.First);
    }

    public void PreviewApparel(SerializableWrapper<Apparel> apparel)
    {
        PreviewTemplate(UnityDB.FindAsset<ApparelTemplateAsset>(apparel.Value.Template.Id));
        PreviewPattern(UnityDB.FindAsset<ApparelPatternAsset>(apparel.Value.Pattern.Id));
    }

    public void PreviewTemplate(ApparelTemplateAsset template)
    {
        if (_templateObj)
            Destroy(_templateObj);

        if (template.Preview)
            _templateObj = Instantiate(template.Preview, _objParent);

        int price = template.Price;
        _price.SetText($"${price}");

        _price.color = InventoryController.Instance.HasEnoughCurrency(price) ? Color.white : Color.red;
    }

    public void PreviewPattern(ApparelPatternAsset material)
    {
        /*if (_templateObj)
            Destroy(_templateObj);

        if (template.Weapon)
            _templateObj = Instantiate(template.Weapon, _objParent);*/

        int price = _template.Price + material.Price;
        _price.SetText($"${price}");

        _price.color = InventoryController.Instance.HasEnoughCurrency(price) ? Color.white : Color.red;
    }
}
