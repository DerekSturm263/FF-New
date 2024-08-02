public class EyesPopulator : PopulateAsset<EyesAsset>
{
    protected override string FilePath() => "DB/Assets/Build/Cosmetics/Eyes";

    protected override bool IsEquipped(EyesAsset item) => BuildController.Instance.CurrentlySelected.value.Cosmetics.Eyes.Eyes.Id == item.AssetObject.Guid;
}
