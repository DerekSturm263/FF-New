using Extensions.Components.Miscellaneous;
using Quantum;
using UnityEngine;

public class RulesetManager : Controller<RulesetManager>
{
    public static string GetPath() => $"{Application.persistentDataPath}/Rulesets";

    public override void Initialize()
    {
        base.Initialize();
    }

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
}
