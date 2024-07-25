using Extensions.Components.UI;
using Extensions.Types;
using Quantum;
using UnityEngine;

using Type = HairAsset;

public class DisplayHair : DisplayTextAndImage<Type>
{
    protected override Tuple<string, Sprite> GetInfo(Type item)
    {
        return new($"<font=\"KeaniaOne-Title SDF\"><size=50>{item.name}</size></font>\n\n{item.Description}", item.Icon);
    }

    protected override Type GetValue() => UnityDB.FindAsset<Type>(BuildController.Instance.CurrentlySelected.value.Cosmetics.Hair.Id);
}
