public class StatusEffectPopulator : PopulateAsset<StatusEffectAsset>
{
    protected override string FilePath() => "DB/Assets/Player/Status Effects";

    protected override bool IsEquipped(StatusEffectAsset item) => false;
}
