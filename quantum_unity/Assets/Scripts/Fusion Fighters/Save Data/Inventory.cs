using Extensions.Types;
using Quantum;
using UnityEngine;

[System.Serializable]
public struct Inventory
{
    public ulong Currency;

    [Tooltip("Dictionary of all Info Assets to a tuple which represents if they start off unlocked and how many you have (null means infinite)")] public Dictionary<AssetRefInfoAsset, Tuple<bool, Nullable<int>>> ItemCollection;

    public Inventory DeepCopy()
    {
        return new()
        {
            Currency = Currency,
            ItemCollection = new(ItemCollection)
        };
    }
}
