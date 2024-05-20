using Extensions.Components.Miscellaneous;
using Quantum;
using UnityEngine;

public class RulesetController : Controller<RulesetController>
{
    private Ruleset _ruleset;
    public Ruleset Ruleset => _ruleset;

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

    public void SendToSimulation(Ruleset ruleset)
    {
        CommandSetRuleset setRuleset = new()
        {
            ruleset = _ruleset
        };

        QuantumRunner.Default.Game.SendCommand(setRuleset);
    }
}
