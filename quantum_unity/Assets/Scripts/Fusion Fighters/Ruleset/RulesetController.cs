using Extensions.Components.Miscellaneous;
using Quantum;
using UnityEngine;

public class RulesetController : Controller<RulesetController>
{
    private Ruleset? _ruleset;
    public Ruleset? Ruleset => _ruleset;

    public static string GetPath() => $"{Application.persistentDataPath}/Rulesets";

    public Ruleset New()
    {
        Ruleset ruleset = new();

        ruleset.SerializableData.Name = "Untitled";
        ruleset.SerializableData.Guid = AssetGuid.NewGuid();
        ruleset.SerializableData.CreationDate = System.DateTime.Now.Ticks;
        ruleset.SerializableData.LastEdittedDate = System.DateTime.Now.Ticks;

        return ruleset;
    }

    public void Save(Ruleset ruleset)
    {
        Serializer.Save(ruleset, ruleset.SerializableData.Guid, GetPath());
    }

    public void Select(Ruleset ruleset)
    {
        _ruleset = ruleset;
    }

    public void ResetValue()
    {
        _ruleset = null;
    }

    public void SendToSimulation(QuantumGame game)
    {
        if (!_ruleset.HasValue)
            return;

        CommandSetRuleset setRuleset = new()
        {
            ruleset = _ruleset.Value
        };

        game.SendCommand(setRuleset);
    }
}
