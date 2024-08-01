public class WeaponTemplatePopulator : PopulateAsset<WeaponTemplateAsset>
{
    protected override string FilePath() => "DB/Assets/Build/Equipment/Weapons/Weapons/Templates";

    protected override bool DoSpawn(WeaponTemplateAsset item) => item.IncludeInLists && InventoryController.Instance.HasUnlockedItem(item);

    protected override bool HasEquipped(WeaponTemplateAsset item) => false;
}
