public class BuildPopulator : PopulateSerializable<Quantum.Build, BuildAssetAsset>
{
    protected override string BuiltInFilePath() => "DB/Assets/Build/Builds";
    protected override string CustomFilePath() => BuildController.GetPath();

    protected override SerializableWrapper<Quantum.Build> GetFromBuiltInAsset(BuildAssetAsset asset) => asset.Build;

    protected override bool IsEquipped(SerializableWrapper<Quantum.Build> item) => BuildController.Instance.CurrentBuild.Equals(item);
    protected override bool IsNone(SerializableWrapper<Quantum.Build> item) => false;
}
