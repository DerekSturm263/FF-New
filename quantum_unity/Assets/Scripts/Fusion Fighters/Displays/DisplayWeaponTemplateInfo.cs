using Extensions.Components.UI;
using Extensions.Types;
using UnityEngine;

using Type = WeaponTemplateAsset;

public class DisplayWeaponTemplateInfo : DisplayTextAndImage<Type>
{
    protected override Tuple<string, Sprite> GetInfo(Type item)
    {
        return new($"<font=\"KeaniaOne-Title SDF\"><size=50>{item.name}</size></font>\n\n{item.Description}", item.Icon);
    }

    protected override Type GetValue() => default;
}
