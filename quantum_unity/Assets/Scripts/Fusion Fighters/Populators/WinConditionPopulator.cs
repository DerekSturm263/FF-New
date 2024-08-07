public class WinConditionPopulator : PopulateAsset<WinConditionAsset>
{
    protected override string FilePath() => "DB/Assets/Ruleset/Win Conditions";

    protected override bool IsEquipped(WinConditionAsset item) => false;
}
