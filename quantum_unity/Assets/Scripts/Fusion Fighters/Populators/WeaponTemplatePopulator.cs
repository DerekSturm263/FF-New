public class WeaponTemplatePopulator : PopulateAsset<WeaponTemplateAsset>
{
    protected override string FilePath() => "DB/Assets/Build/Equipment/Weapons/Weapons/Templates";

    protected override bool DoSpawn(WeaponTemplateAsset item) => InventoryController.Instance.HasUnlockedItem(item);
}
