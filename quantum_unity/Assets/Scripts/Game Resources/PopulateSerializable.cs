using Extensions.Components.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class PopulateSerializable<T, TAssetAsset> : Populate<SerializableWrapper<T>> where T : unmanaged where TAssetAsset : InfoAssetAsset
{
    [Flags]
    public enum CreationType
    {
        BuiltIn = 1 << 0,
        Custom = 1 << 1
    }
    [SerializeField] protected CreationType _loadType = CreationType.BuiltIn | CreationType.Custom;

    protected override IEnumerable<SerializableWrapper<T>> LoadAll()
    {
        List<SerializableWrapper<T>> results = new();

        if (_loadType.HasFlag(CreationType.BuiltIn))
            results.AddRange(Resources.LoadAll<TAssetAsset>(BuiltInFilePath()).Where(item => item.IncludeInLists).Select(GetFromBuiltInAsset));

        if (_loadType.HasFlag(CreationType.Custom))
            results.AddRange(FusionFighters.Serializer.LoadAllFromDirectory<SerializableWrapper<T>>(CustomFilePath()));

        return results;
    }

    protected override string Name(SerializableWrapper<T> item) => item.Name;
    protected override string Description(SerializableWrapper<T> item) => item.Description;
    protected override Sprite Icon(SerializableWrapper<T> item) => item.Icon;

    protected override Dictionary<string, Predicate<SerializableWrapper<T>>> GetAllFilterModes()
    {
        return new()
        {
            ["All"] = (_) => true,
            ["Custom"] = (value) => value.MadeByPlayer || IsNone(value),
            ["Built-In"] = (value) => !value.MadeByPlayer || IsNone(value)
        };
    }

    protected override Dictionary<string, Func<SerializableWrapper<T>, (string, object)>> GetAllGroupModes()
    {
        return new()
        {
            ["All"] = (value) => ("All", 0)
        };
    }

    protected override Dictionary<string, Comparison<SerializableWrapper<T>>> GetAllSortModes()
    {
        return new()
        {
            ["Last Edited"] = (lhs, rhs) => lhs.LastEditedDate.CompareTo(rhs.LastEditedDate),
            ["Created"] = (lhs, rhs) => lhs.CreationDate.CompareTo(rhs.CreationDate),
            ["Name"] = (lhs, rhs) => lhs.Name.CompareTo(rhs.Name)
        };
    }

    protected abstract string BuiltInFilePath();
    protected abstract string CustomFilePath();
    protected abstract SerializableWrapper<T> GetFromBuiltInAsset(TAssetAsset asset);
}
