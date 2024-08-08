using Extensions.Components.UI;
using Extensions.Types;
using UnityEngine;

using Type = ItemAsset;

public class DisplayItem : DisplayTextAndImage<Type>
{
    protected override Tuple<string, Sprite> GetInfo(Type item)
    {
        return new(string.Format(_format, item.name, item.Description), item.Icon);
    }

    protected override Type GetValue() => UnityDB.FindAsset<Type>(RulesetController.Instance.CurrentRuleset.value.Items.StartingItem.Id);
}
