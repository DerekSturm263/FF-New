using Extensions.Types;
using Quantum;
using Quantum.Inspector;

[System.Serializable]
public struct Inventory
{
    public int Currency;

    [Tooltip("Things that need multiple to be collected and are consumed")] public Dictionary<AssetRefInfoAsset, int> CountItemCollection;
    [Tooltip("Things that you only need one collected and are not consumed")] public Dictionary<AssetRefInfoAsset, bool> ConditionalItemCollection;
}
