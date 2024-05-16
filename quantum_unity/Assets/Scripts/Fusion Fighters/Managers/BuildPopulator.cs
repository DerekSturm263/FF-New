using Extensions.Components.UI;
using Quantum;
using System;
using System.Collections.Generic;
using UnityEngine;

using Type = Quantum.Build;

public class BuildPopulator : Populate<Type, long>
{
    protected override Sprite Icon(Type item) => null;

    protected override IEnumerable<Type> LoadAll() => Serializer.LoadAllFromDirectory<Type>(BuildManager.GetPath());

    protected override string Name(Type item) => item.SerializableData.Name;

    protected override Func<Type, long> Sort() => (build) => build.SerializableData.LastEdittedDate;
}
