using Extensions.Components.UI;
using System;
using System.Collections.Generic;
using UnityEngine;

using Type = SerializableWrapper<Quantum.Sub>;

public class SubPopulator : Populate<Type, long>
{
    protected override Sprite Icon(Type item) => item.Icon;

    protected override IEnumerable<Type> LoadAll() => Serializer.LoadAllFromDirectory<Type>(SubController.GetPath());

    protected override string Name(Type item) => item.Value.SerializableData.Name;

    protected override Func<Type, long> Sort() => (build) => build.Value.SerializableData.LastEdittedDate;
}
