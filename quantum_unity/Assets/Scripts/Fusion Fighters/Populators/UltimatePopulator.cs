public class UltimatePopulator : PopulateAsset<UltimateAsset>
{
    protected override string FilePath() => "DB/Assets/Build/Equipment/Ultimates";

    protected override bool DoSpawn(UltimateAsset item) => InventoryController.Instance.HasItem(item);
}
