using Extensions.Components.UI;
using Quantum;

public class DisplayRulesetInfo : DisplayText<Ruleset>
{
    protected override string GetInfo(Ruleset item) => item.SerializableData.Name;
    protected override Ruleset GetValue() => default;
}
