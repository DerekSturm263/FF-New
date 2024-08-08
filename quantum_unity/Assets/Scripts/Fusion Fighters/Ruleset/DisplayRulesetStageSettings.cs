using Extensions.Components.UI;
using System.Collections.Generic;
using UnityEngine.UI;

using Type = Quantum.StageSettings;

public class DisplayRulesetStageSettings : Display<Type, List<Selectable>>
{
    public override void UpdateDisplay(Type item)
    {
        (_component[0] as TMPro.TMP_Dropdown).value = (int)item.StagePicker;
        (_component[1] as Toggle).isOn = item.AllowGizmos;
        (_component[2] as Toggle).isOn = item.AllowCustomStages;
        (_component[3] as Toggle).isOn = item.AllowDuplicateSelection;
    }

    protected override Type GetValue() => RulesetController.Instance.CurrentRuleset.value.Stage;
}
