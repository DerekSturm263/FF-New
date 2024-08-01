public class AvatarPopulator : PopulateAsset<FFAvatarAsset>
{
    protected override string FilePath() => "DB/Assets/Build/Cosmetics/Avatars";

    protected override bool DoSpawn(FFAvatarAsset item) => InventoryController.Instance.HasUnlockedItem(item);

    protected override bool HasEquipped(FFAvatarAsset item) => BuildController.Instance.CurrentlySelected.value.Cosmetics.Avatar.Avatar.Id == item.AssetObject.Guid;
}
