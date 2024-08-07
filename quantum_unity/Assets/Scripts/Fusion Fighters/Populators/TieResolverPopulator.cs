public class TieResolverPopulator : PopulateAsset<TieResolverAsset>
{
    protected override string FilePath() => "DB/Assets/Ruleset/Tie Resolvers";

    protected override bool IsEquipped(TieResolverAsset item) => false;
}
