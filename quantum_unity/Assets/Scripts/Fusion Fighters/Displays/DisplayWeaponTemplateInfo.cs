using Extensions.Components.UI;
using Extensions.Types;
using UnityEngine;

using Type = WeaponTemplateAsset;

public class DisplayWeaponTemplateInfo : DisplayTextAndImage<Type>
{
    protected override Tuple<string, Sprite> GetInfo(Type item)
    {
        if (!item)
            return new(string.Empty, null);

        return new(string.Format(_format, item.name, item.Description), item.Icon);
    }

    protected override Type GetValue() => default;
}
