using Extensions.Components.Miscellaneous;
using Quantum;
using UnityEngine;

public class RulesetController : Controller<RulesetController>
{
    private SerializableWrapper<Ruleset> _ruleset;
    public SerializableWrapper<Ruleset> Ruleset => _ruleset;

    public static string GetPath() => $"{Application.persistentDataPath}/Rulesets";

    public SerializableWrapper<Ruleset> New()
    {
        Ruleset ruleset = new();

        ruleset.SerializableData.Name = "Untitled";
        ruleset.SerializableData.Guid = AssetGuid.NewGuid();
        ruleset.SerializableData.CreationDate = System.DateTime.Now.Ticks;
        ruleset.SerializableData.LastEdittedDate = System.DateTime.Now.Ticks;

        return new(ruleset);
    }

    public void Save(SerializableWrapper<Ruleset> ruleset)
    {
        Serializer.Save(ruleset, ruleset.Value.SerializableData.Guid, GetPath());
    }

    public void Select(SerializableWrapper<Ruleset> ruleset)
    {
        _ruleset = ruleset;
    }

    public void LoadFromAsset(RulesetAssetAsset ruleset)
    {
        _ruleset = ruleset.Ruleset;
        FindFirstObjectByType<QuantumRunnerLocalDebug>().OnStartDeferred.AddListener(_ => SendToSimulation());
    }

    public void ResetValue()
    {
        _ruleset = null;
    }

    public void SendToSimulation()
    {
        if (_ruleset is null)
            return;

        CommandSetRuleset setRuleset = new()
        {
            ruleset = _ruleset
        };

        QuantumRunner.Default.Game.SendCommand(setRuleset);
    }
}
