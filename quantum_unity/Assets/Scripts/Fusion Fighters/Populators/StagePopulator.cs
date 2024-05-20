using Extensions.Components.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using Type = Quantum.Stage;

public class StagePopulator : Populate<Type, long>
{
    private const string FILE_PATH = "DB/Assets/Stage/Stages";

    protected override Sprite Icon(Type item) => null;

    protected override IEnumerable<Type> LoadAll() => Serializer.LoadAllFromDirectory<Type>(StageController.GetPath()).Concat(Resources.LoadAll<StageAssetAsset>(FILE_PATH).Select(item => item.Settings_StageAsset.Stage));

    protected override string Name(Type item) => item.SerializableData.Name;

    protected override Func<Type, long> Sort() => (build) => build.SerializableData.LastEdittedDate;
}
