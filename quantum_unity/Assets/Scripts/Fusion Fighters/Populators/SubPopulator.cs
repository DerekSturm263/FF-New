public class SubPopulator : PopulateSerializable<Quantum.Sub, SubAssetAsset>
{
    protected override string BuiltInFilePath() => "DB/Assets/Build/Equipment/Weapons/Subs/Subs";
    protected override string CustomFilePath() => SubController.GetPath();

    protected override SerializableWrapper<Quantum.Sub> GetFromBuiltInAsset(SubAssetAsset asset) => asset.Sub;

    protected override bool IsEquipped(SerializableWrapper<Quantum.Sub> item) => BuildController.Instance.CurrentlySelected.value.Equipment.Weapons.SubWeapon.Equals(item.value);
    protected override bool IsNone(SerializableWrapper<Quantum.Sub> item) => !item.value.Template.Id.IsValid;
}
