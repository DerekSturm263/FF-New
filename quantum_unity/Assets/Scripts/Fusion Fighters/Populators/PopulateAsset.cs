using Extensions.Components.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class PopulateAsset<T> : Populate<T> where T : InfoAssetAsset
{
    protected override IEnumerable<T> LoadAll() => Resources.LoadAll<T>(FilePath()).Where(item => item.IncludeInLists);

    protected override T Random(IEnumerable<T> items)
    {
        T newSO = Instantiate(items.ElementAt(UnityEngine.Random.Range(0, items.Count())));

        newSO.name = "Random";
        newSO.Description = $"Selects a random {typeof(T).Name.ToLower()}.";
        newSO.isRandom = true;

        return newSO;
    }
    protected override void ReassignRandom(ref T random, IEnumerable<T> items)
    {
        // TODO: FIX. THIS WILL REASSIGN THE REFERENCE
        random = Instantiate(items.ElementAt(UnityEngine.Random.Range(0, items.Count())));

        random.name = "Random";
        random.Description = $"Selects a random {typeof(T).Name.ToLower()}.";
        random.isRandom = true;
        random.Icon = Resources.Load<Sprite>("DB/Textures/Random.png");
    }

    protected override bool IsRandom(T item) => item.isRandom;

    protected override string Name(T item) => item.name;
    protected override string Description(T item) => item.Description;
    protected override Sprite Icon(T item) => item.Icon;

    protected override Dictionary<string, Predicate<T>> GetAllFilterModes()
    {
        Dictionary<string, Predicate<T>> tagGroups = new();

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

        return new Dictionary<string, Predicate<T>>()
        {
            ["All"] = (value) => InventoryController.Instance.HasUnlockedItem(value)
        }.Concat(tagGroups).ToDictionary(item => item.Key, item => item.Value);
    }

    protected override Dictionary<string, Func<T, (string, object)>> GetAllGroupModes()
    {
        Dictionary<string, Func<T, (string, object)>> tagGroups = new();

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

        return new Dictionary<string, Func<T, (string, object)>>()
        {
            ["All"] = (value) => ("All", 0)
        }.Concat(tagGroups).ToDictionary(item => item.Key, item => item.Value);
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
