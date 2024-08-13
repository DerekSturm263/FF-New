public class HairPopulator : PopulateAsset<HairAsset>
{
    protected override string FilePath() => "DB/Assets/Build/Cosmetics/Hair";

    protected override bool IsEquipped(HairAsset item) => BuildController.Instance.CurrentBuild.value.Frame.Hair.Hair.Id == item.AssetObject.Guid;
}
