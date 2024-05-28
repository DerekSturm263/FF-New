using Extensions.Components.Miscellaneous;
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

    public static string GetPath() => $"{Application.persistentDataPath}/Weapons";

    public void SaveNew()
    {
        Weapon weapon = new();

        //weapon.SerializableData.Name = "New Weapon";
        weapon.SerializableData.Guid = AssetGuid.NewGuid();
        weapon.SerializableData.CreationDate = System.DateTime.Now.Ticks;
        weapon.SerializableData.LastEdittedDate = System.DateTime.Now.Ticks;

        weapon.Template = new AssetRefWeaponTemplate() { Id = _template.AssetObject.Guid };
        weapon.Material = new AssetRefWeaponMaterial() { Id = _material.AssetObject.Guid };
        weapon.Enhancers.Enhancer1 = new AssetRefWeaponEnhancer() { Id = _enhancer1.AssetObject.Guid };
        weapon.Enhancers.Enhancer2 = new AssetRefWeaponEnhancer() { Id = _enhancer2.AssetObject.Guid };

        SerializableWrapper<Weapon> serializable = new(weapon);
        Serializer.Save(serializable, serializable.Value.SerializableData.Guid, GetPath());
    }
}
