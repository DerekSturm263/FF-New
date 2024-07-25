using Extensions.Components.UI;
using Quantum;
using UnityEngine;

using Type = SerializableWrapper<Quantum.Weapon>;

public class DisplayWeaponInfo : DisplayBase
{

    [SerializeField] private DisplayWeaponTemplateInfo _component1;
    [SerializeField] private DisplayWeaponMaterialInfo _component2;
    [SerializeField] private DisplayWeaponEnhancerInfo _component3;

    [SerializeField] private AssetRefWeaponEnhancer _none;

    public override void UpdateDisplayOnEnable() => UpdateDisplay();

    protected Type GetValue() => default;

    public void UpdateDisplay(Type item)
    {
        _component1.UpdateDisplay(UnityDB.FindAsset<WeaponTemplateAsset>(item.value.Template.Id));
        _component2.UpdateDisplay(UnityDB.FindAsset<WeaponMaterialAsset>(item.value.Material.Id));

        if (item.value.Enhancer.Id.IsValid)
            _component3.UpdateDisplay(UnityDB.FindAsset<WeaponEnhancerAsset>(item.value.Enhancer.Id));
        else
            _component3.UpdateDisplay(UnityDB.FindAsset<WeaponEnhancerAsset>(_none.Id));
    }

    public void UpdateDisplay() => UpdateDisplay(GetValue());
}
