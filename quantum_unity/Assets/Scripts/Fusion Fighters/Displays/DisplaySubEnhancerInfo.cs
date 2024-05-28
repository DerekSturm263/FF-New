using Extensions.Components.UI;
using UnityEngine;

using Type = SubEnhancerAsset;

public class DisplaySubEnhancerInfo : DisplayImage<Type>
{
    protected override Sprite GetInfo(Type item)
    {
        return item.Icon;
    }

    protected override Type GetValue() => default;
}
