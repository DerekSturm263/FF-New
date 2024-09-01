public class StagePickerPopulator : PopulateAsset<StagePickerAsset>
{
    protected override string FilePath() => "DB/Assets/Ruleset/Stage Pickers";

    protected override bool IsEquipped(StagePickerAsset item) => RulesetController.Instance.CurrentRuleset.value.Stage.StagePicker.Id == item.AssetObject.Guid;
}
