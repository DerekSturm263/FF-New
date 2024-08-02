public class HairPopulator : PopulateAsset<HairAsset>
{
    protected override string FilePath() => "DB/Assets/Build/Cosmetics/Hair";

    protected override bool IsEquipped(HairAsset item) => BuildController.Instance.CurrentlySelected.value.Cosmetics.Hair.Hair.Id == item.AssetObject.Guid;
}
