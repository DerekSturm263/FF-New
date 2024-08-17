using Extensions.Components.UI;
using System.Collections.Generic;
using UnityEngine.UI;

using Type = Quantum.PlayerSettings;

public class DisplayRulesetPlayerSettings : Display<Type, List<Selectable>>
{
    public override void UpdateDisplay(Type item)
    {
        (_component[0] as TMPro.TMP_InputField).SetTextWithoutNotify(item.StockCount.ToString());
        (_component[1] as TMPro.TMP_InputField).SetTextWithoutNotify(item.MaxHealth.ToString());
        (_component[2] as TMPro.TMP_InputField).SetTextWithoutNotify(item.MaxEnergy.ToString());
        (_component[3] as Slider).value = item.EnergyChargeRate.AsFloat;
        (_component[4] as TMPro.TMP_InputField).SetTextWithoutNotify(item.RespawnTime.ToString());
        (_component[5] as Toggle).isOn = item.AllowFriendlyFire;
        (_component[6] as Toggle).isOn = item.AllowDuplicateSelection;
    }

    protected override Type GetValue() => RulesetController.Instance.CurrentRuleset.value.Players;
}
