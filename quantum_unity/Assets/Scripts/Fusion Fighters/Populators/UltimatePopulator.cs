public class UltimatePopulator : PopulateAsset<UltimateAsset>
{
    protected override string FilePath() => "DB/Assets/Build/Equipment/Ultimates";

    protected override bool DoSpawn(UltimateAsset item) => item.IncludeInLists && InventoryController.Instance.HasUnlockedItem(item);

    protected override bool HasEquipped(UltimateAsset item) => BuildController.Instance.CurrentlySelected.value.Equipment.Ultimate.Id == item.AssetObject.Guid;
}
