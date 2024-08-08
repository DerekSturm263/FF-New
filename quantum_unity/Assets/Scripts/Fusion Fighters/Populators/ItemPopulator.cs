public class ItemPopulator : PopulateAsset<ItemAsset>
{
    protected override string FilePath() => "DB/Assets/Item/Items";

    protected override bool IsEquipped(ItemAsset item) => RulesetController.Instance.CurrentRuleset.value.Items.StartingItem.Id == item.AssetObject.Guid;
}
