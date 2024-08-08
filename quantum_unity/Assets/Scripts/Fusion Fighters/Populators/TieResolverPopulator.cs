public class TieResolverPopulator : PopulateAsset<TieResolverAsset>
{
    protected override string FilePath() => "DB/Assets/Ruleset/Tie Resolvers";

    protected override bool IsEquipped(TieResolverAsset item) => RulesetController.Instance.CurrentRuleset.value.Match.TieResolver.Id == item.AssetObject.Guid;
}
