using Extensions.Components.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class PopulateAsset<T> : Populate<T> where T : InfoAssetAsset
{
    protected override IEnumerable<T> LoadAll() => Resources.LoadAll<T>(FilePath()).Where(item => item.IncludeInLists);

    protected override string Name(T item) => item.name;
    protected override string Description(T item) => item.Description;
    protected override Sprite Icon(T item) => item.Icon;

    protected override Dictionary<string, Predicate<T>> GetAllFilterModes()
    {
        Dictionary<string, Predicate<T>> tagGroups = new();

        foreach (var key in _itemsToButtons.Keys)
        {
            foreach (string tag in key.FilterTags)
            {
                tagGroups.TryAdd(tag, (value) => value.FilterTags.Contains(tag));
            }
        }

        return tagGroups.Concat(new Dictionary<string, Predicate<T>>()
        {
            ["Unlocked"] = (value) => InventoryController.Instance.HasUnlockedItem(value) || IsNone(value)
        }).ToDictionary(item => item.Key, item => item.Value);
    }

    protected override Dictionary<string, Func<T, (string, object)>> GetAllGroupModes()
    {
        Dictionary<string, Func<T, (string, object)>> tagGroups = new();

        foreach (var key in _itemsToButtons.Keys)
        {
            foreach (var tag in key.GroupTags)
            {
                tagGroups.TryAdd(tag.Item1, (value) => (tag.Item2, value.GroupTags.Contains(tag)));
            }
        }

        return tagGroups.Concat(new Dictionary<string, Func<T, (string, object)>>()
        {
            ["All"] = (value) => ("All", 0)
        }).ToDictionary(item => item.Key, item => item.Value);
    }

    protected override Dictionary<string, Comparison<T>> GetAllSortModes()
    {
        return new()
        {
            ["Name"] = (lhs, rhs) => lhs.name.CompareTo(rhs.name)
        };
    }

    protected abstract string FilePath();

    protected override bool IsNone(T item) => item.name.Equals("None");
}
