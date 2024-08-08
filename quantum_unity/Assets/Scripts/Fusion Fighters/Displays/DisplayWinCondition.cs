using Extensions.Components.UI;
using Extensions.Types;
using UnityEngine;

using Type = WinConditionAsset;

public class DisplayWinCondition : DisplayTextAndImage<Type>
{
    protected override Tuple<string, Sprite> GetInfo(Type item)
    {
        return new(string.Format(_format, item.name, item.Description), item.Icon);
    }

    protected override Type GetValue() => UnityDB.FindAsset<Type>(RulesetController.Instance.CurrentRuleset.value.Match.WinCondition.Id);
}
