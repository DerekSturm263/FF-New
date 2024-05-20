using Extensions.Components.UI;
using Quantum;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PopulateInfoAssets : Populate<InfoAsset, int>
{
    [SerializeField] private string _filePath;

    protected override Sprite Icon(InfoAsset item) => item.GetUnityAsset().Icon;

    protected override IEnumerable<InfoAsset> LoadAll() => Resources.LoadAll<InfoAssetAsset>(_filePath).Select(item => item.Settings);

    protected override string Name(InfoAsset item) => item.GetUnityAsset().name;

    protected override Func<InfoAsset, int> Sort() => (infoAsset) => infoAsset.GetUnityAsset().SortingID;
}
