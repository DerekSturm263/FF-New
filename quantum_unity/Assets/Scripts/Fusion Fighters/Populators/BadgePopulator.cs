public class BadgePopulator : PopulateAsset<BadgeAsset>
{
    protected override string FilePath() => "DB/Assets/Build/Equipment/Badges";

    protected override bool IsEquipped(BadgeAsset item) => BuildController.Instance.CurrentlySelected.value.Equipment.Badge.Id == item.AssetObject.Guid;
}
