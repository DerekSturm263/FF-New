using Extensions.Components.UI;
using System;
using System.Collections.Generic;
using UnityEngine;

using Type = SerializableWrapper<Quantum.Apparel>;

public class ApparelPopulator : Populate<Type, long>
{
    protected override Sprite Icon(Type item) => item.Icon;

    protected override IEnumerable<Type> LoadAll() => Serializer.LoadAllFromDirectory<Type>(ApparelController.GetPath());

    protected override string Name(Type item) => item.Value.SerializableData.Name;

    protected override Func<Type, long> Sort() => (build) => build.Value.SerializableData.LastEdittedDate;
}
