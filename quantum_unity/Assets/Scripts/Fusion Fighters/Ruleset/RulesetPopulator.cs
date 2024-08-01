using Extensions.Components.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using Type = SerializableWrapper<Quantum.Ruleset>;

public class RulesetPopulator : Populate<Type, long>
{
    [SerializeField] private Type.CreationType _loadType = Type.CreationType.BuiltIn | Type.CreationType.Custom;

    private const string FILE_PATH = "DB/Assets/Ruleset/Rulesets";

    protected override Sprite Icon(Type item) => item.Icon;

    protected override IEnumerable<Type> LoadAll()
    {
        List<Type> results = new();

        if (_loadType.HasFlag(Type.CreationType.Custom))
            results.AddRange(FusionFighters.Serializer.LoadAllFromDirectory<Type>(RulesetController.GetPath()));

        if (_loadType.HasFlag(Type.CreationType.BuiltIn))
            results.AddRange(Resources.LoadAll<RulesetAssetAsset>(FILE_PATH).Where(item => item.IncludeInLists).Select(item => item.Ruleset));

        return results;
    }

    protected override string Name(Type item) => item.Name;

    protected override Func<Type, long> Sort() => (build) => build.LastEditedDate;
}
