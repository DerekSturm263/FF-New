using Extensions.Components.UI;
using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class PopulateAsset<T> : Populate<T, int> where T : InfoAssetAsset
{
    protected abstract string FilePath();

    protected override Sprite Icon(T item) => item.Icon;

    protected override IEnumerable<T> LoadAll() => Resources.LoadAll<T>(FilePath());

    protected override string Name(T item) => item.name;

    protected override Func<T, int> Sort() => (item) => item.SortingID;
}
