using Extensions.Components.UI;
using UnityEngine;

using Type = SubTemplateAsset;

public class DisplaySubTemplateInfo : DisplayImage<Type>
{
    protected override Sprite GetInfo(Type item)
    {
        return item.Icon;
    }

    protected override Type GetValue() => default;
}
