using Extensions.Components.UI;
using UnityEngine;

using Type = ApparelModifierAsset;

public class DisplayApparelModifierInfo : DisplayImage<Type>
{
    protected override Sprite GetInfo(Type item)
    {
        return item.Icon;
    }

    protected override Type GetValue() => default;
}
