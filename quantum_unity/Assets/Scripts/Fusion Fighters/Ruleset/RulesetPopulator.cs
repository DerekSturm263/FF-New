public class RulesetPopulator : PopulateSerializable<Quantum.Ruleset, RulesetAssetAsset>
{
    protected override string BuiltInFilePath() => "DB/Assets/Ruleset/Rulesets";
    protected override string CustomFilePath() => RulesetController.GetPath();

    protected override SerializableWrapper<Quantum.Ruleset> GetFromBuiltInAsset(RulesetAssetAsset asset) => asset.Ruleset;

    protected override bool IsEquipped(SerializableWrapper<Quantum.Ruleset> item) => RulesetController.Instance.CurrentRuleset.Equals(item);
    protected override bool IsNone(SerializableWrapper<Quantum.Ruleset> item) => false;
}
