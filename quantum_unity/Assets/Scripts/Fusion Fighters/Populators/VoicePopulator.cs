public class VoicePopulator : PopulateAsset<VoiceAsset>
{
    protected override string FilePath() => "DB/Assets/Build/Cosmetics/Voices";

    protected override bool IsEquipped(VoiceAsset item) => BuildController.Instance.CurrentBuild.value.Cosmetics.Voice.Id == item.AssetObject.Guid;
}
