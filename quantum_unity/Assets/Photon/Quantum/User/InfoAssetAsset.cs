using UnityEngine;

public abstract partial class InfoAssetAsset : AssetBase
{
    [Header("Unity")]

    public string Name;
    [Multiline] public string Description;

    public Sprite Icon;
    public Sprite Background;

    public Color MainColor;
    public Color LightColor;
    public Color DarkColor;

    public int SortingID;
}
