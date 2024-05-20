using Extensions.Components.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using Type = Quantum.Build;

public class BuildPopulator : Populate<Type, long>
{
    private const string FILE_PATH = "DB/Assets/Build/Builds";

    protected override Sprite Icon(Type item) => null;

    protected override IEnumerable<Type> LoadAll() => Serializer.LoadAllFromDirectory<Type>(BuildController.GetPath()).Concat(Resources.LoadAll<BuildAssetAsset>(FILE_PATH).Select(item => item.Settings_BuildAsset.Build));

    protected override string Name(Type item) => item.SerializableData.Name;

    protected override Func<Type, long> Sort() => (build) => build.SerializableData.LastEdittedDate;
}
