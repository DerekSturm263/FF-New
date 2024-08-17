public class MatchPopulator : PopulateAsset<MatchAssetAsset>
{
    protected override string FilePath() => "DB/Assets/Matches";

    protected override bool IsEquipped(MatchAssetAsset item) => false;
}
