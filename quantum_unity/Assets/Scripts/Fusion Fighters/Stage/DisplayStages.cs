using Extensions.Components.UI;
using Extensions.Miscellaneous;
using Extensions.Types;
using Quantum.Types;
using System.Linq;
using UnityEngine;

using Type = Quantum.ArrayStages;

public class DisplayStages : DisplayTextAndImage<Type>
{
    protected override Tuple<string, Sprite> GetInfo(Type item)
    {
        string items = Helper.PrintNames(ArrayHelper.All(item).Where(item => item.Id.IsValid), item => UnityDB.FindAsset<StageAssetAsset>(item.Id).name);

        return new(string.Format(_format, items), null);
    }

    protected override Type GetValue() => RulesetController.Instance.CurrentRuleset.value.Stage.Stages;
}
