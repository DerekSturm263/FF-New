using Extensions.Types;
using UnityEngine;

public abstract partial class InfoAssetAsset : AssetBase
{
    [Header("Unity")]

    public Sprite Icon;
    [TextArea] public string Description;

    [Space(10)]

    [Header("Unlock Data")]

    public bool IncludeInLists;
    public bool IsUnlocked;
    public Nullable<int> StartingCount;
    public uint Price;
}
