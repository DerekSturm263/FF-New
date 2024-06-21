using Extensions.Components.Miscellaneous;
using Quantum;
using System.Linq;
using UnityEngine;

public class RulesetController : Controller<RulesetController>
{
    private SerializableWrapper<Ruleset> _ruleset;
    public SerializableWrapper<Ruleset> Ruleset => _ruleset;

    public static string GetPath() => $"{Application.persistentDataPath}/Rulesets";

    public SerializableWrapper<Ruleset> New()
    {
        Ruleset ruleset = new();
        return new(ruleset, "Untitled", "", AssetGuid.NewGuid(), System.DateTime.Now.Ticks, System.DateTime.Now.Ticks);
    }

    public void Save(SerializableWrapper<Ruleset> ruleset)
    {
        Serializer.Save(ruleset, ruleset.Guid, GetPath());
    }

    public void Select(SerializableWrapper<Ruleset> ruleset)
    {
        _ruleset = ruleset;
    }

    public void LoadFromAsset(RulesetAssetAsset ruleset)
    {
        Load(ruleset.Ruleset);
    }

    public void Load(Ruleset ruleset)
    {
        _ruleset = new(ruleset, "", "", AssetGuid.NewGuid(), 0, 0);
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
