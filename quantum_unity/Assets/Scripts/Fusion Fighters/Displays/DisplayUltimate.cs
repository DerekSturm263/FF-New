using Extensions.Components.UI;
using Extensions.Types;
using UnityEngine;

using Type = UltimateAsset;

public class DisplayUltimate : DisplayTextAndImage<Type>
{
    protected override Tuple<string, Sprite> GetInfo(Type item)
    {
        if (item)
            return new($"<font=\"KeaniaOne-Title SDF\"><size=50>{item.name}</size></font>\n\n{item.Description}", item.Icon);
        else
            return new("", null);
    }

    protected override Type GetValue() => UnityDB.FindAsset<Type>(BuildController.Instance.CurrentlySelected.value.Equipment.Ultimate.Id);
}
