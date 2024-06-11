public class HairPopulator : PopulateAsset<HairAsset>
{
    protected override string FilePath() => "DB/Assets/Build/Cosmetics/Hair";

    protected override bool DoSpawn(HairAsset item) => InventoryController.Instance.HasItem(item);
}
