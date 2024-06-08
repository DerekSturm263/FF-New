using Extensions.Components.Miscellaneous;
using GameResources.UI.Popup;
using Quantum;
using UnityEngine;

public class WeaponController : Controller<WeaponController>
{
    private WeaponTemplateAsset _template;
    public void SetTemplate(WeaponTemplateAsset template) => _template = template;

    private WeaponMaterialAsset _material;
    public void SetMaterial(WeaponMaterialAsset material) => _material = material;

    private WeaponEnhancerAsset _enhancer1;
    public void SetEnhancer1(WeaponEnhancerAsset enhancer1) => _enhancer1 = enhancer1;

    private WeaponEnhancerAsset _enhancer2;
    public void SetEnhancer2(WeaponEnhancerAsset enhancer2) => _enhancer2 = enhancer2;

    [SerializeField] private Popup _onSuccess;
    [SerializeField] private Popup _onFail;

    public static string GetPath() => $"{Application.persistentDataPath}/Weapons";

    public void SaveNew()
    {
        if (!_template || !_material)
        {
            PopupController.Instance.DisplayPopup(_onFail);
            return;
        }

        Weapon weapon = new();

        //weapon.SerializableData.Name = "New Weapon";
        weapon.SerializableData.Guid = AssetGuid.NewGuid();
        weapon.SerializableData.CreationDate = System.DateTime.Now.Ticks;
        weapon.SerializableData.LastEdittedDate = System.DateTime.Now.Ticks;

        weapon.Template = new AssetRefWeaponTemplate() { Id = _template.AssetObject.Guid };
        weapon.Material = new AssetRefWeaponMaterial() { Id = _material.AssetObject.Guid };
        weapon.Enhancers.Enhancer1 = new AssetRefWeaponEnhancer() { Id = _enhancer1 ? _enhancer1.AssetObject.Guid : AssetGuid.Invalid };
        weapon.Enhancers.Enhancer2 = new AssetRefWeaponEnhancer() { Id = _enhancer2 ? _enhancer2.AssetObject.Guid : AssetGuid.Invalid };

        SerializableWrapper<Weapon> serializable = new(weapon);
        serializable.SetIcon(_template.Icon.texture);

        Serializer.Save(serializable, serializable.Value.SerializableData.Guid, GetPath());

        PopupController.Instance.DisplayPopup(_onSuccess);
    }
}
