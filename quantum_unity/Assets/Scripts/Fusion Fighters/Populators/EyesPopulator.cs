public class EyesPopulator : PopulateAsset<EyesAsset>
{
    protected override string FilePath() => "DB/Assets/Build/Cosmetics/Eyes";

    protected override bool DoSpawn(EyesAsset item) => InventoryController.Instance.HasUnlockedItem(item);

    protected override bool HasEquipped(EyesAsset item) => BuildController.Instance.CurrentlySelected.value.Cosmetics.Eyes.Eyes.Id == item.AssetObject.Guid;
}
