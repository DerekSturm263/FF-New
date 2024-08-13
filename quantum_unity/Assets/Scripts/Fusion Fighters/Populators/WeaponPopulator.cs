using UnityEngine;

public class WeaponPopulator : PopulateSerializable<Quantum.Weapon, WeaponAssetAsset>
{
    [SerializeField] private WeaponAssetAsset _none;
    [SerializeField] private int _type;

    protected override string BuiltInFilePath() => "DB/Assets/Build/Equipment/Weapons/Weapons/Weapons";
    protected override string CustomFilePath() => WeaponController.GetPath();

    protected override SerializableWrapper<Quantum.Weapon> GetFromBuiltInAsset(WeaponAssetAsset asset)
    {
        var item = asset.Weapon;

        return item;
    }

    protected override bool IsEquipped(SerializableWrapper<Quantum.Weapon> item)
    {
        bool isSame = _type switch
        {
            0 => BuildController.Instance.CurrentBuild.value.Gear.MainWeapon.Equals(item.value),
            1 => BuildController.Instance.CurrentBuild.value.Gear.AltWeapon.Equals(item.value),
            _ => false
        };

        return isSame;
    }
    protected override bool IsNone(SerializableWrapper<Quantum.Weapon> item) => item.value.Equals(_none.Weapon.value);

    protected override bool GiveEvents(SerializableWrapper<Quantum.Weapon> item)
    {
        if (IsNone(item))
            return true;

        if (_type == 0)
            return BuildController.Instance.CurrentBuild.value.Gear.AltWeapon.Template.Id != item.value.Template.Id;
        else
            return BuildController.Instance.CurrentBuild.value.Gear.MainWeapon.Template.Id != item.value.Template.Id;
    }
}
