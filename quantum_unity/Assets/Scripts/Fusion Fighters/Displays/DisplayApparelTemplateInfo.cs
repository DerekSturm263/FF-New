using Extensions.Components.UI;
using Extensions.Types;
using UnityEngine;

using Type = ApparelTemplateAsset;

public class DisplayApparelTemplateInfo : DisplayTextAndImage<Type>
{
    protected override Tuple<string, Sprite> GetInfo(Type item)
    {
        if (!item)
            return new(string.Empty, null);

        return new(string.Format(_format, item.name, item.Description), item.Icon);
    }

    protected override Type GetValue() => default;
}
