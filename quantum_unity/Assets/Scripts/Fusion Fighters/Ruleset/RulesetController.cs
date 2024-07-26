using Extensions.Components.Miscellaneous;
using Quantum;
using UnityEngine;

public class RulesetController : Controller<RulesetController>
{
    private SerializableWrapper<Ruleset>? _ruleset;
    public SerializableWrapper<Ruleset>? Ruleset => _ruleset;

    public static string GetPath() => $"{Application.persistentDataPath}/SaveData/Custom/Rulesets";

    public SerializableWrapper<Ruleset> New()
    {
        Ruleset ruleset = new();
        return new(ruleset, "Untitled", "", System.DateTime.Now.Ticks, System.DateTime.Now.Ticks, AssetGuid.NewGuid());
    }

    public void Save(SerializableWrapper<Ruleset> ruleset)
    {
        ruleset.Save(GetPath());
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
        _ruleset = new(ruleset, "", "", 0, 0, AssetGuid.NewGuid());
        FindFirstObjectByType<QuantumRunnerLocalDebug>().OnStart.AddListener(_ => SendToSimulation());
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
            ruleset = _ruleset.GetValueOrDefault()
        };

        QuantumRunner.Default.Game.SendCommand(setRuleset);
    }
}
