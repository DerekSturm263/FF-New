public class AvatarPopulator : PopulateAsset<FFAvatarAsset>
{
    protected override string FilePath() => "DB/Assets/Build/Cosmetics/Avatars";

    protected override bool IsEquipped(FFAvatarAsset item) => BuildController.Instance.CurrentBuild.value.Cosmetics.Avatar.Avatar.Id == item.AssetObject.Guid;
}
