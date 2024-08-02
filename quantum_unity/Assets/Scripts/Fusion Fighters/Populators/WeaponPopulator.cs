using UnityEngine;

public class WeaponPopulator : PopulateSerializable<Quantum.Weapon, WeaponAssetAsset>
{
    [SerializeField] private int _type;

    protected override string BuiltInFilePath() => "DB/Assets/Build/Equipment/Weapons/Weapons/Weapons";
    protected override string CustomFilePath() => WeaponController.GetPath();

    protected override SerializableWrapper<Quantum.Weapon> GetFromBuiltInAsset(WeaponAssetAsset asset) => asset.Weapon;

    protected override bool IsEquipped(SerializableWrapper<Quantum.Weapon> item)
    {
        bool isSame = _type switch
        {
            0 => BuildController.Instance.CurrentlySelected.value.Equipment.Weapons.MainWeapon.Equals(item.value),
            1 => BuildController.Instance.CurrentlySelected.value.Equipment.Weapons.AltWeapon.Equals(item.value),
            _ => false
        };

        return isSame;
    }
    protected override bool IsNone(SerializableWrapper<Quantum.Weapon> item) => !item.value.Template.Id.IsValid;
}
