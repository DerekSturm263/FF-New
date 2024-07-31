using Extensions.Components.UI;
using Extensions.Types;
using UnityEngine;

using Type = WeaponMaterialAsset;

public class DisplayWeaponMaterialInfo : DisplayTextAndImage<Type>
{
    protected override Tuple<string, Sprite> GetInfo(Type item)
    {
        return new(string.Format(_format, item.name, item.Description), item.Icon);
    }

    protected override Type GetValue() => default;
}
