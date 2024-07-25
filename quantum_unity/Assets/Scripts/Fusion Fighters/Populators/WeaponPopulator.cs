using Extensions.Components.UI;
using System;
using System.Collections.Generic;
using UnityEngine;

using Type = SerializableWrapper<Quantum.Weapon>;

public class WeaponPopulator : Populate<Type, long>
{
    protected override Sprite Icon(Type item) => item.Icon;

    protected override IEnumerable<Type> LoadAll() => FusionFighters.Serializer.LoadAllFromDirectory<Type>(WeaponController.GetPath());

    protected override string Name(Type item) => item.Name;

    protected override Func<Type, long> Sort() => (build) => build.LastEditedDate;
}
