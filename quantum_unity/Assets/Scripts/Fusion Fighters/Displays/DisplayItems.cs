using Extensions.Components.UI;
using Extensions.Types;
using Quantum.Types;
using System.Linq;
using System.Text;
using UnityEngine;

using Type = Quantum.ArrayItems;

public class DisplayItems : DisplayTextAndImage<Type>
{
    protected override Tuple<string, Sprite> GetInfo(Type item)
    {
        StringBuilder items = new();

        if (ArrayHelper.All(item).Count(item => item.Id.IsValid) > 0)
        {
            foreach (var stage in ArrayHelper.All(item))
            {
                var itemAsset = UnityDB.FindAsset<ItemAsset>(stage.Id);
                if (itemAsset)
                {
                    items.Append(itemAsset.name + ", ");
                }
            }

            items.Remove(items.Length - 2, 2);
        }
        else
        {
            items.Append("None");
        }

        return new(string.Format(_format, items), null);
    }

    protected override Type GetValue() => RulesetController.Instance.CurrentRuleset.value.Items.Items;
}
