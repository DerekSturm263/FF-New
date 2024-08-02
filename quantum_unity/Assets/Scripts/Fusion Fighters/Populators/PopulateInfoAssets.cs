using Extensions.Components.UI;
using Quantum;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PopulateInfoAssets : Populate<InfoAsset>
{
    [SerializeField] private string _filePath;

    protected override IEnumerable<InfoAsset> LoadAll() => Resources.LoadAll<InfoAssetAsset>(_filePath).Select(item => item.Settings);

    protected override string Name(InfoAsset item) => item.GetUnityAsset().name;
    protected override string Description(InfoAsset item) => item.GetUnityAsset().Description;
    protected override Sprite Icon(InfoAsset item) => item.GetUnityAsset().Icon;

    protected override Dictionary<string, Predicate<InfoAsset>> GetAllFilterModes()
    {
        return new()
        {
            ["Unlocked"] = (value) => InventoryController.Instance.HasUnlockedItem(value) || IsNone(value)
        };
    }
    protected override Predicate<InfoAsset> GetDefaultFilterMode() => _allFilterModes["All"];

    protected override Dictionary<string, Comparison<InfoAsset>> GetAllSortModes()
    {
        return new()
        {
            ["Name"] = (lhs, rhs) => lhs.GetUnityAsset().name.CompareTo(rhs.GetUnityAsset().name)
        };
    }
    protected override Comparison<InfoAsset> GetDefaultSortMode() => _allSortModes["Name"];

    protected override bool IsEquipped(InfoAsset item) => false;
    protected override bool IsNone(InfoAsset item) => item.GetUnityAsset().name.Equals("None");
}
