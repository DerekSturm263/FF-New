using Extensions.Components.Miscellaneous;
using Quantum;
using UnityEngine;

public class RulesetController : Controller<RulesetController>
{
    [SerializeField] private RulesetAssetAsset _default;

    private SerializableWrapper<Ruleset>? _ruleset;
    public SerializableWrapper<Ruleset>? Ruleset => _ruleset;

    public static string GetPath() => $"{Application.persistentDataPath}/SaveData/Custom/Rulesets";

    public SerializableWrapper<Ruleset> New()
    {
        string[] filterTags = new string[] { };
        Extensions.Types.Tuple<string, string>[] groupTags = new Extensions.Types.Tuple<string, string>[] { };

        return new(_default.Ruleset.value, "Untitled", "", System.DateTime.Now.Ticks, System.DateTime.Now.Ticks, AssetGuid.NewGuid(), filterTags, groupTags, null, null);
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
        string[] filterTags = new string[] { };
        Extensions.Types.Tuple<string, string>[] groupTags = new Extensions.Types.Tuple<string, string>[] { };
        _ruleset = new(ruleset, "", "", 0, 0, AssetGuid.NewGuid(), filterTags, groupTags, null, null);

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
