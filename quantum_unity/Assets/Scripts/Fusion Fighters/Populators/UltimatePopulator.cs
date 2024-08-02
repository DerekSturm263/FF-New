public class UltimatePopulator : PopulateAsset<UltimateAsset>
{
    protected override string FilePath() => "DB/Assets/Build/Equipment/Ultimates";

    protected override bool IsEquipped(UltimateAsset item) => BuildController.Instance.CurrentlySelected.value.Equipment.Ultimate.Id == item.AssetObject.Guid;
}
