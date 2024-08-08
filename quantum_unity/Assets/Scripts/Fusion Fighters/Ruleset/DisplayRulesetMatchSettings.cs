using Extensions.Components.UI;
using System.Collections.Generic;
using UnityEngine.UI;

using Type = Quantum.MatchSettings;

public class DisplayRulesetMatchSettings : Display<Type, List<Selectable>>
{
    public override void UpdateDisplay(Type item)
    {
        (_component[0] as TMPro.TMP_InputField).SetTextWithoutNotify(item.Time.ToString());
        (_component[1] as Toggle).isOn = item.ShowTimer;
        (_component[2] as Toggle).isOn = item.ShowScores;
        (_component[3] as TMPro.TMP_InputField).SetTextWithoutNotify(item.MatchCount.ToString());
        (_component[4] as Toggle).isOn = item.EndMatchesWhenWinnerClear;
        (_component[5] as Toggle).isOn = item.ShowCurrentWinner;
    }

    protected override Type GetValue() => RulesetController.Instance.CurrentRuleset.value.Match;
}
