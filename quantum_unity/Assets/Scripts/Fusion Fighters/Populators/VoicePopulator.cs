public class VoicePopulator : PopulateAsset<VoiceAsset>
{
    protected override string FilePath() => "DB/Assets/Build/Cosmetics/Voices";

    protected override bool DoSpawn(VoiceAsset item) => InventoryController.Instance.HasItem(item);
}
