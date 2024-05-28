using Extensions.Components.UI;
using UnityEngine;

using Type = ApparelPatternAsset;

public class DisplayApparelPatternInfo : DisplayImage<Type>
{
    protected override Sprite GetInfo(Type item)
    {
        return item.Icon;
    }

    protected override Type GetValue() => default;
}
