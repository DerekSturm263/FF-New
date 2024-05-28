using Extensions.Components.UI;
using UnityEngine;

using Type = ApparelTemplateAsset;

public class DisplayApparelTemplateInfo : DisplayImage<Type>
{
    protected override Sprite GetInfo(Type item)
    {
        return item.Icon;
    }

    protected override Type GetValue() => default;
}
