public class BadgePopulator : PopulateAsset<BadgeAsset>
{
    protected override string FilePath() => "DB/Assets/Build/Equipment/Badges";

    protected override bool DoSpawn(BadgeAsset item) => InventoryController.Instance.HasItem(item);
}
