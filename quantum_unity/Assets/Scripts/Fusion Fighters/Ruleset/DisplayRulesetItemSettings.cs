using Extensions.Components.UI;
using System.Collections.Generic;
using UnityEngine.UI;

using Type = Quantum.ItemSettings;

public class DisplayRulesetItemSettings : Display<Type, List<Selectable>>
{
    public override void UpdateDisplay(Type item)
    {
        (_component[0] as Slider).value = item.SpawnFrequency.AsFloat;
    }

    protected override Type GetValue() => RulesetController.Instance.CurrentRuleset.value.Items;
}
