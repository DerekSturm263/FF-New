public class WinConditionPopulator : PopulateAsset<WinConditionAsset>
{
    protected override string FilePath() => "DB/Assets/Ruleset/Win Conditions";

    protected override bool IsEquipped(WinConditionAsset item) => RulesetController.Instance.CurrentRuleset.value.Match.WinCondition.Id == item.AssetObject.Guid;
}
