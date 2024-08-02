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
        return new()
        {
            ["Unlocked"] = (value) => InventoryController.Instance.HasUnlockedItem(value) || IsNone(value)
        };
    }
    protected override Predicate<T> GetDefaultFilterMode() => _allFilterModes["Unlocked"];

    protected override Dictionary<string, Comparison<T>> GetAllSortModes()
    {
        return new()
        {
            ["Name"] = (lhs, rhs) => lhs.name.CompareTo(rhs.name)
        };
    }
    protected override Comparison<T> GetDefaultSortMode() => _allSortModes["Name"];

    protected abstract string FilePath();

    protected override bool IsNone(T item) => item.name.Equals("None");
}
