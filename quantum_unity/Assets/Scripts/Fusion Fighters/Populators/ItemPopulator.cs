public class ItemPopulator : PopulateAsset<ItemAsset>
{
    protected override string FilePath() => "DB/Assets/Item/Items";

    protected override bool IsEquipped(ItemAsset item) => false;
}
