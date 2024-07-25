using Extensions.Components.UI;
using Extensions.Types;
using UnityEngine;

using Type = FFAvatarAsset;

public class DisplayAvatar : DisplayTextAndImage<Type>
{
    protected override Tuple<string, Sprite> GetInfo(Type item)
    {
        return new($"<font=\"KeaniaOne-Title SDF\"><size=50>{item.name}</size></font>\n\n{item.Description}", item.Icon);
    }

    protected override Type GetValue() => UnityDB.FindAsset<Type>(BuildController.Instance.CurrentlySelected.value.Cosmetics.Avatar.Id);
}
