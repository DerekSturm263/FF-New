using Extensions.Components.Miscellaneous;
using Extensions.Components.UI;
using GameResources.UI.Popup;
using Quantum;
using UnityEngine;

public class WeaponController : Controller<WeaponController>
{
    private WeaponTemplateAsset _template;
    public void SetTemplate(WeaponTemplateAsset template) => _template = template;
    public void ClearTemplate() => _template = null;

    private WeaponMaterialAsset _material;
    public void SetMaterial(WeaponMaterialAsset material) => _material = material;
    public void ClearMaterial() => _material = null;

    private WeaponEnhancerAsset _enhancer1;
    public void SetEnhancer1(WeaponEnhancerAsset enhancer1) => _enhancer1 = enhancer1;
    public void ClearEnhancer1() => _enhancer1 = null;

    private WeaponEnhancerAsset _enhancer2;
    public void SetEnhancer2(WeaponEnhancerAsset enhancer2) => _enhancer2 = enhancer2;
    public void ClearEnhancer2() => _enhancer2 = null;

    public void Clear()
    {
        ClearTemplate();
        ClearMaterial();
        ClearEnhancer1();
        ClearEnhancer2();
    }

    [SerializeField] private Popup _onSuccess;
    [SerializeField] private Popup _onFail;
    [SerializeField] private Popup _onNotEnoughCurrency;

    [SerializeField] private PopulateBase _populator;

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

        if (!InventoryController.Instance.HasEnoughCurrency(_template.Price, _material.Price, _enhancer1?.Price, _enhancer2?.Price))
        {
            PopupController.Instance.DisplayPopup(_onNotEnoughCurrency);
            return;
        }

        Weapon weapon = new();
        weapon.Template = new AssetRefWeaponTemplate() { Id = _template.AssetObject.Guid };
        weapon.Material = new AssetRefWeaponMaterial() { Id = _material.AssetObject.Guid };
        weapon.Enhancers.Enhancer1 = new AssetRefWeaponEnhancer() { Id = _enhancer1 ? _enhancer1.AssetObject.Guid : AssetGuid.Invalid };
        weapon.Enhancers.Enhancer2 = new AssetRefWeaponEnhancer() { Id = _enhancer2 ? _enhancer2.AssetObject.Guid : AssetGuid.Invalid };

        InventoryController.Instance.UseCountableItem(_template);
        InventoryController.Instance.UseCountableItem(_material);

        if (_enhancer1 && _enhancer1.AssetObject.Guid != AssetGuid.Invalid)
        {
            InventoryController.Instance.UseCountableItem(_enhancer1);
            InventoryController.Instance.LoseCurrency(_enhancer1.Price);
        }

        if (_enhancer2 && _enhancer2.AssetObject.Guid != AssetGuid.Invalid)
        {
            InventoryController.Instance.UseCountableItem(_enhancer2);
            InventoryController.Instance.LoseCurrency(_enhancer2.Price);
        }
        
        InventoryController.Instance.LoseCurrency(_template.Price + _material.Price);

        SerializableWrapper<Weapon> serializable = new(weapon, "Untitled", "", AssetGuid.NewGuid(), System.DateTime.Now.Ticks, System.DateTime.Now.Ticks);
        serializable.SetIcon(_template.Icon.texture);

        Serializer.Save(serializable, serializable.Guid, GetPath());

        PopupController.Instance.DisplayPopup(_onSuccess);
        Clear();
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
}
