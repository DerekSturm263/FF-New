using UnityEngine;

public abstract partial class InfoAssetAsset : AssetBase
{
    [Header("Unity")]

    [TextArea] public string Description;

    [Space(10)]

    public int Price;

    public Sprite Icon;
    public Sprite Background;

    public Color MainColor;
    public Color LightColor;
    public Color DarkColor;

    public int SortingID;
}
