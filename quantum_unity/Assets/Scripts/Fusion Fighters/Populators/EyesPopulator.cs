public class EyesPopulator : PopulateAsset<EyesAsset>
{
    protected override string FilePath() => "DB/Assets/Build/Cosmetics/Eyes";

    protected override bool DoSpawn(EyesAsset item) => InventoryController.Instance.HasItem(item);
}
