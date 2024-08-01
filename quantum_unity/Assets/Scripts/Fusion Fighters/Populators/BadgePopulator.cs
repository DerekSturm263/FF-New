public class BadgePopulator : PopulateAsset<BadgeAsset>
{
    protected override string FilePath() => "DB/Assets/Build/Equipment/Badges";

    protected override bool DoSpawn(BadgeAsset item) => InventoryController.Instance.HasUnlockedItem(item);

    protected override bool HasEquipped(BadgeAsset item) => BuildController.Instance.CurrentlySelected.value.Equipment.Badge.Id == item.AssetObject.Guid;
}
