using Extensions.Components.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using Type = SerializableWrapper<Quantum.Build>;

public class BuildPopulator : Populate<Type, long>
{
    private const string FILE_PATH = "DB/Assets/Build/Builds";

    protected override Sprite Icon(Type item) => item.Icon;

    protected override IEnumerable<Type> LoadAll() => Serializer.LoadAllFromDirectory<Type>(BuildController.GetPath()).Concat(Resources.LoadAll<BuildAssetAsset>(FILE_PATH).Select(item => item.Build));

    protected override string Name(Type item) => item.Value.SerializableData.Name;

    protected override Func<Type, long> Sort() => (build) => build.Value.SerializableData.LastEdittedDate;
}
