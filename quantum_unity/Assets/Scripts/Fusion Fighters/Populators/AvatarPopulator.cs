public class AvatarPopulator : PopulateAsset<FFAvatarAsset>
{
    protected override string FilePath() => "DB/Assets/Build/Cosmetics/Avatars";

    protected override bool DoSpawn(FFAvatarAsset item) => InventoryController.Instance.HasItem(item);
}
