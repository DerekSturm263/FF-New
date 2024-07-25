using Extensions.Components.Miscellaneous;
using Extensions.Components.UI;
using GameResources.UI.Popup;
using Quantum;
using UnityEngine;
using UnityEngine.Events;

public class WeaponController : Controller<WeaponController>
{
    public static GameObject TemplateObj;

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

    [SerializeField] private UnityEvent _onSuccessEvent;
    [SerializeField] private float _delayTime = 8;

    private bool _doAction;

    private SerializableWrapper<Weapon> _lastWeapon;
    [SerializeField] private UnityEvent<SerializableWrapper<Weapon>> _onSuccessEventDelayed;

    public static string GetPath() => $"{Application.persistentDataPath}/SaveData/Equipment/Weapons";

    private void Awake()
    {
        _instance = this;
    }

    public void SaveNew()
    {
        if (!_template || !_material)
        {
            PopupController.Instance.Spawn(_onFail);
            return;
        }

        if (!InventoryController.Instance.HasEnoughCurrency(_template.Price, _material.Price, _enhancer?.Price))
        {
            PopupController.Instance.Spawn(_onNotEnoughCurrency);
            return;
        }

        Weapon weapon = new()
        {
            Template = new AssetRefWeaponTemplate() { Id = _template.AssetObject.Guid },
            Material = new AssetRefWeaponMaterial() { Id = _material.AssetObject.Guid },
            Enhancer = new AssetRefWeaponEnhancer() { Id = _enhancer ? _enhancer.AssetObject.Guid : AssetGuid.Invalid }
        };

        InventoryController.Instance.LoseItem(_template, 1);
        InventoryController.Instance.LoseItem(_material, 1);

        if (_enhancer && _enhancer.AssetObject.Guid != AssetGuid.Invalid)
        {
            InventoryController.Instance.LoseItem(_enhancer, 1);
            InventoryController.Instance.LoseCurrency(_enhancer.Price);
        }
        
        InventoryController.Instance.LoseCurrency(_template.Price + _material.Price);

        SerializableWrapper<Weapon> serializable = new(weapon, _name, _description, AssetGuid.NewGuid(), System.DateTime.Now.Ticks, System.DateTime.Now.Ticks);
        serializable.Value.FileGuid = serializable.Guid;
        serializable.SetIcon(_template.Icon);

        Serializer.Save(serializable, serializable.Guid, GetPath());
        _lastWeapon = serializable;

        Clear();
        _onSuccessEvent.Invoke();

        _doAction = true;
        Invoke(nameof(InvokeEventDelay), _delayTime);
    }

    public void InvokeEventNoDelay()
    {
        InvokeEventDelay();
        _doAction = false;
    }

    private void InvokeEventDelay()
    {
        if (!_doAction)
            return;

        PopupController.Instance.Spawn(_onSuccess);
        _onSuccessEventDelayed.Invoke(_lastWeapon);
    }

    private SerializableWrapper<Weapon> _currentlySelected;
    public void SetCurrentlySelected(SerializableWrapper<Weapon> weapon) => _currentlySelected = weapon;

    public void InstanceDelete() => Instance.Delete();

    private void Delete()
    {
        InventoryController.Instance.GainItem(_currentlySelected.Value.Template.Id, 1);
        InventoryController.Instance.GainItem(_currentlySelected.Value.Material.Id, 1);
        InventoryController.Instance.GainItem(_currentlySelected.Value.Enhancer.Id, 1);

        string path = GetPath();
        Serializer.Delete($"{path}/{_currentlySelected.Guid}.json", path);

        Destroy(WeaponPopulator.ButtonFromItem(_currentlySelected));
        Extensions.Miscellaneous.Helper.Delay(0.1f, () => _populator.GetComponent<SelectAuto>().SetSelectedItem(SelectAuto.SelectType.First));
    }

    public void PreviewWeapon(SerializableWrapper<Weapon> weapon)
    {
        PreviewTemplate(UnityDB.FindAsset<WeaponTemplateAsset>(weapon.Value.Template.Id));
        PreviewMaterial(UnityDB.FindAsset<WeaponMaterialAsset>(weapon.Value.Material.Id));
        PreviewEnhancer(UnityDB.FindAsset<WeaponEnhancerAsset>(weapon.Value.Enhancer.Id));
    }

    public void PreviewTemplate(WeaponTemplateAsset template)
    {
        if (TemplateObj)
            Destroy(TemplateObj);

        if (template.Preview)
            TemplateObj = Instantiate(template.Preview, _objParent);

        if (_price.isActiveAndEnabled)
        {
            ulong price = template.Price;
            _price.SetText($"${price}");

            _price.color = InventoryController.Instance.HasEnoughCurrency(price) ? Color.white : Color.red;
        }
    }

    public void PreviewMaterial(WeaponMaterialAsset material)
    {
        /*if (_templateObj)
            Destroy(_templateObj);

        if (template.Weapon)
            _templateObj = Instantiate(template.Weapon, _objParent);*/

        if (_price.isActiveAndEnabled)
        {
            ulong price = _template.Price + material.Price;
            _price.SetText($"${price}");

            _price.color = InventoryController.Instance.HasEnoughCurrency(price) ? Color.white : Color.red;
        }
    }

    public void PreviewEnhancer(WeaponEnhancerAsset enhancer)
    {
        /*if (_templateObj)
            Destroy(_templateObj);

        if (template.Weapon)
            _templateObj = Instantiate(template.Weapon, _objParent);*/

        if (_price.isActiveAndEnabled)
        {
            ulong price = _template.Price + _material.Price + enhancer.Price;
            _price.SetText($"${price}");

            _price.color = InventoryController.Instance.HasEnoughCurrency(price) ? Color.white : Color.red;
        }
    }

    public void ClearPreview()
    {
        if (TemplateObj)
            Destroy(TemplateObj);
    }
}
