using Extensions.Components.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using Type = SerializableWrapper<Quantum.Ruleset>;

public class RulesetPopulator : Populate<Type, long>
{
    private const string FILE_PATH = "DB/Assets/Ruleset/Rulesets";

    protected override Sprite Icon(Type item) => item.Icon;

    protected override IEnumerable<Type> LoadAll() => FusionFighters.Serializer.LoadAllFromDirectory<Type>(RulesetController.GetPath()).Concat(Resources.LoadAll<RulesetAssetAsset>(FILE_PATH).Select(item => item.Ruleset));

    protected override string Name(Type item) => item.Name;

    protected override Func<Type, long> Sort() => (build) => build.LastEditedDate;
}
