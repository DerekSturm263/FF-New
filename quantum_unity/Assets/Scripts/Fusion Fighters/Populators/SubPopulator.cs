using UnityEngine;

public class SubPopulator : PopulateSerializable<Quantum.Sub, SubAssetAsset>
{
    [SerializeField] private SubAssetAsset _none;

    protected override string BuiltInFilePath() => "DB/Assets/Build/Equipment/Weapons/Subs/Subs";
    protected override string CustomFilePath() => SubController.GetPath();

    protected override SerializableWrapper<Quantum.Sub> GetFromBuiltInAsset(SubAssetAsset asset)
    {
        var item = asset.Sub;

        return item;
    }

    protected override bool IsEquipped(SerializableWrapper<Quantum.Sub> item) => BuildController.Instance.CurrentBuild.value.Equipment.Weapons.SubWeapon.Equals(item.value);
    protected override bool IsNone(SerializableWrapper<Quantum.Sub> item) => item.value.Equals(_none.Sub.value);
}
