using Extensions.Components.UI;
using Extensions.Types;
using UnityEngine;

using Type = SubTemplateAsset;

public class DisplaySubTemplateInfo : DisplayTextAndImage<Type>
{
    protected override Tuple<string, Sprite> GetInfo(Type item)
    {
        return new(string.Format(_format, item.name, item.Description), item.Icon);
    }

    protected override Type GetValue() => default;
}
