using Extensions.Components.UI;
using Extensions.Types;
using UnityEngine;

using Type = ApparelTemplateAsset;

public class DisplayApparelTemplateInfo : DisplayTextAndImage<Type>
{
    protected override Tuple<string, Sprite> GetInfo(Type item)
    {
        return new(string.Format(_format, item.name, item.Description), item.Icon);
    }

    protected override Type GetValue() => default;
}
