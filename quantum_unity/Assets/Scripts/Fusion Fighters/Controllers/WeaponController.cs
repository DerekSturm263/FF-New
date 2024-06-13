using Codice.Utils;
using Extensions.Components.Miscellaneous;
using Extensions.Components.UI;
using GameResources.UI.Popup;
using Quantum;
using UnityEngine;
using UnityEngine.Events;

public class WeaponController : Controller<WeaponController>
{
    private WeaponTemplateAsset _template;
    public void SetTemplate(WeaponTemplateAsset template) => _template = template;
    public void ClearTemplate() => _template = null;

    private WeaponMaterialAsset _material;
    public void SetMaterial(WeaponMaterialAsset material) => _material = material;
    public void ClearMaterial() => _material = null;

    private WeaponEnhancerAsset _enhancer;
    public void SetEnhancer(WeaponEnhancerAsset enhancer) => _enhancer = enhancer;
    public void ClearEnhancer() => _enhancer = null;

    private string _name = "Untitled";
    public void SetName(string name) => _name = name;
    public void ClearName() => _name = string.Empty;

    private string _description = string.Empty;
    public void SetDescription(string description) => _description = description;
    public void ClearDescription() => _description = string.Empty;

    public void Clear()
    {
        ClearTemplate();
        ClearMaterial();
        ClearEnhancer();
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
    [SerializeField] private UnityEvent _onSuccessEventDelayed;

    public static string GetPath() => $"{Application.persistentDataPath}/Weapons";

    private void Awake()
    {
        _instance = this;
    }

    public void SaveNew()
    {
        if (!_template || !_material)
        {
            PopupController.Instance.DisplayPopup(_onFail);
            return;
        }

        if (!InventoryController.Instance.HasEnoughCurrency(_template.Price, _material.Price, _enhancer?.Price))
        {
            PopupController.Instance.DisplayPopup(_onNotEnoughCurrency);
            return;
        }

        Weapon weapon = new()
        {
            Template = new AssetRefWeaponTemplate() { Id = _template.AssetObject.Guid },
            Material = new AssetRefWeaponMaterial() { Id = _material.AssetObject.Guid }
        };
        weapon.Enhancers.Enhancer1 = new AssetRefWeaponEnhancer() { Id = _enhancer ? _enhancer.AssetObject.Guid : AssetGuid.Invalid };

        InventoryController.Instance.UseCountableItem(_template);
        InventoryController.Instance.UseCountableItem(_material);

        if (_enhancer && _enhancer.AssetObject.Guid != AssetGuid.Invalid)
        {
            InventoryController.Instance.UseCountableItem(_enhancer);
            InventoryController.Instance.LoseCurrency(_enhancer.Price);
        }
        
        InventoryController.Instance.LoseCurrency(_template.Price + _material.Price);

        SerializableWrapper<Weapon> serializable = new(weapon, _name, _description, AssetGuid.NewGuid(), System.DateTime.Now.Ticks, System.DateTime.Now.Ticks);
        serializable.SetIcon(_template.Icon.texture);

        Serializer.Save(serializable, serializable.Guid, GetPath());

        Clear();

        _onSuccessEvent.Invoke();
        Extensions.Miscellaneous.Helper.Delay(7, () => PopupController.Instance.DisplayPopup(_onSuccess));
    }

    private SerializableWrapper<Weapon> _currentlySelected;
    public void SetCurrentlySelected(SerializableWrapper<Weapon> weapon) => _currentlySelected = weapon;

    public void InstanceDelete() => Instance.Delete();

    private void Delete()
    {
        string path = GetPath();
        Serializer.Delete($"{path}/{_currentlySelected.Guid}.json", path);

        Destroy(WeaponPopulator.ButtonFromItem(_currentlySelected));
        _populator.GetComponent<SelectAuto>().SetSelectedItem(SelectAuto.SelectType.First);
    }

    public void PreviewWeapon(SerializableWrapper<Weapon> weapon)
    {
        PreviewTemplate(UnityDB.FindAsset<WeaponTemplateAsset>(weapon.Value.Template.Id));
        PreviewMaterial(UnityDB.FindAsset<WeaponMaterialAsset>(weapon.Value.Material.Id));
        PreviewEnhancer(UnityDB.FindAsset<WeaponEnhancerAsset>(weapon.Value.Enhancers.Enhancer1.Id));
    }

    public void PreviewTemplate(WeaponTemplateAsset template)
    {
        if (_templateObj)
            Destroy(_templateObj);

        if (template.Preview)
            _templateObj = Instantiate(template.Preview, _objParent);

        int price = template.Price;
        _price.SetText($"${price}");

        _price.color = InventoryController.Instance.HasEnoughCurrency(price) ? Color.white : Color.red;
    }

    public void PreviewMaterial(WeaponMaterialAsset material)
    {
        /*if (_templateObj)
            Destroy(_templateObj);

        if (template.Weapon)
            _templateObj = Instantiate(template.Weapon, _objParent);*/

        int price = _template.Price + material.Price;
        _price.SetText($"${price}");

        _price.color = InventoryController.Instance.HasEnoughCurrency(price) ? Color.white : Color.red;
    }

    public void PreviewEnhancer(WeaponEnhancerAsset enhancer)
    {
        /*if (_templateObj)
            Destroy(_templateObj);

        if (template.Weapon)
            _templateObj = Instantiate(template.Weapon, _objParent);*/

        int price = _template.Price + _material.Price + enhancer.Price;
        _price.SetText($"${price}");

        _price.color = InventoryController.Instance.HasEnoughCurrency(price) ? Color.white : Color.red;
    }
}
