public class EyesPopulator : PopulateAsset<EyesAsset>
{
    protected override string FilePath() => "DB/Assets/Build/Cosmetics/Eyes";

    protected override bool IsEquipped(EyesAsset item) => BuildController.Instance.CurrentBuild.value.Frame.Eyes.Eyes.Id == item.AssetObject.Guid;
}
