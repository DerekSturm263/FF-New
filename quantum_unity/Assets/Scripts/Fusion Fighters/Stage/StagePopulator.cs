using Quantum.Types;
using System.Linq;

public class StagePopulator : PopulateSerializable<Quantum.Stage, StageAssetAsset>
{
    protected override string BuiltInFilePath() => "DB/Assets/Stage/Stages";
    protected override string CustomFilePath() => StageController.GetPath();

    protected override SerializableWrapper<Quantum.Stage> GetFromBuiltInAsset(StageAssetAsset asset)
    {
        var item = asset.Stage;
        item.SetIconForBuiltIn(asset.Icon);

        return item;
    }

    protected override bool IsEquipped(SerializableWrapper<Quantum.Stage> item) => ArrayHelper.All(RulesetController.Instance.CurrentRuleset.value.Stage.Stages).Any(stage => stage.Id == item.FileID);
    protected override bool IsNone(SerializableWrapper<Quantum.Stage> item) => false;
}
