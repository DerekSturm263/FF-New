using Quantum;

public class BehaviorPopulator : PopulateAsset<BehaviorAsset>
{
    protected override string FilePath() => "DB/Assets/Player/Behaviors";

    protected override unsafe bool IsEquipped(BehaviorAsset item)
    {
        EntityRef bot = FighterIndex.GetFirstEntity(QuantumRunner.Default.Game.Frames.Verified, item => item.Type == FighterType.Bot);

        if (QuantumRunner.Default.Game.Frames.Verified.Unsafe.TryGetPointer(bot, out AIData* aiData))
            return aiData->Behavior.Id == item.AssetObject.Guid;

        return false;
    }
}
