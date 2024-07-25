public class EmotePopulator : PopulateAsset<EmoteAsset>
{
    protected override string FilePath() => "DB/Assets/Build/Cosmetics/Emotes";

    protected override bool DoSpawn(EmoteAsset item) => InventoryController.Instance.HasUnlockedItem(item);
}
