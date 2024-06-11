using UnityEngine;

public abstract partial class InfoAssetAsset : AssetBase
{
    [Header("Unity")]

    [Multiline] public string Description;
    public int Price;

    public Sprite Icon;
    public Sprite Background;

    public Color MainColor;
    public Color LightColor;
    public Color DarkColor;

    public int SortingID;
}
