using Extensions.Components.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class PopulateSerializable<T, TAssetAsset> : Populate<SerializableWrapper<T>> where T : struct where TAssetAsset : InfoAssetAsset
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
        Dictionary<string, Predicate<SerializableWrapper<T>>> tagGroups = new();

        foreach (var key in _itemsToButtons.Keys)
        {
            if (key.FilterTags is not null)
            {
                foreach (string tag in key.FilterTags)
                {
                    tagGroups.TryAdd(tag, (value) =>
                    {
                        if (value.FilterTags is not null)
                            return value.FilterTags.Contains(tag);

                        return false;
                    });
                }
            }
        }

        Dictionary<string, Predicate<SerializableWrapper<T>>> baseFilters = new();

        if (_loadType.HasFlag(CreationType.BuiltIn) && _loadType.HasFlag(CreationType.Custom))
        {
            baseFilters.Add("All", (_) => true);
            baseFilters.Add("Custom", (value) => value.MadeByPlayer);
            baseFilters.Add("Built-In", (value) => !value.MadeByPlayer);
        }
        else
        {
            baseFilters.Add("All", (_) => true);
        }
        
        return baseFilters.Concat(tagGroups).ToDictionary(item => item.Key, item => item.Value);
    }

    protected override Dictionary<string, Func<SerializableWrapper<T>, (string, object)>> GetAllGroupModes()
    {
        Dictionary<string, Func<SerializableWrapper<T>, (string, object)>> tagGroups = new();

        foreach (var key in _itemsToButtons.Keys)
        {
            if (key.GroupTags is not null)
            {
                foreach (var tag in key.GroupTags)
                {
                    tagGroups.TryAdd(tag.Item1, (value) =>
                    {
                        if (value.GroupTags is not null)
                            return (tag.Item2, tag);

                        return default;
                    });
                }
            }
        }

        return new Dictionary<string, Func<SerializableWrapper<T>, (string, object)>>()
        {
            ["All"] = (value) => ("All", 0)
        }.Concat(tagGroups).ToDictionary(item => item.Key, item => item.Value);
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
