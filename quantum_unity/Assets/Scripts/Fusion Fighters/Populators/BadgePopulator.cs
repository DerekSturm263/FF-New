public class BadgePopulator : PopulateAsset<BadgeAsset>
{
    protected override string FilePath() => "DB/Assets/Build/Equipment/Badges";

    protected override bool IsEquipped(BadgeAsset item) => BuildController.Instance.CurrentBuild.value.Gear.Badge.Id == item.AssetObject.Guid;
}
