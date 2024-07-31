using Extensions.Types;
using UnityEngine;

public abstract partial class InfoAssetAsset : AssetBase
{
    [Header("Unity")]

    [TextArea] public string Description;

    [Space(10)]

    public bool IsUnlocked;
    public Nullable<int> StartingCount;

    public uint Price;

    public Sprite Icon;
    public Sprite Background;

    public int SortingID;
}
