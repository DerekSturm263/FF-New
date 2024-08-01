public class HairPopulator : PopulateAsset<HairAsset>
{
    protected override string FilePath() => "DB/Assets/Build/Cosmetics/Hair";

    protected override bool DoSpawn(HairAsset item) => InventoryController.Instance.HasUnlockedItem(item);

    protected override bool HasEquipped(HairAsset item) => BuildController.Instance.CurrentlySelected.value.Cosmetics.Hair.Hair.Id == item.AssetObject.Guid;
}
