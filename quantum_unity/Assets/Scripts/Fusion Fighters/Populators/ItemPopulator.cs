using Quantum.Types;
using System.Linq;
using UnityEngine;

public class ItemPopulator : PopulateAsset<ItemAsset>
{
    public enum ItemType
    {
        Starting,
        All
    }

    [SerializeField] private ItemType _type;

    protected override string FilePath() => "DB/Assets/Item/Items";

    protected override bool IsEquipped(ItemAsset item)
    {
        return _type switch
        {
            ItemType.Starting => RulesetController.Instance.CurrentRuleset.value.Items.StartingItem.Id == item.AssetObject.Guid,
            ItemType.All => ArrayHelper.All(RulesetController.Instance.CurrentRuleset.value.Items.Items).Any(itemItem => itemItem.Id == item.AssetObject.Guid),
            _ => false
        };
    }
}
