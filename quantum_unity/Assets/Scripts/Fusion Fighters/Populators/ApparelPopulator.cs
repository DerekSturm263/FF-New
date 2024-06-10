using Extensions.Components.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using Type = SerializableWrapper<Quantum.Apparel>;

public class ApparelPopulator : Populate<Type, long>
{
    [SerializeField] private Quantum.ApparelTemplate.ApparelType _type;

    protected override Sprite Icon(Type item) => item.Icon;

    protected override IEnumerable<Type> LoadAll() => Serializer.LoadAllFromDirectory<Type>(ApparelController.GetPath()).Where(item => UnityDB.FindAsset<ApparelTemplateAsset>(item.Value.Template.Id).Settings_ApparelTemplate.Type.HasFlag(_type));

    protected override string Name(Type item) => item.Value.SerializableData.Name;

    protected override Func<Type, long> Sort() => (build) => build.Value.SerializableData.LastEdittedDate;
}
